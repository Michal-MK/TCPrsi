﻿using System;
using System.Collections.Generic;
using UnityEngine;
using static Structs;

public class Player : MonoBehaviour {

	public static event EventHandler<Player> OnEndTurn;

	public static event EventHandler<Player> OnVictory;

	public static event EventHandler<Card> OnCardPlayed;

	public static event EventHandler<Card.CardColor> OnColorSelected;

	private List<Card> hand;
	public GameManager manager;

	private bool waitingForInput;
	private bool turnInProgress;
	public bool controlledByLocal;

	public byte index;

	public string controllingClientName;

	public float angle;
	public float distance;

	public void Initialise(byte index, string playerName, GameManager manager) {
		this.name = playerName + "_PlayerObject";
		this.index = index;
		controllingClientName = playerName;
		this.manager = manager;
		if (index == manager.localId) {
			controlledByLocal = true;
		}
		hand = new List<Card>();
		GameManager.OnTurnBegin += BeginTurn;
	}

	public void OnDestroy() {
		GameManager.OnTurnBegin -= BeginTurn;
	}

	public void Draw(int numCards) {
		Card drawnCard = null;
		if (waitingForInput || !turnInProgress) {
			ArrangeCards();
			return;
		}
		for (int i = 0; i < numCards; i++) {
			Card c = manager.deckManager.DrawCard();
			c.currentOwner = this;

			c.IsFaceUp = controlledByLocal;
			c.myCollider.enabled = true;

			hand.Add(c);
			drawnCard = c;
			ArrangeCards();
		}
		if (controlledByLocal && manager.gameBegan) {
			manager.SendCardDrawn(new DrawCardAction(numCards, index));
		}
		turnInProgress = false;

		OnEndTurn(this, this);
	}

	public void Play(Card card) {
		if (waitingForInput || !turnInProgress) {
			ArrangeCards();
			return;
		}
		BaseCardBehaviour(card);

		switch (card.cardValue) {
			case Card.CardValue.Sedm: {
				manager.sevenState += 2;
				turnInProgress = false;
				OnEndTurn(this, this);
				break;
			}
			case Card.CardValue.Svrsek: {
				workingCard = card;
				if (controlledByLocal) {
					GetColorInput(card);
				}
				break;
			}
			case Card.CardValue.Eso: {
				manager.aceState = true;
				turnInProgress = false;
				OnEndTurn(this, this);
				break;
			}
			default: {
				turnInProgress = false;
				OnEndTurn(this, this);
				break;
			}
		}
		
		if (hand.Count == 0) {
			OnVictory?.Invoke(this, this);
		}
		ArrangeCards();
	}

	public bool CanPlay(Card card) {
		switch (card.cardValue) {
			case Card.CardValue.Sedm: {
				if ((manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue)
						&& manager.aceState == false) {
					return true;
				}
				return false;
			}
			case Card.CardValue.Svrsek: {
				if (manager.aceState == false && manager.sevenState == 0) {
					return true;
				}
				return false;
			}
			case Card.CardValue.Eso: {
				if ((manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue)
						&& manager.sevenState == 0) {
					return true;
				}
				return false;
			}
			default: {
				if ((manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue)
						&& manager.sevenState == 0 && manager.aceState == false) {
					return true;
				}
				return false;
			}
		}
	}

	private void BaseCardBehaviour(Card card) {
		card.MoveToTalon();
		hand.Remove(card);
		int order = manager.deckManager.talon.Peek().myRenderer.sortingOrder;
		manager.deckManager.talon.Push(card);
		card.myRenderer.sortingOrder = order + 1;
		card.currentOwner = null;
		OnCardPlayed?.Invoke(this,card);
		if (controlledByLocal) {
			manager.SendCardPlayed(new PlayCardAction(new CardInfo(card.cardColor, card.cardValue), index));
		}
	}

	public void ArrangeCards() {
		Vector2 middle;
		Quaternion cardAngle;
		float cardOverlap;
		Vector2 cardStep;
		Vector3 cardScale;

		if (!controlledByLocal) {
			
			middle = new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
			cardAngle = Quaternion.Euler(0, 0, 90 + Mathf.Rad2Deg * angle);
			cardOverlap = Constants.CardWidth * 0.25f;
			cardStep = new Vector2(-Mathf.Sin(angle) * cardOverlap, Mathf.Cos(angle) * cardOverlap);
			cardScale = Vector3.one;

			
		}
		else {
			int count = hand.Count;
			middle = new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
			cardAngle = Quaternion.Euler(0, 0, 90 + Mathf.Rad2Deg * angle);
			cardOverlap = Constants.CardWidth * 1;
			cardStep = new Vector2(-Mathf.Sin(angle) * cardOverlap, Mathf.Cos(angle) * cardOverlap);
			cardScale = Vector3.one * 2;

			float width = count * cardOverlap;
			if(width * 2 > manager.cameraWidth) {
				cardScale = Vector3.one * (manager.cameraWidth / width);
			}
		}
		for (int i = 0; i < hand.Count; i++) {
			Vector2 realPos = middle + ((float)(i - (hand.Count * 0.5 - 0.5f)) * cardStep * cardScale);
			hand[i].gameObject.GetComponent<RectTransform>().SetPositionAndRotation(realPos, cardAngle);
			hand[i].transform.localScale = cardScale;
			hand[i].myRenderer.sortingOrder = i;

		}
	}

	private Card workingCard;

	public void GetColorInput(Card card) {
		Controls.OnColorSelected += ColorSelected;
		manager.controls.colorSelector.SetActive(true);
		waitingForInput = true;
	}

	public void ColorSelected(object sender, Card.CardColor color) {
		waitingForInput = false;
		Controls.OnColorSelected -= ColorSelected;
		workingCard.cardColor = color;
		workingCard = null;
		OnColorSelected?.Invoke(this, color);

		if (controlledByLocal) {
			manager.SendExtraAction(new ExtraCardArgs(color, index));
		}
		turnInProgress = false;

		OnEndTurn(this, this);

	}



	public void BeginTurn(object sender, Player player) {
		if (player == this) {
			turnInProgress = true;
		}
	}

	private void FlipCards(bool visible) {
		foreach (Card card in hand) {
			card.IsFaceUp = visible;
		}
	}


	public Card cardFromCardInfo(CardInfo info) {
		foreach (Card card in hand) {
			if (card.cardValue == info.value && card.cardColor == info.color) {
				return card;
			}
		}
		throw new SynchronizationException(string.Format("SyncError, card {0} {1} not in '{2}'s hand!", info.color, info.value, controllingClientName));
	}
}
