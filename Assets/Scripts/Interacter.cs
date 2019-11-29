using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interacter : MonoBehaviour {
	List<SnowBall> snowBalls = new List<SnowBall>();
	
	void OnTriggerEnter(Collider other) {
		SnowBall snowBall = other.GetComponent<SnowBall>();
		if (!snowBall) return;
		
		//snowBall.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.cyan);

		snowBalls.Add(snowBall);
	}

	void OnTriggerExit(Collider other) {
		SnowBall snowBall = other.GetComponent<SnowBall>();
		if (!snowBall) return;
		
		//snowBall.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);

		snowBalls.Remove(snowBall);
	}

	//Get the ball closer to be in front of the player
	public SnowBall GetSnowBall() {
		if (snowBalls.Count == 0)
			return null;
		
		SnowBall snowBall = snowBalls[0];
		float angle = GetAngle(snowBall);

		foreach (SnowBall ball in snowBalls) {
			if (GetAngle(ball) < angle)
				snowBall = ball;
		}

		return snowBall;
	}

	float GetAngle(SnowBall snowBall) {
		return Vector2.Angle(
			Helper.RemoveY(transform.forward),
			Helper.RemoveY(snowBall.transform.position - transform.position)
		);
	}

	public void Remove(SnowBall snowBall) {
		snowBalls.Remove(snowBall);
	}
}