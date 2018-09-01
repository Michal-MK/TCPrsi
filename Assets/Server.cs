using UnityEngine;
using System.Collections;
using Igor.TCP;
using System.Collections.Generic;
using System;

public class Server : MonoBehaviour {
	public TCPServer server;
	public Dictionary<byte, string> players = new Dictionary<byte, string>();
	private LobbyManager lm;

	public bool isInitialised;

	public void Initialise(LobbyManager lm, ushort port) {
		this.lm = lm;
		server = new TCPServer();
		server.Start(port);
		server.OnConnectionEstablished += Server_OnConnectionEstablished;
		DontDestroyOnLoad(gameObject);
		lm.Print("Hosting on " + Helper.GetActiveIPv4Address() + ":" + port);
		isInitialised = true;
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
		e.myServer.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<Structs.ServerState>(Structs.ServerStateId, null);
		e.myServer.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<Structs.NewClient>(Structs.NewClientId, null);
		server.GetConnection(e.clientInfo.clientID).OnStringReceived += OnStringReceived;
		server.GetConnection(e.clientInfo.clientID).OnInt64Received += Server_OnInt64Received;
		foreach (TCPClientInfo info in server.getConnectedClients) {
			if (e.clientInfo.clientID != info.clientID) {
				server.GetConnection(info.clientID).SendUserDefinedData(Structs.NewClientId, Helper.GetBytesFromObject(new Structs.NewClient(e.clientInfo, e.clientInfo.clientID)));
			}
		}
		server.GetConnection(e.clientInfo.clientID).SendUserDefinedData(Structs.ServerStateId, Helper.GetBytesFromObject(new Structs.ServerState(e.myServer.getConnectedClients, e.clientInfo.clientID)));
	}


	private void Server_OnInt64Received(object sender, PacketReceivedEventArgs<long> e) {
		if(e.data == Constants.GameOverIdentifier) {
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
			dataIDs.DefineCustomDataTypeForID<Structs.LossPacket>(Structs.LossId, OnLossPacketReceived);
		}
	}

	private void OnLossPacketReceived(Structs.LossPacket data, byte sender) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.LossId, Helper.GetBytesFromObject(data));
			}
		}
	}

	public void OnPlayCardPacketReceived(Structs.PlayCardAction data, byte sender) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.PlayCardPacketId, Helper.GetBytesFromObject(data));
			}
		}
	}

	private void OnDrawCardPacketReceived(Structs.DrawCardAction data, byte sender) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.DrawPacketId, Helper.GetBytesFromObject(data));
			}
		}
	}

	private void OnExtraCardArgsPacketReceived(Structs.ExtraCardArgs data, byte sender) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.ExtraCardArgsPacketId, Helper.GetBytesFromObject(data));
			}
		}
	}
}
