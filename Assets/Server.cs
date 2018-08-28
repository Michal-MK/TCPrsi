using UnityEngine;
using System.Collections;
using Igor.TCP;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviour {
	public TCPServer server;
	public Dictionary<byte, string> players = new Dictionary<byte, string>();
	private LobbyManager lm;

	public void Initialise(LobbyManager lm, ushort port) {
		this.lm = lm;
		
		
		server = new TCPServer();
		server.Start(port);
		server.OnConnectionEstablished += Server_OnConnectionEstablished;
		DontDestroyOnLoad(gameObject);
	}

	public void StartGame(bool firstTime = true) {
		if (firstTime) {
			SetUpTransmissionIds();
		}
		
		Structs.InitialData data = new Structs.InitialData(lm.GenerateCardInfo(), players);
		foreach (byte id in players.Keys) {
			server.GetConnection(id).SendUserDefinedData(Structs.InitialPacketId, Helper.GetBytesFromObject(data));
		}

	}

	private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
		players.Add(e.clientInfo.clientID, e.clientInfo.computerName);
		server.GetConnection(e.clientInfo.clientID).OnStringReceived += OnStringReceived;
		server.GetConnection(e.clientInfo.clientID).OnInt64Received += Server_OnInt64Received;
		lm.Print(e.clientInfo.computerName);

	}

	private void Server_OnInt64Received(object sender, PacketReceivedEventArgs<long> e) {
		if(e.data == 1) {
			StartGame(false);
		}
	}

	private void OnStringReceived(object sender, PacketReceivedEventArgs<string> e) {
		foreach (TCPClientInfo connectionInfo in server.getConnectedClients) {
			server.GetConnection(connectionInfo.clientID).SendData(connectionInfo.computerName + ": " + e.data);
		}
	}

	public void SetUpTransmissionIds() {
		foreach (TCPClientInfo connectionInfo in server.getConnectedClients) {
			DataIDs dataIDs = server.GetConnection(connectionInfo.clientID).dataIDs;
			dataIDs.DefineCustomDataTypeForID<Structs.InitialData>(Structs.InitialPacketId, null);
			dataIDs.DefineCustomDataTypeForID<Structs.PlayCardAction>(Structs.PlayCardPacketId, OnPlayCardPacketReceived);
			dataIDs.DefineCustomDataTypeForID<Structs.DrawCardAction>(Structs.DrawPacketId, OnDrawCardPacketReceived);
			dataIDs.DefineCustomDataTypeForID<Structs.ExtraCardArgs>(Structs.ExtraCardArgsPacketId, OnExtraCardArgsPacketReceived);
			dataIDs.DefineCustomDataTypeForID<Structs.CardPositionSyncPacket>(Structs.CardPosSyncId, OnCardPositionPacketReceived);
		}
	}

	private void OnCardPositionPacketReceived(Structs.CardPositionSyncPacket data) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.CardPosSyncId, Helper.GetBytesFromObject(data));
			}
		}
	}

	public void OnPlayCardPacketReceived(Structs.PlayCardAction data) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.PlayCardPacketId, Helper.GetBytesFromObject(data));
			}
		}
	}

	private void OnDrawCardPacketReceived(Structs.DrawCardAction data) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.DrawPacketId, Helper.GetBytesFromObject(data));
			}
		}
	}

	private void OnExtraCardArgsPacketReceived(Structs.ExtraCardArgs data) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.ExtraCardArgsPacketId, Helper.GetBytesFromObject(data));
			}
		}
	}
}
