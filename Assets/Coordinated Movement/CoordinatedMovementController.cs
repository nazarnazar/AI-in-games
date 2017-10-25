using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatedMovementController : MonoBehaviour {

	public GameObject characterPrefab;
	public int numberOfSlots;
	public float maxAcceleration;
	public float timeToSetSlots;

	private GameObject[] chars;
	private Vector3 targetPosition;
	private float radius;
	private bool wait;

	void Start ()
	{
		chars = new GameObject[numberOfSlots];
		targetPosition = new Vector3 (Random.Range (-5f, 5f), Random.Range (-3f, 3f), 0f);

		for (int i = 0; i < numberOfSlots; i++)
			chars [i] = Instantiate (characterPrefab);

		float size = chars [0].transform.localScale.x / ((float)numberOfSlots / 4f);
		for (int i = 0; i < numberOfSlots; i++)
			chars [i].transform.localScale = new Vector3 (size, size, size);
		radius = chars[0].transform.localScale.x / Mathf.Sin (Mathf.PI / numberOfSlots);

		timeToSetSlots /= ((float)numberOfSlots / 4f);

		chars [0].GetComponent<SpriteRenderer> ().color = Color.magenta;

		SetStartingPositions (Vector3.zero);
		wait = true;
	}

	void Update ()
	{
		if (!wait)
			for (int i = 0; i < numberOfSlots; i++) {
				if (i == 0) {
					Leader ();
				} else {
					if (!wait) {
						chars [i].transform.position = GetSlotLocation (i, chars [0].transform.position);
						chars [i].transform.eulerAngles = new Vector3 (0f, 0f, GetSlotOrientation (i, chars [0].transform.position) * 57.2958f);
					}
				}
			}
	}

	void Leader()
	{
		Vector2 velocity = new Vector2 (targetPosition.x - chars[0].transform.position.x, targetPosition.y - chars[0].transform.position.y);
		if (velocity.magnitude > 0.1f) {
			velocity.Normalize ();
			velocity *= maxAcceleration;
			chars[0].transform.position = new Vector3 (chars[0].transform.position.x + velocity.x, chars[0].transform.position.y + velocity.y, chars[0].transform.position.z);
			chars[0].transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (velocity.y, velocity.x) * 57.2958f);
		} else {
			velocity = Vector2.zero;
			targetPosition = new Vector3 (Random.Range (-5f, 5f), Random.Range (-3f, 3f), 0f);
			SetStartingPositions (chars [0].transform.position);
			wait = true;
		}
	}

	void SetStartingPositions(Vector3 center)
	{
		for (int i = 0; i < numberOfSlots; i++) {
			chars [i].transform.position = center;
			chars [i].transform.eulerAngles = new Vector3 (0f, 0f, 0f);
		}

		StartCoroutine (SetLeaderRotationAndReleaseCoroutines ());
	}

	IEnumerator SetLeaderRotationAndReleaseCoroutines()
	{
		float finish = Mathf.Atan2 (targetPosition.y - chars[0].transform.position.y, targetPosition.x - chars[0].transform.position.x) * 57.2958f;
		float start = chars [0].transform.eulerAngles.z;
		float t = 0f;

		while (t < 1f) {
			t += Time.deltaTime / timeToSetSlots;
			chars [0].transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Lerp (start, finish, t));
			yield return null;
		}

		StartCoroutine (Positioning (1));
		StartCoroutine (Turning (1));
	}

	IEnumerator Positioning(int index)
	{
		Vector3 finish = GetSlotLocation (index, chars [0].transform.position);
		Vector3 start = chars [index].transform.position;
		float t = 0f;

		while (t < 1f) {
			t += Time.deltaTime / timeToSetSlots;
			chars [index].transform.position = Vector3.Lerp (start, finish, t);
			yield return null;
		}

		if (index < numberOfSlots - 1)
			StartCoroutine (Positioning (index + 1));
		else
			wait = false;
	}

	IEnumerator Turning(int index)
	{
		float finish = GetSlotOrientation (index, chars [0].transform.position) * 57.2958f;
		float start = chars [index].transform.eulerAngles.z;
		float t = 0f;

		while (t < 1f) {
			t += Time.deltaTime / timeToSetSlots;
			chars [index].transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Lerp (start, finish, t));
			yield return null;
		}

		if (index < numberOfSlots - 1)
			StartCoroutine (Turning (index + 1));
		else
			wait = false;
	}


	Vector3 GetSlotLocation(int slotNumber, Vector3 leaderPos)
	{
		Vector2 center = new Vector2 (leaderPos.x - (targetPosition.x - leaderPos.x) / (targetPosition - leaderPos).magnitude * radius,
			                 leaderPos.y - (targetPosition.y - leaderPos.y) / (targetPosition - leaderPos).magnitude * radius);

		float leaderAngle = Mathf.Atan2 (targetPosition.y - leaderPos.y, targetPosition.x - leaderPos.x);

		float angleAroundCircle = leaderAngle + (float)slotNumber / (float)numberOfSlots * Mathf.PI * 2f;
		Vector3 location = new Vector3 (center.x + radius * Mathf.Cos (angleAroundCircle), center.y + radius * Mathf.Sin (angleAroundCircle), 0f);
		return location;
	}

	float GetSlotOrientation(int slotNumber, Vector3 leaderPos)
	{
		float leaderAngle = Mathf.Atan2 (targetPosition.y - leaderPos.y, targetPosition.x - leaderPos.x);
		return leaderAngle + (float)slotNumber / (float)numberOfSlots * Mathf.PI * 2f;
	}
}
