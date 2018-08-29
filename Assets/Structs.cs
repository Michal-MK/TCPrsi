using System.Collections.Generic;
﻿using System;
using UnityEngine;

public class Structs {

	public const byte InitialPacketId = 3;
	public const byte PlayCardPacketId = 4;
	public const byte DrawPacketId = 5;
	public const byte ExtraCardArgsPacketId = 6;
	public const byte LossId = 8;
	public const byte GUID = 10;

	[Serializable]
	public struct InitialData {
		public InitialData(Stack<CardInfo> cards, Dictionary<byte, string> players) {
			this.cards = cards;
			this.players = players;
		}

		public Stack<CardInfo> cards { get; }
		public Dictionary<byte, string> players { get; }
	}

	[Serializable]
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


	[Serializable]
	public struct PlayCardAction {

		public PlayCardAction(CardInfo card, byte playerId) {
			this.playerId = playerId;
			playedCard = card;
		}

		public byte playerId;
		public CardInfo playedCard;

	}

	[Serializable]
	public struct ExtraCardArgs {

		public ExtraCardArgs(Card.CardColor color, byte playerId) {
			cardColor = color;
			this.playerId = playerId;
		}
		public Card.CardColor cardColor;
		public byte playerId;

	}

	[Serializable]
	public struct DrawCardAction {

		public DrawCardAction(int numCards, byte playerId) {
			this.numCards = numCards;
			this.playerId = playerId;
		}

		public byte playerId;
		public int numCards;

	}

	[Serializable]
	public struct ClientGUID {
		public ClientGUID(Guid guid, string clientName, byte playerId) {
			this.guid = guid;
			this.playerId = playerId;
			this.clientName = clientName;
		}

		public Guid guid;
		public byte playerId;
		public string clientName;
	}

	[Serializable]
	public struct LossPacket {
		public LossPacket(Guid winnerGUID, byte playerId) {
			this.winnerGUID = winnerGUID;
			this.playerId = playerId;
		}

		public Guid winnerGUID;
		public byte playerId;
	}

}
