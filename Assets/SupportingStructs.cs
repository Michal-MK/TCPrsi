using System;
using UnityEngine;

public static class SupportingStructs {
	[Serializable]
	public struct SVector3 {
		public SVector3(float x, float y, float z) {
			this.x = x;
			this.z = z;
			this.y = y;
		}


		public float x { get; }
		public float y { get; }
		public float z { get; }


		public static implicit operator Vector3(SVector3 svec) {
			return new Vector3(svec.x, svec.y, svec.z);
		}

		public static implicit operator SVector3(Vector3 vec) {
			return new SVector3(vec.x, vec.y, vec.z);
		}
	}

	[Serializable]
	public struct CardPositionSync {

		public CardPositionSync(Vector3 position) {
			this.position = position;
		}

		public SVector3 position;
	}
}
