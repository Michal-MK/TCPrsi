﻿using System.Collections.Generic;
using UnityEngine;
using System;
using Igor.TCP;
using static Structs;

public class GameManager : MonoBehaviour {

	public static event EventHandler<Player> OnTurnBegin;

	public static event EventHandler OnEffectStateChange;

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
	private float localDownOffset;

	private bool _aceState;
	private int _sevenState;

	public bool aceState {
		get {
			return _aceState;
		}
		set {			
			_aceState = value;
			OnEffectStateChange?.Invoke(this, EventArgs.Empty);
		}
	}
	public int sevenState {
		get {
			return _sevenState;
		}
		set {
			_sevenState = value;
			OnEffectStateChange?.Invoke(this, EventArgs.Empty);
			
		}
	}

	public float cameraWidth;

	public bool gameBegan = false;

	public void Initialise(InitialData data, Client client) {
		this.client = client;
		Player.OnEndTurn += PlayerTurnFinished;
		Player.OnVictory += VictoryHandling;
		deckManager.Initialise(Extensions.Invert(data.cards));
		localId = client.client.clientID;

		localDownOffset = -Mathf.PI / 2 + (- localId + data.players.Count) * ((2 * Mathf.PI) / (data.players.Count));

		Camera.main.orthographicSize = Constants.OverlapPreventionDistance + Constants.HalfCardLenght * 2 + Constants.MiddleRadius;
		cameraWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;


		foreach (byte id in data.players.Keys) {
			players.Add(AddPlayer(id, data.players[id], (byte)data.players.Count));

		}
		display.InstantiateTurnIndicators(players.ToArray());
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

	private Player AddPlayer(byte index, string playerName, byte numPlayers) {
		Player newPlayer = Instantiate(playerPrefab).GetComponent<Player>();
		newPlayer.angle = localDownOffset + (index * ((2 * Mathf.PI) / numPlayers));
		newPlayer.distance = Constants.HalfCardLenght + Constants.OverlapPreventionDistance + Constants.MiddleRadius;
		newPlayer.Initialise(index, playerName, this);
		return newPlayer;
	}

	private void VictoryHandling(object sender, Player winner) {
		display.DisplayText("The winner is " + winner.controllingClientName);

		if(localId == 0) {
			controls.InitialiseButtonOne("RestartGame");
			controls.OnButtonOnePress += WinConfirmed;
			
		}
	}

	private void WinConfirmed(object o, EventArgs e) {
		controls.OnButtonOnePress -= WinConfirmed;
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
}
