using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	public Transform targetTransform;
	public float lerpSpeed;
	public float mouseSensitivity = 3f;

	private float yaw = 0f;
	private float pitch = 0f;

	void LateUpdate()
	{
		if (targetTransform == null) return;

		// Mouse rotation only when right click held
		if (Input.GetMouseButton(1))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
			pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
			pitch = Mathf.Clamp(pitch, -80f, 80f);
		}

		if (Input.GetMouseButtonUp(1))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		// Original follow behavior
		Vector3 currentPosition = transform.position;
		Vector3 targetPosition = targetTransform.position;
		currentPosition.y = 0.0f;
		targetPosition.y = 0.0f;

		Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, lerpSpeed);
		newPosition.y = transform.position.y;
		transform.position = newPosition;

		Vector3 lookTarget = targetTransform.position + Vector3.up * 1f;
		Vector3 lookDir = Quaternion.Euler(pitch, yaw, 0) * (lookTarget - transform.position);
		transform.LookAt(transform.position + lookDir);
	}
}
