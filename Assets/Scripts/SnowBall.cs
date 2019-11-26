using UnityEngine;

public class SnowBall : MonoBehaviour {
	Vector2 lastPos;
	float deltaPos;
	new Rigidbody rigidbody;

	public LowPolyTerrain terrain;
	public float radius {get; private set;}
	public float drawStep = 0.5f;
	public float growthRate = 0.01f;
	public float digDepth = 0.2f;
	public float digRadius = 1f;

	void Start() {
		if (terrain == null)
			Debug.LogError("LowPolyTerrain wasn't set in SnowBall.");
		rigidbody = GetComponent<Rigidbody>();
		
		lastPos = Helper.RemoveY(transform.position);
		radius = transform.localScale.y / 2;
	}

	void LateUpdate() {
		Vector3 pos = transform.position;
		Vector2 newPos = Helper.RemoveY(pos);

		deltaPos += Vector2.Distance(lastPos, newPos);
		lastPos = newPos;

		if (deltaPos > drawStep) {
			deltaPos -= drawStep;
			AmassSnow();
		}
	}

	void AmassSnow() {
		transform.localScale += growthRate * Vector3.one;
		radius += growthRate / 2;
		
		terrain.DrawHeight(transform.position + radius * Vector3.down, -digDepth * radius, digRadius * radius);
	}


	public void Push(Vector3 pushForce) {
		rigidbody.AddForce(pushForce, ForceMode.Acceleration);
	}
}