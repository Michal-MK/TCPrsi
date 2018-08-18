using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Deck : MonoBehaviour {
	public Stack<Card> deck = new Stack<Card>();
	public Stack<Card> talon = new Stack<Card>();

	public GameObject blankCard;

	void Start () {
		for (int i = 0; i < Enum.GetNames(typeof(Card.CardColor)).Length; i++) {
			for (int j = 0; j < Enum.GetNames(typeof(Card.CardValue)).Length; j++) {
				Card newCard = GameObject.Instantiate(blankCard).GetComponent<Card>();
				newCard.Initialise((Card.CardValue)j, (Card.CardColor)i);

				deck.Push(newCard);
			}
		}


		deck.Shuffle();



	}
	


	// Update is called once per frame
	void Update () {
		
	}
}
