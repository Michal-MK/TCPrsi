using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameplayStatistics {

	public static Guid myNetworkGUID { get { return Guid.Parse(guidString); } }

	private Dictionary<Guid, ClientHistory> history = new Dictionary<Guid, ClientHistory>();

	private Dictionary<byte, ClientHistory> currentGame = new Dictionary<byte, ClientHistory>();

	private string filePath;
	private static string guidString;

	public GameplayStatistics() {
		if (!PlayerPrefs.HasKey("guid")) {
			PlayerPrefs.SetString("guid", Guid.NewGuid().ToString());
		}
		guidString = PlayerPrefs.GetString("guid");
		filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "clients.dat";
		BinaryFormatter bf = new BinaryFormatter();
		Player.OnVictory += Player_OnVictory;
		try {
			using (FileStream fs = File.Open(filePath, FileMode.Open)) {
				history = (Dictionary<Guid, ClientHistory>)bf.Deserialize(fs);
			}
		}
		catch (IOException) {
			Debug.Log("File does not exist, creating one!");
			try {
				using (FileStream fs = File.Create(filePath)) {
					bf.Serialize(fs, history);
				}
			}
			catch (IOException) {
				Debug.LogError("Failed to create a new client history file!");
			}
		}
	}

	private void Player_OnVictory(object sender, Player e) {
		Structs.LossPacket packet = new Structs.LossPacket(myNetworkGUID, e.index);
		foreach (KeyValuePair<byte, ClientHistory> kv in currentGame) {
			e.manager.SendLossPacket(packet);
		}
	}

	public ClientHistory OnClientConnected(Structs.ClientGUID clientGuid) {
		ClientHistory client;
		if (history.ContainsKey(clientGuid.guid)) {
			client = history[clientGuid.guid];
		}
		else {
			client = new ClientHistory(clientGuid.clientName);
			history.Add(clientGuid.guid, client);
		}
		currentGame.Add(clientGuid.playerId, client);
		Save();
		return client;
	}

	public void OnMatchLost(Structs.LossPacket winner) {
		history[winner.winnerGUID].CountLoss();
	}


	public void Save() {
		using (FileStream fs = File.Open(filePath, FileMode.Create)) {
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, history);
		}
		Debug.Log("Updated history log!");
	}
}

