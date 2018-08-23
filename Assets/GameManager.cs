using System.Collections.Generic;
using UnityEngine;
using System;
using Igor.TCP;

public class GameManager : MonoBehaviour {

	public static event EventHandler<Player> OnTurnBegin;

	public Deck deckManager;
	public List<Player> players;
	public int turnCounter;
	public GameObject playerPrefab;
	public Vector3 talonPlace;
	public Vector3 deckPlace;

	public Connection connectionManager;
	public Server serverManager;
	public Client clientManager;
	

	void Start () {
		connectionManager = GameObject.Find("ConnectionGO").GetComponent<Connection>();
		if (connectionManager.isServer) {
			serverManager = GameObject.Find("ConnectionGO").GetComponent<Server>();
			players.Add(AddPlayer(0));
			deckPlace = new Vector3(5, 0, 0); //TODO
			talonPlace = new Vector3(-5, 0, 0); //TODO
			Connection.InitialData data = new Connection.InitialData(deckManager.Initialise());
			serverManager.SendInitialData(data);

			Player.OnEndTurn += PlayerTurnFinished;
			Player.OnVictory += VictoryHandling;

			OnTurnBegin?.Invoke(this, players[0]);
			foreach (ConnectionInfo info in serverManager.server.getConnectedClients) {
				players.Add(AddPlayer(info.connectionID + 1));


			}
			foreach (Player player in players) {
				OnTurnBegin?.Invoke(this, player);
				player.Draw(4);
			}
		}
		else if(connectionManager.isServer == false) {
			clientManager = GameObject.Find("ConnectionGO").GetComponent<Client>();
			players.Add(AddPlayer(0));
			deckPlace = new Vector3(5, 0, 0); //TODO
			talonPlace = new Vector3(-5, 0, 0); //TODO

			deckManager.Initialise(Extensions.Invert(clientManager.dataForInitialisation.cards));
			Player.OnEndTurn += PlayerTurnFinished;
			Player.OnVictory += VictoryHandling;
			foreach (Player player in players) {
				OnTurnBegin?.Invoke(this, player);
				player.Draw(4);
			}


			OnTurnBegin?.Invoke(this, players[0]);
		}
		
	}

	private Player AddPlayer(int index) {
		GameObject newPlayer = Instantiate(playerPrefab);
		Player playerScript = newPlayer.GetComponent<Player>();
		playerScript.index = index;
		playerScript.manager = this;
		playerScript.Initialise();
		return playerScript;
	}
	
	private void VictoryHandling(object sender, Player winner) {
		Debug.Log("The winner is " + winner.name);
	}

	private void PlayerTurnFinished(object sender, Player player) {
		turnCounter += 1;
		if (turnCounter == players.Count) {
			turnCounter = 0;
		}
		OnTurnBegin?.Invoke(this,players[turnCounter]);

	}

}
