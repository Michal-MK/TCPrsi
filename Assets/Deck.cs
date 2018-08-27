using System.Collections.Generic;
using UnityEngine;
using System;
using static Structs;

public class Deck : MonoBehaviour {
	public Stack<Card> deck = new Stack<Card>();
	public Stack<Card> talon = new Stack<Card>();
	public GameManager manager;

	public GameObject blankCard;



	public void Initialise(Stack<CardInfo> cardInfoStack) {
		while(cardInfoStack.Count > 0) {
			CardInfo cardInfo = cardInfoStack.Pop();
			Card newCard = Instantiate(blankCard).GetComponent<Card>();
			newCard.gameObject.name = (cardInfo.color.ToString() + " - " + cardInfo.value.ToString());
			newCard.Initialise(cardInfo.value, cardInfo.color);
			newCard.manager = manager;
			newCard.MoveToDeck();
			deck.Push(newCard);
		}
		Card talonCard = deck.Pop();
		talonCard.MoveToTalon();
		talon.Push(talonCard);
	}

	public Card DrawCard() {
		if (deck.Count > 1) {
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
		if (manager.players[manager.turnCounter].controlledByLocal) {
			if(manager.AceState == true) {
				manager.players[manager.turnCounter].Draw(0);
				manager.AceState = false;
			}
			else if(manager.SevenState != 0) {
				manager.players[manager.turnCounter].Draw(manager.SevenState);
				manager.SevenState = 0;
			}
			else {
				manager.players[manager.turnCounter].Draw(1);
			}
			
		}
	}
}
