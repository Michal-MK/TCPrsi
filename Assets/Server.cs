using UnityEngine;
using System.Collections;
using Igor.TCP;

public class Server : Connection {
	public TCPServer server;

	public void Start() {
		base.AssignAsServer(true);
		server = new TCPServer();
		server.Start(25565);
		server.OnConnectionEstablished += Server_OnConnectionEstablished;
	}

	private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
		lm.Print(e.connInfo.connectedAddress + " has connected");
		server.GetConnection(e.connInfo.connectionID).OnStringReceived += OnStringReceived;

	}

	public override void SendString(string s) {
		server.GetConnection(0).SendData(s);
	}

	// Update is called once per frame
	public override void SetUpTransmissionIds() {
		foreach (ConnectionInfo connectionInfo in server.getConnectedClients) {
			server.GetConnection(connectionInfo.connectionID).dataIDs.DefineCustomDataTypeForID<InitialData>(initialPacketId, null);
		}
	}

	public void SendInitialData(InitialData data) {
		foreach (ConnectionInfo connectionInfo in server.getConnectedClients) {
			server.GetConnection(connectionInfo.connectionID).SendData(DataIDs.UserDefined, Helper.GetBytesFromObject<InitialData>(base.initialPacketId, data));
		}
	}
}
