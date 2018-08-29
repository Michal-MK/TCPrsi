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

	public Sprite face;
	public Sprite cover;

	public SpriteRenderer myRenderer;
	public BoxCollider2D myCollider;

	public GameManager manager;
	public Player currentOwner;

	private bool isFaceUp;

	public CardValue cardValue { get; private set; }
	public CardColor cardColor { get; set; }

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

	public void Initialise(CardValue cV, CardColor cC) {
		myRenderer = GetComponent<SpriteRenderer>();
		myCollider = GetComponent<BoxCollider2D>();

		cardValue = cV;
		cardColor = cC;
		SetSprites();
	}

	private void SetSprites() {
		face = Resources.Load<Sprite>("Cards/" + cardColor.ToString() + "/" + cardValue.ToString());
		IsFaceUp = false;
	}

	public void Drag() {
		transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public void MoveToTalon() {
		transform.position = manager.talonPlaceTransform.position;
		transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
		transform.localScale = Vector3.one;
		myCollider.enabled = false;
		IsFaceUp = true;
	}

	public void MoveToDeck() {
		transform.position = manager.deckPlaceTransform.position;
		transform.rotation = Quaternion.identity;
		myCollider.enabled = false;
		IsFaceUp = false;
	}

	public void OnEndDrag(PointerEventData eventData) {
		RaycastHit2D[] hits = Physics2D.RaycastAll(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal);

		foreach (RaycastHit2D hit2D in hits) {
			if (hit2D.transform.name == "_TalonCollider" && currentOwner.controlledByLocal == true && currentOwner.CanPlay(this)) {
				currentOwner.Play(this);
				return;
			}
		}
		currentOwner.ArrangeCards();
	}
}
