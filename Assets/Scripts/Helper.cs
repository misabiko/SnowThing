using UnityEngine;

public static class Helper {
	public static Vector2 RemoveY(Vector3 vector) {
		return new Vector2(vector.x, vector.z);
	}
}