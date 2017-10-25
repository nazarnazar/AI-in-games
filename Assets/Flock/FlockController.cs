using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour {

	public GameObject[] chars;
	public float distanceThreshold;
	public float decayCoefficient;
	public float maxAcceleration;
	public Vector2 targetPosition;

	private Vector2 firstFlockVelocity;

	void Update ()
	{
		for (int i = 0; i < chars.Length; i++) {
			if (i == 0) {
				FirstFlock (chars [i]);
			} else {
				Flock (chars [i]);
			}
		}
	}

	void FirstFlock(GameObject ch)
	{
		firstFlockVelocity = new Vector2 (targetPosition.x - ch.transform.position.x, targetPosition.y - ch.transform.position.y);
		if (firstFlockVelocity.magnitude > 0.1f) {
			firstFlockVelocity.Normalize ();
			firstFlockVelocity *= maxAcceleration;
			ch.transform.position = new Vector3 (ch.transform.position.x + firstFlockVelocity.x, ch.transform.position.y + firstFlockVelocity.y, ch.transform.position.z);
			ch.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (firstFlockVelocity.y, firstFlockVelocity.x) * 57.2958f);
		} else {
			firstFlockVelocity = Vector2.zero;
			targetPosition = new Vector2 (Random.Range (-5f, 5f), Random.Range (-3f, 3f));
			ResetPositions ();
		}
	}

	void Flock(GameObject ch)
	{
		Vector2 chVelocity = firstFlockVelocity;
		Vector2 direction;
		float strength;
		for (int i = 0; i < chars.Length; i++) {
			direction = chars [i].transform.position - ch.transform.position;
			if (ch != chars[i] && direction.magnitude < distanceThreshold) {
				strength = maxAcceleration * (distanceThreshold - direction.magnitude) / distanceThreshold;
				direction.Normalize ();
				direction *= strength;
				chVelocity = new Vector2 (chVelocity.x - direction.x, chVelocity.y - direction.y);
			}
		}
		ch.transform.position = new Vector3 (ch.transform.position.x + chVelocity.x, ch.transform.position.y + chVelocity.y, ch.transform.position.z);
		ch.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (chVelocity.y, chVelocity.x) * 57.2958f);
	}

	void ResetPositions()
	{
		for (int i = 1; i < chars.Length; i++) {
			chars [i].transform.position = chars [0].transform.position;
		}
	}
}
