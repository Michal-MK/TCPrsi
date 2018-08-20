using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Deck : MonoBehaviour {
	public Stack<Card> deck = new Stack<Card>();
	public Stack<Card> talon = new Stack<Card>();
	public GameManager manager;

	public GameObject blankCard;

	public void Initialise () {
		for (int i = 0; i < Enum.GetNames(typeof(Card.CardColor)).Length; i++) {
			for (int j = 0; j < Enum.GetNames(typeof(Card.CardValue)).Length; j++) {
				Card newCard = Instantiate(blankCard).GetComponent<Card>();
				newCard.Initialise((Card.CardValue)j, (Card.CardColor)i);
				newCard.manager = manager;
				newCard.MoveToDeck();
				deck.Push(newCard);

			}
		}


		deck.Shuffle();

		Card talonCard = deck.Pop();
		talonCard.MoveToTalon(); 
		talon.Push(talonCard);
		//deck.Peek().GetComponent<BoxCollider2D>().enabled = true;

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
		while(talon.Count > 0) {
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
