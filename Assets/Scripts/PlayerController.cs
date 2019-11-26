using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {
	CharacterController characterController;
	PlayerInput playerInput;
	InputAction lookAction;
	Vector2 moveInput;

	public CinemachineFreeLook cam;
	public Transform camTransform;
	public float speed = 10f;
	//public Dictionary<
	public float camYSensitivity = 0.01f;
	public float camXSensitivity = 2f;
	public float ballPush = 10f;
	public float pushRadius = 1f;
	public Rigidbody snowBall;
	
	void Start() {
		characterController = GetComponent<CharacterController>();
		playerInput = GetComponent<PlayerInput>();
		if (cam == null)
			Debug.LogError("FreeLook camera wasn't set on PlayerController.");
		if (camTransform == null)
			Debug.LogError("Camera transform (Main Camera) wasn't set on PlayerController.");

		playerInput.actions["Move"].started += OnMove;
		playerInput.actions["Move"].performed += OnMove;
		playerInput.actions["Move"].canceled += OnMove;
		playerInput.actions["Fire"].performed += OnFire;

		lookAction = playerInput.actions["Look"];

		playerInput.onDeviceLost += input => Debug.Log("Device lost");
		playerInput.onDeviceRegained += input => Debug.Log("Device regained");
	}

	void OnMove(InputAction.CallbackContext context) {
		Vector2 input = context.ReadValue<Vector2>();
		moveInput = input.normalized;
	}

	void OnFire(InputAction.CallbackContext context) {
		Debug.Log("OnFire: " + context.valueType);
	}

	void Update() {
		if (!(lookAction.activeControl?.device is Mouse) || Mouse.current.rightButton.isPressed) {
			Vector2 lookInput = lookAction.ReadValue<Vector2>();
			cam.m_XAxis.Value += camXSensitivity * lookInput.x;
			cam.m_YAxis.Value -= camYSensitivity * lookInput.y;
		}
		
		if (moveInput != Vector2.zero) {
			Vector3 direction = camTransform.forward * moveInput.y + camTransform.right * moveInput.x;
			characterController.SimpleMove(speed * direction);
			
			if (snowBall) {
				Vector3 pushDirection = snowBall.transform.position - transform.position;
				pushDirection.y = 0;
				//here modifier is pushDirection projected on player's direction
				float modifier = Vector3.Dot(pushDirection, direction);
				pushDirection.Normalize();

				if (modifier > 0) {
					//and we turn it into a modifier 0 to 1, 1 being closer to the player, and 0 being at pushRadius
					modifier = 1f - Mathf.Clamp01(modifier / pushRadius);
					snowBall.AddForce(modifier * ballPush * pushDirection, ForceMode.Acceleration);
				}
			}
		}
	}
}