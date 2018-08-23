using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public static event EventHandler<Player> OnEndTurn;

	public static event EventHandler<Player> OnVictory;

	private List<Card> hand;
	public GameManager manager;

	private bool waitingForInput;
	private bool turnInProgress;
	
	private Card workingCard;

	public int index;

	public void Initialise() {
		hand =  new List<Card>();
		GameManager.OnTurnBegin += BeginTurn;
 	}

	public void Draw(int numCards) {
		if (waitingForInput || !turnInProgress) {
			ArrangeCards();
			return;
		}
		for (int i = 0; i < numCards; i++) {
			Card c = manager.deckManager.DrawCard();
			c.currentOwner = this;
			c.IsFaceUp = true;
			c.collider.enabled = true;

			hand.Add(c);
			ArrangeCards();
		}
		EndTurn();
	}

	public void Play(Card card) {
		if (waitingForInput  || !turnInProgress) {
			ArrangeCards();
			return;
		}

		switch (card.cardValue) {
			case Card.CardValue.Sedm: {
				if (manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue) {
					card.MoveToTalon();
					hand.Remove(card);
					int order = manager.deckManager.talon.Peek().myRenderer.sortingOrder;
					manager.deckManager.talon.Push(card);
					card.myRenderer.sortingOrder = order + 1;
					card.currentOwner = null;
					EndTurn();
				}
				break;
			}
			case Card.CardValue.Svrsek: {
				card.MoveToTalon();
				hand.Remove(card);
				workingCard = card;

				int order = manager.deckManager.talon.Peek().myRenderer.sortingOrder;
				manager.deckManager.talon.Push(card);
				card.myRenderer.sortingOrder = order + 1;
				card.currentOwner = null;
				GetColorInput();
				break;
			}
			case Card.CardValue.Eso: {
				if (manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue) {
					card.MoveToTalon();
					hand.Remove(card);
					int order = manager.deckManager.talon.Peek().myRenderer.sortingOrder;
					manager.deckManager.talon.Push(card);
					card.myRenderer.sortingOrder = order + 1;
					card.currentOwner = null;
					EndTurn();
				}
				break;
			}
			default: {
				if (manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue) {
					card.MoveToTalon();
					hand.Remove(card);
					int order = manager.deckManager.talon.Peek().myRenderer.sortingOrder;
					manager.deckManager.talon.Push(card);
					card.myRenderer.sortingOrder = order + 1;
					card.currentOwner = null;
					EndTurn();
				}
				break;
			}
		}

		if (hand.Count == 0) {
			OnVictory?.Invoke(this, this);
		}
		ArrangeCards();
	}

	public void ArrangeCards() {
		for (int i = 0; i < hand.Count; i++) {
			if(index == 0) {
				hand[i].transform.position = new Vector2(-11 + i * 2, -7f);
			}
			else if (index == 1) {
				hand[i].transform.position = new Vector2(-11 + i * 2, 7f);
			}
			hand[i].myRenderer.sortingOrder = i;
		}
	}

	public void GetColorInput() {
		Controls.OnColorSelected += ColorSelected;
		Controls.RaiseOnControlSwitch(true);
		waitingForInput = true;
	}

	public void ColorSelected(Card.CardColor color) {
		workingCard.cardColor = color;
		workingCard = null;
		waitingForInput = false;
		Controls.OnColorSelected -= ColorSelected;
		EndTurn();
	}

	private void EndTurn() {
		turnInProgress = false;
		OnEndTurn(this,this);
		FlipCards(false);
	}

	public void BeginTurn(object sender, Player player) {
		if(player == this) {
			turnInProgress = true;
			FlipCards(true);
		}
	} 

	private void FlipCards(bool visible) {
		foreach (Card card in hand) {
			card.IsFaceUp = visible;
		}
	}
}
