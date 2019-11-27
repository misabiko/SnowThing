using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Detector : MonoBehaviour {
	public Collider target;
	public bool contains {get; private set;}

	void OnTriggerEnter(Collider other) {
		if (other != target) return;
		
		Debug.Log("Detected");
		contains = true;
	}

	void OnTriggerExit(Collider other) {
		if (other != target) return;
		
		Debug.Log("Lost");
		contains = false;
	}
}