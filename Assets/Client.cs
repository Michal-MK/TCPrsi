using UnityEngine;
using System.Collections;
using Igor.TCP;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour {
	public TCPClient client;
	public LobbyManager lm;
	public GameManager gm;

	private string ipAddress;
	private ushort port;

	public bool isHost;


	#region ToMainThreadVariables
	private bool gotMessage;
	private PacketReceivedEventArgs<string> chatMessage;

	private bool gotDrawCard;
	private Structs.DrawCardAction draw;

	private bool gotExtraCard;
	private Structs.ExtraCardArgs extra;

	private bool gotCardPlayed;
	private Structs.PlayCardAction play;

	private bool gotInitData;
	private Structs.InitialData init;

	private bool gotServerState;
	private Structs.ServerState serverState;

	private bool gotNewClient;
	private Structs.NewClient newClient;
	#endregion

	public void Connect(string ip, ushort port) {
		this.ipAddress = ip;
		this.port = port;
		client = new TCPClient(ip, port);
		client.SetUpClientInfo(string.Format("({0})-{1}", FindObjectOfType<Server>() != null ? "Server" : "Client", Environment.UserName));
		client.Connect();
		client.getConnection.OnStringReceived += OnStringReceived;
		SetUpTransmissionIds();
		DontDestroyOnLoad(gameObject);
	}

	private void OnStringReceived(object sender, PacketReceivedEventArgs<string> e) {
		chatMessage = e;
		gotMessage = true;
	}

	private void OnDrawCard(Structs.DrawCardAction action, byte sender) {
		draw = action;
		gotDrawCard = true;
	}

	private void OnExtraInfo(Structs.ExtraCardArgs action, byte sender) {
		extra = action;
		gotExtraCard = true;
	}

	private void OnCardPlayed(Structs.PlayCardAction action, byte sender) {
		play = action;
		gotCardPlayed = true;
	}

	private void OnInitialData(Structs.InitialData data, byte sender) {
		init = data;
		gotInitData = true;
	}

	private void OnServerStateReceived(Structs.ServerState obj, byte sender) {
		serverState = obj;
		gotServerState = true;
	}

	private void OnNewClientConnected(Structs.NewClient obj, byte sender) {
		newClient = obj;
		gotNewClient = true;
	}


	public void SetUpTransmissionIds() {
		DataIDs ids = client.getConnection.dataIDs;

		try {
			ids.DefineCustomDataTypeForID<Structs.InitialData>(Structs.InitialPacketId, OnInitialData);
			ids.DefineCustomDataTypeForID<Structs.PlayCardAction>(Structs.PlayCardPacketId, OnCardPlayed);
			ids.DefineCustomDataTypeForID<Structs.DrawCardAction>(Structs.DrawPacketId, OnDrawCard);
			ids.DefineCustomDataTypeForID<Structs.ExtraCardArgs>(Structs.ExtraCardArgsPacketId, OnExtraInfo);
			ids.DefineCustomDataTypeForID<Structs.ServerState>(Structs.ServerStateId, OnServerStateReceived);
			ids.DefineCustomDataTypeForID<Structs.NewClient>(Structs.NewClientId, OnNewClientConnected);
			client.getConnection.OnStringReceived += OnStringReceived;
		}
		catch(Exception e) {
			Debug.LogWarning(e.Message + "\n" + e.InnerException?.Message);
			lm.Print(e.Message + "\n" + e.InnerException?.Message);
		}
	}


	private void Update() {
		if (gotMessage) {
			lm.Print(chatMessage.data);
			gotMessage = false;
		}
		if (gotCardPlayed) {
			gm.players[play.playerId].Play(gm.players[play.playerId].cardFromCardInfo(play.playedCard));
			gotCardPlayed = false;
		}
		if (gotDrawCard) {
			if(draw.numCards != 1) {
				if(draw.numCards == 0) {
					gm.aceState = false;
					gm.players[draw.playerId].Draw(draw.numCards);
				}
				if(draw.numCards >= 2) {
					gm.sevenState = 0;
					gm.players[draw.playerId].Draw(draw.numCards);
				}
				
			}
			else {
				gm.players[draw.playerId].Draw(draw.numCards);
			}
			
			gotDrawCard = false;
		}
		if (gotExtraCard) {
			gm.players[extra.playerId].ColorSelected(this,extra.cardColor);
			gotExtraCard = false;
		}
		if (gotInitData) {
			SceneManager.LoadScene(Constants.Scene_Game);
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}
		if (gotServerState) {
			if (!isHost) {
				lm.Print("Connected to: " + ipAddress + ":" + port);
			}
			foreach (TCPClientInfo info in serverState.connectedClients) {
				if (info.clientID == client.clientID) {
					lm.Print("(You)" + client.clientInfo.computerName);
				}
				else {
					lm.Print(info.computerName);
				}
			}
			gotServerState = false;
		}
		if (gotNewClient) {
			lm.Print(newClient.clientInfo.computerName + " just connected!");
			gotNewClient = false;
		}
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
		gm = GameObject.Find(Constants.GameManagerName).GetComponent<GameManager>();
		gm.Initialise(init, this);
		gotInitData = false;
		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
	}
}
