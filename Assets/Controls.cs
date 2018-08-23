using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {


	public delegate void ColorSelect(Card.CardColor color);
	public static ColorSelect OnColorSelected;

	public delegate void ControlSwitch(bool on);
	public static event ControlSwitch OnControlSwitch;
	public SpriteRenderer myRenderer;
	public CircleCollider2D myCollider;

	private void Start() {
		myRenderer = gameObject.GetComponent<SpriteRenderer>();
		myCollider = gameObject.GetComponent<CircleCollider2D>();
		OnControlSwitch += SwitchSprite;
	}

	public void Kule () {
		OnColorSelected?.Invoke(Card.CardColor.Kule);
		OnControlSwitch?.Invoke(false);
	}
	public void Listy() {
		OnColorSelected?.Invoke(Card.CardColor.Listy);
		OnControlSwitch?.Invoke(false);
	}
	public void Srdce() {
		OnColorSelected?.Invoke(Card.CardColor.Srdce);
		OnControlSwitch?.Invoke(false);
	}
	public void Zaludy() {
		OnColorSelected?.Invoke(Card.CardColor.Zaludy);
		OnControlSwitch?.Invoke(false);
	}

	void SwitchSprite(bool on) {
		myRenderer.enabled = on;
		myCollider.enabled = on;
	}

	public static void RaiseOnControlSwitch(bool on) {
		OnControlSwitch?.Invoke(on);
	}
}
