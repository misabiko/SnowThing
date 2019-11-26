using UnityEngine;
using UnityTemplateProjects;

public class SnowBall : MonoBehaviour {
	Vector2 lastPos;
	float deltaPos;
	float radius;

	public LowPolyTerrain terrain;
	public float drawStep = 0.5f;

	void Start() {
		if (terrain == null)
			Debug.LogError("LowPolyTerrain wasn't set in SnowBall.");
		
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
			
			terrain.DrawHeight(pos + radius * Vector3.down, -0.05f, 0.5f);
		}
	}
}