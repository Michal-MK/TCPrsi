using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Deck deckManager;
	public List<Player> players;
	public int turnCounter;
	public GameObject playerPrefab;
	public Vector3 talonPlace;
	public Vector3 deckPlace;

	// Use this for initialization
	void Start () {
		players.Add(AddPlayer());
		deckPlace = new Vector3(5, 0, 0);
		talonPlace = new Vector3(-5, 0, 0);
		print(talonPlace);
		deckManager.Initialise();
	}

	Player AddPlayer() {
		GameObject newPlayer = Instantiate(playerPrefab);
		Player playerScript = newPlayer.GetComponent<Player>();
		playerScript.manager = this;
		return playerScript;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
