using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Igor.TCP;

public abstract class Connection : MonoBehaviour {

	public bool isServer;


	public LobbyManager lm;

	public GameManager gm;


	public byte initialPacketId = 3;

	public void AssignAsServer(bool yes) {
		DontDestroyOnLoad(gameObject);
		isServer = yes;
	}

	public abstract void SendString(string s);

	protected void OnStringReceived(object sender, string e) {
		lm.Print(e);
	}

	public abstract void SetUpTransmissionIds();


	[System.Serializable]
	public struct InitialData {
		public InitialData(Stack<Card> cards) {
			this.cards = new Stack<CardInfo>();
			while (cards.Count > 0) {
				this.cards.Push(cardToCardInfo(cards.Pop()));
			}

		}

		public Stack<CardInfo> cards { get; }

	}

	[System.Serializable]
	public struct CardInfo {
		public CardInfo(Card.CardColor color, Card.CardValue value) {
			this.color = color;
			this.value = value;
		}

		public Card.CardColor color { get; }
		public Card.CardValue value { get; }
	}

	public static CardInfo cardToCardInfo(Card card) {
		return new CardInfo(card.cardColor, card.cardValue);
	}

}
