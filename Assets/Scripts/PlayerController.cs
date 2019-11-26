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
	Material ballMaterial;
	Color ballColor;
	static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

	public CinemachineFreeLook cam;
	public Transform camTransform;
	public float speed = 10f;
	//public Dictionary<
	public float camYSensitivity = 0.01f;
	public float camXSensitivity = 2f;
	public float ballPush = 10f;
	public float pushRadius = 1f;
	public float pushAngle = 15f;
	public SnowBall snowBall;

	void Start() {
		characterController = GetComponent<CharacterController>();
		playerInput = GetComponent<PlayerInput>();
		if (cam == null)
			Debug.LogError("FreeLook camera wasn't set on PlayerController.");
		if (camTransform == null)
			Debug.LogError("Camera transform (Main Camera) wasn't set on PlayerController.");
		if (snowBall) {
			ballMaterial = snowBall.GetComponent<Renderer>().material;
			ballColor = ballMaterial.GetColor(BaseColor);
		}

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

		ballMaterial.SetColor(BaseColor, ballColor);
		if (moveInput != Vector2.zero) {
			Vector3 moveDirection = camTransform.forward * moveInput.y + camTransform.right * moveInput.x;
			characterController.SimpleMove(speed * moveDirection);
			
			if (snowBall) {
				Vector2 pushDirection2d = Helper.RemoveY(snowBall.transform.position) - Helper.RemoveY(transform.position);
				Vector3 pushDirection3d = new Vector3(pushDirection2d.x, 0f, pushDirection2d.y);
				Vector2 moveDirection2d = Helper.RemoveY(moveDirection);

				if (Vector2.Angle(moveDirection2d, pushDirection2d) < pushAngle) {
					//TODO make that readable
					float modifier = 1f - Mathf.Clamp01(Vector2.Dot(pushDirection2d, moveDirection2d) / (pushRadius + snowBall.radius));
					snowBall.Push(modifier * ballPush * pushDirection3d.normalized);

					ballMaterial.SetColor(BaseColor, Color.blue);
				}
			}
		}
	}
}