using UnityEngine;
using System.Collections;
using Igor.TCP;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour {
	public TCPClient client;
	public LobbyManager lm;
	public GameManager gm;

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

	#endregion

	public void Connect(string ip, ushort port) {
		client = new TCPClient(ip, port);
		client.SetUpClientInfo(string.Format("({0})-{1}", FindObjectOfType<Server>() != null ? "Server" : "Client", Environment.UserName));
		client.Connect();
		client.getConnection.OnStringReceived += OnStringReceived;
		SetUpTransmissionIds();
		DontDestroyOnLoad(gameObject);
		lm.Print("Connected to: " + ip +":"+ port);
	}

	private void OnStringReceived(object sender, PacketReceivedEventArgs<string> e) {
		chatMessage = e;
		gotMessage = true;
	}

	private void OnDrawCard(Structs.DrawCardAction action) {
		draw = action;
		gotDrawCard = true;
	}

	private void OnExtraInfo(Structs.ExtraCardArgs action) {
		extra = action;
		gotExtraCard = true;
	}

	private void OnCardPlayed(Structs.PlayCardAction action) {
		play = action;
		gotCardPlayed = true;
	}

	private void OnInitialData(Structs.InitialData data) {
		init = data;
		gotInitData = true;
	}


	public void SetUpTransmissionIds() {
		DataIDs ids = client.getConnection.dataIDs;
		ids.DefineCustomDataTypeForID<Structs.InitialData>(Structs.InitialPacketId, OnInitialData);
		ids.DefineCustomDataTypeForID<Structs.PlayCardAction>(Structs.PlayCardPacketId, OnCardPlayed);
		ids.DefineCustomDataTypeForID<Structs.DrawCardAction>(Structs.DrawPacketId, OnDrawCard);
		ids.DefineCustomDataTypeForID<Structs.ExtraCardArgs>(Structs.ExtraCardArgsPacketId, OnExtraInfo);
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
					gm.AceState = false;
					gm.players[draw.playerId].Draw(draw.numCards);
				}
				if(draw.numCards >= 2) {
					gm.SevenState = 0;
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
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
		gm = GameObject.Find(Constants.GameManagerName).GetComponent<GameManager>();
		gm.Initialise(init, this);
		gotInitData = false;
		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
	}
}
