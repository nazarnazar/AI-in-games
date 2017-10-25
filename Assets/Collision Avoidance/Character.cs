using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	public Vector2 goThere;
	public float maxSpeed;

	Vector2 charVeclocity;
	bool stoped = false;

	void Update ()
	{
		if (stoped == false) {
			charVeclocity = new Vector2 (goThere.x - gameObject.transform.position.x, goThere.y - gameObject.transform.position.y);
			if (charVeclocity.magnitude > 0.1f) {
				charVeclocity.Normalize ();
				charVeclocity *= maxSpeed;
				gameObject.transform.position = new Vector3 (gameObject.transform.position.x + charVeclocity.x, gameObject.transform.position.y + charVeclocity.y, 0f);
				gameObject.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (charVeclocity.y, charVeclocity.x) * 57.2958f);
			}
		}
	}

	public Vector2 CharVelocity()
	{
		return charVeclocity;
	}

	public void StopTheCharacter()
	{
		stoped = true;
	}
}
