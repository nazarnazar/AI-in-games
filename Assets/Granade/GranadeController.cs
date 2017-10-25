using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeController : MonoBehaviour {

	public GameObject character;
	public GameObject granadePrefab;
	public float granadeSpeed;
	public float g;
	public float granadeThreshold;
	public float charSpeed;

	bool granadeIsThrown = false;
	GameObject granade;
	Vector2 startPos;
	Vector2 direction;
	float t = 0f;
	Vector3 goAwayTarget;
	Vector3 goAwayDirection;
	bool goAway = false;

	void Update ()
	{
		if (Input.GetMouseButtonUp(0) && !granadeIsThrown) {
			granadeIsThrown = true;
			Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			direction = new Vector2 (mouseInWorld.x - character.transform.position.x, mouseInWorld.y - character.transform.position.y);
			granadeSpeed *= direction.magnitude;
			direction.Normalize ();
			granade = Instantiate (granadePrefab);
			granade.transform.position = character.transform.position;
			startPos = new Vector2 (granade.transform.position.x, granade.transform.position.y);
			Invoke ("CalculateLandingSpot", 0.25f);
		}

		if (granadeIsThrown && granade.transform.position.y > -2.75f) {
			t += Time.deltaTime;
			granade.transform.position = new Vector3 (startPos.x + (direction.x * granadeSpeed * t),
				startPos.y + (direction.y * granadeSpeed * t) - (g * t * t / 2f), granade.transform.position.z);
		}

		if (goAway) {
			if ((character.transform.position - goAwayTarget).magnitude > 0.1f) {
				character.transform.position = new Vector3 (character.transform.position.x + goAwayDirection.x * charSpeed * Time.deltaTime, character.transform.position.y, character.transform.position.z);
				character.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (0f, goAwayDirection.x) * 57.2958f);
			}
		}
	}

	void CalculateLandingSpot()
	{
		float tempTime = 0f;
		Vector2 tempGranadePos = granade.transform.position;
		while (tempGranadePos.y > -2.75f) {
			tempGranadePos = new Vector2 (startPos.x + (direction.x * granadeSpeed * tempTime), startPos.y + (direction.y * granadeSpeed * tempTime) - (g * tempTime * tempTime / 2f));
			tempTime += 1f;
		}
		float landingSpot = tempGranadePos.x;

		if (Mathf.Abs (landingSpot - character.transform.position.x) < granadeThreshold) {
			goAway = true;
			if (landingSpot < character.transform.position.x) {
				goAwayTarget = new Vector3 (character.transform.position.x + (granadeThreshold - Mathf.Abs (landingSpot - character.transform.position.x)), character.transform.position.y, character.transform.position.z);
			} else {
				goAwayTarget = new Vector3 (character.transform.position.x - (granadeThreshold - Mathf.Abs (landingSpot - character.transform.position.x)), character.transform.position.y, character.transform.position.z);
			}
			goAwayDirection = goAwayTarget.normalized;
		}
	}
}
