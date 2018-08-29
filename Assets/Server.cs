using UnityEngine;
using System.Collections;
using Igor.TCP;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

	private async void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
		players.Add(e.clientInfo.clientID, e.clientInfo.computerName);
		server.DefineRequestEntry<Structs.ClientGUID>(e.clientInfo.clientID, Structs.GUID);
		e.myServer.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<Structs.ClientGUID>(Structs.GUID, null);
		TCPResponse response = await server.RaiseRequestAsync(e.clientInfo.clientID, Structs.GUID);
		using (MemoryStream ms = new MemoryStream()) {
			ms.Write(response.rawData, 0, response.rawData.Length);
			ms.Seek(0, SeekOrigin.Begin);
			BinaryFormatter bf = new BinaryFormatter();
			Structs.ClientGUID newg = (Structs.ClientGUID)bf.Deserialize(ms);
			OnGUIDReceived(newg);
		}
		server.GetConnection(e.clientInfo.clientID).OnStringReceived += OnStringReceived;
		server.GetConnection(e.clientInfo.clientID).OnInt64Received += Server_OnInt64Received;
		lm.Print(e.clientInfo.computerName);
	}

	private void Server_OnInt64Received(object sender, PacketReceivedEventArgs<long> data) {
		if (data.data == 1) {
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

	private void OnLossPacketReceived(Structs.LossPacket data) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				server.GetConnection(id).SendUserDefinedData(Structs.LossId, Helper.GetBytesFromObject(data));
			}
		}
	}

	private void OnGUIDReceived(Structs.ClientGUID data) {
		foreach (byte id in players.Keys) {
			if (id != data.playerId) {
				byte[] dataa;
				using (MemoryStream ms = new MemoryStream()) {
					BinaryFormatter bf = new BinaryFormatter();
					bf.Serialize(ms, data);
					dataa = ms.ToArray();
				}
				server.GetConnection(id).SendUserDefinedData(Structs.GUID, dataa);
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
