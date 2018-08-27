using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDisplayer : MonoBehaviour {

	public Sprite kule;
	public Sprite listy;
	public Sprite srdce;
	public Sprite zaludy;

	public SpriteRenderer colorDisplay;

	private bool showColorForOneTurn;

	public SpriteRenderer[] playerArrows;

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
		playerArrows[(e.index + 1) % playerArrows.Length].color = Color.green;
		
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

	public void ShowPlayerTurn(byte index) {

	}


	public void HideColor() {
		colorDisplay.enabled = false;
	}
}
