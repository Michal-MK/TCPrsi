using System.Collections.Generic;
using UnityEngine;
using System;
using Igor.TCP;
using static Structs;

public class GameManager : MonoBehaviour {

	public static event EventHandler<Player> OnTurnBegin;

	public Deck deckManager;
	public List<Player> players; //Server
	public int turnCounter;
	public GameObject playerPrefab;

	public InfoDisplayer display;
	public Controls controls;

	public Transform deckPlaceTransform;
	public Transform talonPlaceTransform;

	private Client client;
	public byte localId;


	public bool AceState;
	public int SevenState;


	public bool gameBegan = false;

	public void Initialise(InitialData data, Client client) {
		this.client = client;
		Player.OnEndTurn += PlayerTurnFinished;
		Player.OnVictory += VictoryHandling;
		deckManager.Initialise(Extensions.Invert(data.cards));
		localId = client.client.clientID;

		foreach (byte id in data.players.Keys) {
			players.Add(AddPlayer(id, data.players[id]));
		}
		foreach (Player player in players) {
			OnTurnBegin?.Invoke(this, player);
			player.Draw(4);
		}

		gameBegan = true;
		OnTurnBegin?.Invoke(this, players[0]);
	}

	private void OnDestroy() {
		Player.OnEndTurn -= PlayerTurnFinished;
		Player.OnVictory -= VictoryHandling;
	}

	private Player AddPlayer(byte index, string playerName) {
		Player newPlayer = Instantiate(playerPrefab).GetComponent<Player>();
		newPlayer.Initialise(index, playerName, this);
		return newPlayer;
	}

	private void VictoryHandling(object sender, Player winner) {
		Debug.Log("The winner is " + winner.name);

		client.client.getConnection.SendData(Constants.GameOverIdentifier);

	}

	private void PlayerTurnFinished(object sender, Player player) {
		turnCounter = (turnCounter == players.Count - 1) ? 0 : turnCounter + 1;
		OnTurnBegin?.Invoke(this, players[turnCounter]);
	}

	public void SendCardDrawn(DrawCardAction action) {
		client.client.getConnection.SendUserDefinedData(DrawPacketId, Helper.GetBytesFromObject(action));
	}

	public void SendCardPlayed(PlayCardAction action) {
		client.client.getConnection.SendUserDefinedData(PlayCardPacketId, Helper.GetBytesFromObject(action));
	}

	public void SendExtraAction(ExtraCardArgs action) {
		client.client.getConnection.SendUserDefinedData(ExtraCardArgsPacketId, Helper.GetBytesFromObject(action));
	}


	public void ShowSelectedColor(Card.CardColor color) {
		display.DisplayColor(color);
	}
}
