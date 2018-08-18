using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Card : MonoBehaviour {

	public enum CardValue {
		Sedm,
		Osm,
		Devet,
		Deset,
		Spodek,
		Svrsek,
		Kral,
		Eso

	}

	public enum CardColor {
		Srdce,
		Kule,
		Listy,
		Zaludy

	}

	public SpriteRenderer myRenderer;
	public Sprite face;
	public Sprite cover;
	private bool faceUp;
	public bool IsFaceUp {
		get {
			return faceUp;
		}
		set {
			if (value == true) {
				myRenderer.sprite = face;
			}
			else {
				myRenderer.sprite = cover;
			}
			faceUp = value;
		}
	}
	public CardValue cardValue { get; private set; }
	public CardColor cardColor { get; private set; }

	public void Initialise(CardValue cV, CardColor cC) {
		myRenderer = gameObject.GetComponent<SpriteRenderer>();
		cardValue = cV;
		cardColor = cC;
		SetSprites();
		gameObject.GetComponent<BoxCollider2D>().enabled = false;

	}

	private void SetSprites() {
		face = Resources.Load<Sprite>("Cards/" + cardColor.ToString() + "/" + cardValue.ToString());
		IsFaceUp = false;
	}
}
