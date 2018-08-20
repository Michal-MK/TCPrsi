using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private List<Card> hand;
	public GameManager manager;
	// Use this for initialization
	void Start () {
		hand = new List<Card>();
	}

	public void Draw(int numCards) {
		for (int i = 0; i < numCards; i++) {
			Card c = manager.deckManager.DrawCard();
			c.currentOwner = this;
			hand.Add(c);
			c.IsFaceUp = true;
			c.collider.enabled = true;
			ArrangeCards();
		}
	}

	public void Play(Card card) {
		if(manager.deckManager.talon.Peek().cardColor == card.cardColor || manager.deckManager.talon.Peek().cardValue == card.cardValue) {
			card.MoveToTalon();
			hand.Remove(card);
			int order = manager.deckManager.talon.Peek().myRenderer.sortingOrder;
			manager.deckManager.talon.Push(card);
			card.myRenderer.sortingOrder = order + 1;
			card.currentOwner = null;
		}
		else {
			ArrangeCards();
		}
	}

	public void ArrangeCards() {
		for (int i = 0; i < hand.Count; i++) {
			hand[i].transform.position = new Vector2(-11 + i * 2, -7f);
		}
	}
}
