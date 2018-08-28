using System;
using System.Collections.Generic;
using UnityEngine;
using static Structs;

public class Player : MonoBehaviour {

	public static event EventHandler<Player> OnEndTurn;

	public static event EventHandler<Player> OnVictory;

	private List<Card> hand;
	public GameManager manager;

	private bool waitingForInput;
	private bool turnInProgress;
	public bool controlledByLocal;
	
	private Card workingCard;

	public byte index;

	public string controllingClientName;

	public void Initialise(byte index, string playerName, GameManager manager) {
		this.name = playerName + "_PlayerObject";
		this.index = index;
		controllingClientName = playerName;
		this.manager = manager;
		if (controllingClientName == System.Environment.UserName) {
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
		if (controlledByLocal  && manager.gameBegan) {
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
				manager.SevenState += 2;
				OnEndTurn(this, this);
				break;
			}
			case Card.CardValue.Svrsek: {
				if (controlledByLocal) {
					GetColorInput();
				}
				break;
			}
			case Card.CardValue.Eso: {
				manager.AceState = true;
				OnEndTurn(this, this);
				break;
			}
			default: {
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
						&& manager.AceState == false) {
					return true;
				}
				return false;
			}
			case Card.CardValue.Svrsek: {
				if (manager.AceState == false && manager.SevenState == 0) {
					return true;
				}
				return false;
			}
			case Card.CardValue.Eso: {
				if ((manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue)
						&& manager.SevenState == 0) {
					return true;
				}
				return false;
			}
			default: {
				if ((manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue)
						&& manager.SevenState == 0 && manager.AceState == false) {
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
		if (controlledByLocal) {
			manager.SendCardPlayed(new PlayCardAction(new CardInfo(card.cardColor, card.cardValue), index));
		}
	}

	public void ArrangeCards() {
		for (int i = 0; i < hand.Count; i++) {
			if(index == 0) {
				hand[i].transform.position = new Vector2(-11 + i * 2, -7f);
			}
			else {
				hand[i].transform.position = new Vector2(-11 + i * 2, 7f);
				
			}
			hand[i].myRenderer.sortingOrder = i;
		}
	}

	public void GetColorInput() {
		Controls.OnColorSelected += ColorSelected;
		manager.controls.colorSelector.SetActive(true);
		waitingForInput = true;
	}

	public void ColorSelected(object sender,Card.CardColor color) {
		
		waitingForInput = false;
		Controls.OnColorSelected -= ColorSelected;
		workingCard.cardColor = color;
		workingCard = null;
		manager.ShowSelectedColor(color);

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
			if(card.cardValue == info.value && card.cardColor == info.color) {
				return card;
			}
		}
		throw new SynchronizationException(string.Format("SyncError, card {0} {1} not in '{2}'s hand!", info.color, info.value, controllingClientName));
	}
}
