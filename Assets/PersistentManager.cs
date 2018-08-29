using UnityEngine;

public class PersistentManager : MonoBehaviour {
	public static PersistentManager instance;
	private void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this) {
			instance = null;
		}
	}

	public GameplayStatistics statistics;


	private void OnDestroy() {
		instance = null;
	}
}
