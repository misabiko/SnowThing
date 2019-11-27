using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {
	CharacterController characterController;
	PlayerInput playerInput;
	InputAction lookAction;
	Vector2 moveInput;
	Material material;
	Color defaultColor;
	SnowBall snowBall;

	public CinemachineFreeLook cam;
	public Transform camTransform;
	public float camYSensitivity = 0.01f;
	public float camXSensitivity = 2f;

	public Interacter interacter;
	
	public float speed = 5f;

	public float ballPush = 50f;
	public float pushRadius = 3f;
	public float pushAngle = 45f;
	
	static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

	void Start() {
		characterController = GetComponent<CharacterController>();
		playerInput = GetComponent<PlayerInput>();
		material = GetComponent<Renderer>().material;
		defaultColor = material.GetColor(BaseColor);
		
		if (!cam)
			Debug.LogError("FreeLook camera wasn't set on PlayerController.");
		if (!camTransform)
			Debug.LogError("Camera transform (Main Camera) wasn't set on PlayerController.");
		if (!interacter)
			Debug.LogError("Interacter wasn't set on PlayerController.");

		playerInput.actions["Move"].started += OnMove;
		playerInput.actions["Move"].performed += OnMove;
		playerInput.actions["Move"].canceled += OnMove;
		
		playerInput.actions["Push"].started += OnPushStart;
		playerInput.actions["Push"].canceled += OnPushStop;

		lookAction = playerInput.actions["Look"];
	}

	void OnMove(InputAction.CallbackContext context) {
		Vector2 input = context.ReadValue<Vector2>();
		moveInput = input.normalized;
	}

	void OnPushStart(InputAction.CallbackContext context) {
		snowBall = interacter.GetSnowBall();
		
		material.SetColor(BaseColor, Color.red);
	}

	void OnPushStop(InputAction.CallbackContext context) {
		snowBall = null;
		
		material.SetColor(BaseColor, defaultColor);
	}

	void Update() {
		if (!(lookAction.activeControl?.device is Mouse) || Mouse.current.rightButton.isPressed) {
			Vector2 lookInput = lookAction.ReadValue<Vector2>();
			cam.m_XAxis.Value += camXSensitivity * lookInput.x;
			cam.m_YAxis.Value -= camYSensitivity * lookInput.y;
		}

		if (moveInput != Vector2.zero) {
			Vector3 moveDirection = camTransform.forward * moveInput.y + camTransform.right * moveInput.x;
			characterController.SimpleMove(speed * moveDirection);

			PushSnowBall(moveDirection);
		}else
			characterController.SimpleMove(Vector3.zero);
	}

	void PushSnowBall(Vector3 moveDirection) {
		if (!snowBall) return;
		
		Vector2 pushDirection2d = Helper.RemoveY(snowBall.transform.position) - Helper.RemoveY(transform.position);
		Vector3 pushDirection3d = new Vector3(pushDirection2d.x, 0f, pushDirection2d.y);
		Vector2 moveDirection2d = Helper.RemoveY(moveDirection);

		//if the ball is in the push fov
		if (!(Vector2.Angle(moveDirection2d, pushDirection2d) < pushAngle)) return;
		
		//TODO make that readable
		float modifier = 1f - Mathf.Clamp01(Vector2.Dot(pushDirection2d, moveDirection2d) / (pushRadius + snowBall.radius));
		snowBall.Push(modifier * ballPush * pushDirection3d.normalized);
	}
}