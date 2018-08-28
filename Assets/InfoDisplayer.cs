using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDisplayer : MonoBehaviour {

	public GameObject infoDisplay;
	public GameObject turnIndicatorPrefab;

	public Sprite kule;
	public Sprite listy;
	public Sprite srdce;
	public Sprite zaludy;

	public SpriteRenderer colorDisplay;

	private bool showColorForOneTurn;

	private List<SpriteRenderer> playerArrows = new List<SpriteRenderer>();

	private void Start() {
		Player.OnEndTurn += Player_OnEndTurn;
	}

	private void OnDestroy() {
		Player.OnEndTurn -= Player_OnEndTurn;
	}

	private void Player_OnEndTurn(object sender, Player e) {
		if (showColorForOneTurn == false && colorDisplay.enabled == true) {
			showColorForOneTurn = true;
		}
		else {
			HideColor();
			showColorForOneTurn = false;
		}
		foreach (SpriteRenderer r in playerArrows) {
			r.color = Color.red;
		}
		playerArrows[(e.index + 1) % playerArrows.Count].color = Color.green;
		
	}

	public void DisplayColor(Card.CardColor color) {
		
		colorDisplay.enabled = true;
		showColorForOneTurn = false;
		switch (color) {
			case Card.CardColor.Kule: {
				colorDisplay.sprite = kule;
				break;
			}
			case Card.CardColor.Listy: {
				colorDisplay.sprite = listy;
				break;
			}
			case Card.CardColor.Srdce: {
				colorDisplay.sprite = srdce;
				break;
			}
			case Card.CardColor.Zaludy: {
				colorDisplay.sprite = zaludy;
				break;
			}
		}
	}

	public void InstantiateTurnIndicators(Player[] players) {
		for (int i = 0; i < players.Length; i++) {
			Player p = players[i];
			Vector2 direction = new Vector3(Mathf.Cos(p.angle), Mathf.Sin(p.angle));
			if (p.controlledByLocal) {
				direction = direction * (Constants.MiddleRadius * 0.5f);
			}
			else {
				direction = direction * (Constants.MiddleRadius);

			}
			

			playerArrows.Add(Instantiate(turnIndicatorPrefab, direction, new Quaternion(0, 0, p.angle, 0), infoDisplay.transform).GetComponent<SpriteRenderer>());
			if(i == 0) {
				playerArrows[0].color = Color.green;
			}
		}
	}

	public void HideColor() {
		colorDisplay.enabled = false;
	}
}
