using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidanceController : MonoBehaviour {

	public GameObject character;
	public float maxSpeed;
	public Vector3 targetPosition;
	public float distanceToTarget;
	public float avoidanceKoef;

	Vector2 charVelocity;
	RaycastHit2D wallHit;

	void Update ()
	{
		charVelocity = new Vector2 (targetPosition.x - character.transform.position.x, targetPosition.y - character.transform.position.y);

		wallHit = Physics2D.Raycast (character.transform.position, charVelocity, distanceToTarget);
		if (wallHit.collider != null) {

			targetPosition = new Vector3 (wallHit.point.x + wallHit.normal.x * avoidanceKoef, wallHit.point.y + wallHit.normal.y * avoidanceKoef, targetPosition.z);
			charVelocity = new Vector2 (targetPosition.x - character.transform.position.x, targetPosition.y - character.transform.position.y);

		}

		if (charVelocity.magnitude > 0.1f) {
			charVelocity.Normalize ();
			charVelocity *= maxSpeed;
			character.transform.position = new Vector3 (character.transform.position.x + charVelocity.x, character.transform.position.y + charVelocity.y, character.transform.position.z);
			character.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (charVelocity.y, charVelocity.x) * 57.2958f);
		}
	}
}
