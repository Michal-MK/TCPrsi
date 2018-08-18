using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	List<Card> hand;
	// Use this for initialization
	void Start () {
		hand = new List<Card>();
	}
}
