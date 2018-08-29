using System;
[Serializable]
public class SynchronizationException : Exception {
	public SynchronizationException() {	}

	public SynchronizationException(string message) : base(message) {  }
}
