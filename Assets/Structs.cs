using System.Collections.Generic;

public class Structs {

	public const byte InitialPacketId = 3;
	public const byte PlayCardPacketId = 4;
	public const byte DrawPacketId = 5;
	public const byte ExtraCardArgsPacketId = 6;

	[System.Serializable]
	public struct InitialData {
		public InitialData(Stack<CardInfo> cards, Dictionary<byte, string> players) {
			this.cards = cards;
			this.players = players;
		}

		public Stack<CardInfo> cards { get; }
		public Dictionary<byte, string> players { get; }
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


	[System.Serializable]
	public struct PlayCardAction {

		public PlayCardAction(CardInfo card, byte playerId) {
			this.playerId = playerId;
			playedCard = card;
		}

		public byte playerId;
		public CardInfo playedCard;

	}

	[System.Serializable]
	public struct ExtraCardArgs {

		public ExtraCardArgs(Card.CardColor color, byte playerId) {
			cardColor = color;
			this.playerId = playerId;
		}
		public Card.CardColor cardColor;
		public byte playerId;

	}

	[System.Serializable]
	public struct DrawCardAction {

		public DrawCardAction(int numCards, byte playerId) {
			this.numCards = numCards;
			this.playerId = playerId;
		}

		public byte playerId;
		public int numCards;

	}

}
