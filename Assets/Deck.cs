using System.Collections.Generic;
using UnityEngine;
using System;

public class Deck : MonoBehaviour {
	public Stack<Card> deck = new Stack<Card>();
	public Stack<Card> talon = new Stack<Card>();
	public GameManager manager;

	public GameObject blankCard;

	public Stack<Card> Initialise() {
		for (int i = 0; i < Enum.GetNames(typeof(Card.CardColor)).Length; i++) {
			for (int j = 0; j < Enum.GetNames(typeof(Card.CardValue)).Length; j++) {
				Card newCard = Instantiate(blankCard).GetComponent<Card>();
				newCard.gameObject.name = ((Card.CardColor)i).ToString() + " - " + ((Card.CardValue)j).ToString();
				newCard.Initialise((Card.CardValue)j, (Card.CardColor)i);
				newCard.manager = manager;
				newCard.MoveToDeck();
				deck.Push(newCard);
			}
		}

		deck.Shuffle();

		Stack<Card> copyOfStack = new Stack<Card>(deck);

		PutStartingCardInTalon();
		return copyOfStack;

		
	}

	public void Initialise(Stack<Connection.CardInfo> cardInfoStack) {
		while(cardInfoStack.Count > 0) {
			Connection.CardInfo cardInfo = cardInfoStack.Pop();
			Card newCard = Instantiate(blankCard).GetComponent<Card>();
			newCard.gameObject.name = (cardInfo.color.ToString() + " - " + cardInfo.value.ToString());
			newCard.Initialise(cardInfo.value, cardInfo.color);
			newCard.manager = manager;
			newCard.MoveToDeck();
			deck.Push(newCard);
		}	
		PutStartingCardInTalon();
	}

	private void PutStartingCardInTalon() {
		Card talonCard = deck.Pop();
		talonCard.MoveToTalon();
		talon.Push(talonCard);
	}

	public Card DrawCard() {
		if (deck.Count >= 1) {
			return deck.Pop();
		}
		else {
			FlipTalon();
			return deck.Pop();
		}
	}

	private void FlipTalon() {
		Card topCard = talon.Pop();
		while (talon.Count > 0) {
			Card c = talon.Pop();
			c.MoveToDeck();
			deck.Push(c);
		}
		topCard.MoveToTalon();
		talon.Push(topCard);
	}

	public void OnDeckClick() {
		manager.players[manager.turnCounter].Draw(1);
	}
}
