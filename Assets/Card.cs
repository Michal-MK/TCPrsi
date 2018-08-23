using UnityEngine;
using UnityEngine.EventSystems;


[System.Serializable]
public class Card : MonoBehaviour, IEndDragHandler {

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

	public GameManager manager;
	public SpriteRenderer myRenderer;
	public Sprite face;
	public Sprite cover;
	private bool isFaceUp;
	new public BoxCollider2D collider;
	public Player currentOwner;

	public bool IsFaceUp {
		get {
			return isFaceUp;
		}
		set {
			if (value == true) {
				myRenderer.sprite = face;
			}
			else {
				myRenderer.sprite = cover;
			}
			isFaceUp = value;
		}
	}

	public CardValue cardValue { get; private set; }
	public CardColor cardColor { get; set; }

	public void Initialise(CardValue cV, CardColor cC) {
		myRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<BoxCollider2D>();

		cardValue = cV;
		cardColor = cC;
		SetSprites();
	}

	private void SetSprites() {
		face = Resources.Load<Sprite>("Cards/" + cardColor.ToString() + "/" + cardValue.ToString());
		IsFaceUp = false;
	}

	public void Drag() {
		gameObject.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public void MoveToTalon() {
		gameObject.GetComponent<RectTransform>().position = manager.talonPlace;
		transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
		collider.enabled = false;
		IsFaceUp = true;
	}

	public void MoveToDeck() {
		gameObject.GetComponent<RectTransform>().position = manager.deckPlace;
		transform.rotation = Quaternion.identity;
		collider.enabled = false;
		IsFaceUp = false;
	}


	public void OnEndDrag(PointerEventData eventData) {
		RaycastHit2D[] hits = Physics2D.RaycastAll(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal);

		foreach (RaycastHit2D hit2D in hits) {
			if (hit2D.transform.name == "TalonCollider") {
				currentOwner.Play(this);
				return;
			}
		}
		currentOwner.ArrangeCards();
	}
}
