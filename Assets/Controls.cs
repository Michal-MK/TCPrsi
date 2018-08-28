using System;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {
	public static event EventHandler<Card.CardColor> OnColorSelected;

	public GameObject colorSelector;

	public void Kule() {
		OnColorSelected?.Invoke(this,Card.CardColor.Kule);
		colorSelector.SetActive(false);
	}
	public void Listy() {
		OnColorSelected?.Invoke(this,Card.CardColor.Listy);
		colorSelector.SetActive(false);
	}
	public void Srdce() {
		OnColorSelected?.Invoke(this,Card.CardColor.Srdce);
		colorSelector.SetActive(false);
	}
	public void Zaludy() {
		OnColorSelected?.Invoke(this,Card.CardColor.Zaludy);
		colorSelector.SetActive(false);
	}
}
