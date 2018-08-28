using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Igor.TCP;
using System.Threading;
using System.Collections.Generic;
using static Structs;
using System;

public class LobbyManager : MonoBehaviour {

	public GameObject connectionOBJ;

	public TMP_InputField textToSend;

	public TextMeshProUGUI chatbox;

	public TMP_InputField adressPort;

	public TMP_InputField serverPort;

	public Client client;
	private Server server;


	private void Start() {
		adressPort.text = Helper.GetActiveIPv4Address().ToString() + ":256";
	}

	public void IamServer() {
		server = connectionOBJ.AddComponent<Server>();
		ushort port;
		if (ushort.TryParse(serverPort.text, out port)) {

		}
		else {
			port = 256;
		}
		server.Initialise(this, port);
		AddClient().Connect(Helper.GetActiveIPv4Address().ToString(), port);
	}

	public void IamClient() {
		AddClient().Connect(adressPort.text.Split(':')[0], ushort.Parse(adressPort.text.Split(':')[1]));
	}

	Client AddClient() {
		client = connectionOBJ.AddComponent<Client>();
		client.lm = this;
		return client;
	}

	//Unnecessary ?
	public void Print(string s) {
		this.s = s;
		change = true;
	}
	private string s = "";
	private bool change = false;

	private void Update() {
		if (change) {
			chatbox.text += ("\n" + s);
			change = false;
			s = "";
		}
	}
	//

	//No refenreces
	public void Send() {
		client.client.getConnection.SendData(textToSend.text);
		textToSend.text = "";
	}

	//1 reference lm.Play()
	public void Play() {
		server.StartGame();
	}

	public Stack<CardInfo> GenerateCardInfo() {
		Stack<CardInfo> copyOfStack = new Stack<CardInfo>();

		for (int i = 0; i < Enum.GetNames(typeof(Card.CardColor)).Length; i++) {
			for (int j = 0; j < Enum.GetNames(typeof(Card.CardValue)).Length; j++) {
				copyOfStack.Push(new CardInfo((Card.CardColor)i, (Card.CardValue)j));
			}
		}
		copyOfStack.Shuffle();
		return copyOfStack;

	}
}

