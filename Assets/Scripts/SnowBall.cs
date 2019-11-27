using System;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour {
	Vector3 lastPos;
	Vector3 lastDirection;
	float deltaPos;
	new Rigidbody rigidbody;
	List<Vector3> hits;

	public LowPolyTerrain terrain;
	public float radius {get; private set;}
	public float drawStep = 0.5f;
	public float growthRate = 0.01f;
	public float digDepth = 0.2f;
	public float digForce = 0.2f;
	public float forwardDig = 0.2f;

	void Start() {
		if (terrain == null)
			Debug.LogError("LowPolyTerrain wasn't set in SnowBall.");
		
		rigidbody = GetComponent<Rigidbody>();

		hits = new List<Vector3>();
		
		lastPos = transform.position;
		radius = transform.localScale.y / 2;
	}

	void LateUpdate() {
		Vector3 pos = transform.position;
		Vector3 newPos = pos;

		deltaPos += Vector2.Distance(lastPos, newPos);
		lastDirection = (newPos - lastPos).normalized;
		lastPos = newPos;
		
		CheckAmass();
	}

	void CheckAmass() {
		if (!(deltaPos > drawStep)) return;
		
		Vector3? groundDirection = GetGroundDirection();
		if (!groundDirection.HasValue) return;
		
		Ray ray = new Ray(transform.position, groundDirection.Value);
		RaycastHit hit;
		if (!terrain.meshCollider.Raycast(ray, out hit, radius + 1.5f)) return;
		hits.Add(hit.point);

		deltaPos -= drawStep;
		AmassSnow(hit.point);
	}

	void AmassSnow(Vector3? hitPos) {
		transform.localScale += growthRate * Vector3.one;
		radius += growthRate / 2;

		if (hitPos.HasValue)
			terrain.DrawHeight(hitPos.Value, -digForce * radius, digDepth * radius);
		else
			terrain.DrawHeight(transform.position + digDepth * radius * GetGroundDirection().Value + lastDirection * forwardDig, -digForce * radius, digDepth * radius);
	}


	public void Push(Vector3 pushForce) {
		rigidbody.AddForce(pushForce, ForceMode.Acceleration);
	}

	Vector3? GetGroundDirection() {
		Vector3 onPlane = Vector3.Cross(lastDirection, Vector3.up);
		return Vector3.Cross(lastDirection, onPlane).normalized;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		foreach (Vector3 hit in hits)
			Gizmos.DrawSphere(hit, 0.1f);
	}
}