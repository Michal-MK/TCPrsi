using System;

[Serializable]
public class ClientHistory {

	public string clientName { get; }
	public int losses { get; private set; }
	public int wins { get; private set; }
	public float ratio { get { return wins / (float)losses; } }


	public ClientHistory(string clientName) {
		this.clientName = clientName;
	}
	
	public void CountLoss() {
		losses++;
	}

	public void CountWin() {
		wins++;
	}
}

