using UnityEngine;

public class SnowBall : MonoBehaviour {
	Vector2 lastPos;
	float deltaPos;
	new Rigidbody rigidbody;
	new Collider collider;

	public LowPolyTerrain terrain;
	public float radius {get; private set;}
	public float drawStep = 0.5f;
	public float growthRate = 0.01f;
	public float digDepth = 0.2f;
	public float digForce = 0.2f;
	public float crushedHeight = 2f;

	void Start() {
		if (terrain == null)
			Debug.LogError("LowPolyTerrain wasn't set in SnowBall.");
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		
		lastPos = Helper.RemoveY(transform.position);
		radius = transform.localScale.y / 2;
	}

	void LateUpdate() {
		Vector3 pos = transform.position;
		Vector2 newPos = Helper.RemoveY(pos);

		deltaPos += Vector2.Distance(lastPos, newPos);
		lastPos = newPos;

		if (!collider.isTrigger && deltaPos > drawStep) {
			deltaPos -= drawStep;
			AmassSnow();
		}
	}

	void AmassSnow() {
		transform.localScale += growthRate * Vector3.one;
		radius += growthRate / 2;
		
		terrain.DrawHeight(transform.position + digDepth * radius * Vector3.down, -digForce * radius, radius);
	}


	public void Push(Vector3 pushForce) {
		rigidbody.AddForce(pushForce, ForceMode.Acceleration);
	}

	public void Crush() {
		terrain.DrawHeightSmooth(transform.position + radius * Vector3.down, crushedHeight * radius, 1.5f * radius);
		Destroy(gameObject);
	}

	public void DisableCollider() {
		collider.isTrigger = true;
		Destroy(rigidbody);
	}

	public void EnableCollider() {
		collider.isTrigger = false;
		rigidbody = gameObject.AddComponent<Rigidbody>();
	}

	public void Throw(Vector3 force) {
		rigidbody.AddForce(force, ForceMode.Impulse);
	}
}