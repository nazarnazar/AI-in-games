using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPointController : MonoBehaviour {

	public GameObject character;
	public float rotationSpeed;
	public float moveSpeed;

	private Coroutine lastRotationRoutine;
	private Coroutine lastMoveRoutine;

	void Start ()
	{
		lastRotationRoutine = null;
		lastMoveRoutine = null;
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0) && lastRotationRoutine == null && lastMoveRoutine == null) {
			lastRotationRoutine = StartCoroutine (RotateCharacter ());
			lastMoveRoutine = StartCoroutine (MoveCharacter ());
		}
	}

	IEnumerator MoveCharacter()
	{
		Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector2 moveDirection = new Vector2 (mousePositionInWorld.x - character.transform.position.x, mousePositionInWorld.y - character.transform.position.y);

		while (Mathf.Sqrt(Mathf.Pow(character.transform.position.x - mousePositionInWorld.x, 2) + Mathf.Pow (character.transform.position.y - mousePositionInWorld.y, 2)) > 0.1f) {
			character.transform.position = new Vector3 (character.transform.position.x + moveDirection.x * moveSpeed * Time.deltaTime, character.transform.position.y + moveDirection.y * moveSpeed * Time.deltaTime, character.transform.position.z);
			yield return null;
		}
		lastMoveRoutine = null;
	}

	IEnumerator RotateCharacter()
	{
		Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		float targetAngle = Mathf.Atan2 (mousePositionInWorld.y - character.transform.position.y, mousePositionInWorld.x - character.transform.position.x);
		targetAngle = targetAngle * 57.2958f;
		if (targetAngle < 0)
			targetAngle += 360f;
		Debug.Log ("target angle: " + targetAngle);

		float currentAngle = 0;
		if (character.transform.localEulerAngles.z < 0f)
			currentAngle = character.transform.localEulerAngles.z + 360f;
		else if (character.transform.localEulerAngles.z > 360f)
			currentAngle = character.transform.localEulerAngles.z - 360f;
		else
			currentAngle = character.transform.localEulerAngles.z;
		Debug.Log ("current angle: " + currentAngle);

		while (Mathf.Abs(Mathf.RoundToInt (currentAngle) - Mathf.RoundToInt (targetAngle)) > 5f) {
			if (currentAngle < targetAngle) {
				if (Mathf.Abs (currentAngle - targetAngle) < 180f)
					currentAngle += (targetAngle - currentAngle) / 2f * rotationSpeed * Time.deltaTime;
				else
					currentAngle -= (targetAngle - currentAngle) / 2f * rotationSpeed * Time.deltaTime;
			} else {
				if (Mathf.Abs (currentAngle - targetAngle) < 180f)
					currentAngle -= (currentAngle - targetAngle) / 2f * rotationSpeed * Time.deltaTime;
				else
					currentAngle += (currentAngle - targetAngle) / 2f * rotationSpeed * Time.deltaTime;
			}
			if (currentAngle > 360f)
				currentAngle -= 360f;
			else if (currentAngle < 0)
				currentAngle += 360f;
			character.transform.localEulerAngles = new Vector3 (character.transform.localEulerAngles.x, character.transform.localEulerAngles.y, currentAngle);
			yield return null;
		}
		lastRotationRoutine = null;
	}
}
