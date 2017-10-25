using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeekAndFleeController : MonoBehaviour {

	public GameObject character;
	public float maxSpeed;
	public bool seek;

	void Update ()
	{
		if (Input.GetMouseButton (0)) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 moveVelocity;
			if (seek)
				moveVelocity = new Vector2 (mousePosition.x - character.transform.position.x, mousePosition.y - character.transform.position.y);
			else 
				moveVelocity = new Vector2 (character.transform.position.x - mousePosition.x, character.transform.position.y - mousePosition.y);
			if (moveVelocity.magnitude > 0.1f) {
				moveVelocity.Normalize ();
				moveVelocity *= maxSpeed;
				FaceTheDirection (moveVelocity);
				character.transform.position = new Vector3 (character.transform.position.x + moveVelocity.x, character.transform.position.y + moveVelocity.y, 0f);
			}
		}
	}

	void FaceTheDirection(Vector2 desiredVelocity)
	{
		if (desiredVelocity.magnitude > 0) {
			character.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (desiredVelocity.y, desiredVelocity.x) * 57.2958f);
		}
	}

	public void ChangeToSeekOrFlee(Text seekOrFleeText)
	{
		if (seek) {
			seek = false;
			seekOrFleeText.text = "Seek";
		} else {
			seek = true;
			seekOrFleeText.text = "Flee";
		}
	}
}
