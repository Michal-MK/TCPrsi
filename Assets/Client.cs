using UnityEngine;
using System.Collections;
using Igor.TCP;

public class Client : Connection {
	public TCPClient client;
	// Use this for initialization
	public override void SendString(string s) {
		client.getConnection.SendData(s);
	}

	bool loadGame;
	public InitialData dataForInitialisation;

	public void Connect(string ip, ushort port) {

		base.AssignAsServer(false);
		client = new TCPClient(ip, port);
		client.getConnection.OnStringReceived += OnStringReceived;

		SetUpTransmissionIds();
	}
	// Update is called once per frame
	public override void SetUpTransmissionIds() {
		client.getConnection.dataIDs.DefineCustomDataTypeForID<InitialData>(initialPacketId, OnInitialDataReceived);
	}

	public void OnInitialDataReceived(InitialData initialData) {
		dataForInitialisation = initialData;
		loadGame = true;
	}

	private void Update() {
		if (loadGame) {
			loadGame = false;
			lm.Play();
		}
	}
}
