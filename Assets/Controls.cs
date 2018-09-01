using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour {
	public static event EventHandler<Card.CardColor> OnColorSelected;
	public event EventHandler OnButtonOnePress;


	public GameObject colorSelector;

	public GameObject buttonOne;

	public void InitialiseButtonOne(string text) {
		buttonOne.GetComponentInChildren<Text>().text = text;
		buttonOne.SetActive(true);
	}

	public void ButtonOnePress() {
		OnButtonOnePress?.Invoke(this, EventArgs.Empty);
		buttonOne.SetActive(false);
	}

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
