using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoidanceController : MonoBehaviour {

	public GameObject target;
	public GameObject character;
	public float objectRadius;

	Vector2 relativePos;
	Vector2 relativeVel;
	float relativeSpeed;
	float timeToCollision;

	void Update ()
	{
		relativePos = new Vector2 (target.transform.position.x - character.transform.position.x, target.transform.position.y - character.transform.position.y);
		Debug.Log ("rp: " + relativePos);
		relativeVel = new Vector2 (target.GetComponent<Character> ().CharVelocity ().x - character.GetComponent<Character> ().CharVelocity ().x, target.GetComponent<Character> ().CharVelocity ().y - character.GetComponent<Character> ().CharVelocity ().y);
		Debug.Log ("rv: " + relativeVel);
		relativeSpeed = relativeVel.magnitude;
		Debug.Log ("rs: " + relativeSpeed);
		timeToCollision = (relativePos.magnitude * relativeVel.magnitude * Mathf.Cos (Mathf.Atan2 (relativePos.x - relativeVel.x, relativePos.y - relativeVel.y) * 57.2958f)) / relativeSpeed / relativeSpeed;
		Debug.Log ("ttc: " + timeToCollision);
		if ((relativePos.magnitude + relativeSpeed * timeToCollision) > objectRadius * 2f) {
			Debug.Log ("No collision");
		} else {
			Debug.Log ("Collision");
			character.GetComponent<Character> ().StopTheCharacter ();
		}
	}
}
