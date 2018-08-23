using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {

	public GameObject connectionOBJ;

	public TMP_InputField textToSend;

	public TextMeshProUGUI chatbox;

	public Connection connection;

	public TMP_InputField adressPort;

	private void Start() {
		
	}
	// Use this for initialization
	public void IamServer () {
		connection = connectionOBJ.AddComponent<Server>();
		connection.lm = this;
	}
	
	// Update is called once per frame
	public void IamClient () {
		Client c = connectionOBJ.AddComponent<Client>();
		connection = c;
		c.Connect(adressPort.text.Split(':')[0], ushort.Parse(adressPort.text.Split(':')[1]));
		connection.lm = this;
	}

	public void Print(string s) {
		this.s = s;
		change = true;
	}
	private string s = "";
	private bool change = false;

	private void Update() {
		if (change) {
			chatbox.text += ("\n " + s);
			change = false;
			s = "";
		}
	}

	public void Send() {
		connection.SendString(textToSend.text);
		textToSend.text = "";
	}

	public void Play() {
		if (connection.isServer) {
			connection.SetUpTransmissionIds();
		}
		SceneManager.LoadScene("GameScene");
	}
}

