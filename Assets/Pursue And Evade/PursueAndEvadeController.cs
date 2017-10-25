using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PursueAndEvadeController : MonoBehaviour {

	public GameObject character;
	public GameObject friend;
	public float characterMaxSpeed;
	public float friendMaxSpeed;
	public bool pursue;

	private Vector2 characterMoveVelocity;

	void Update ()
	{
		characterMoveVelocity = Vector2.zero;

		if (Input.GetMouseButton (0)) {
			Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			characterMoveVelocity = new Vector2 (mousePositionInWorld.x - character.transform.position.x, mousePositionInWorld.y - character.transform.position.y);
			if (characterMoveVelocity.magnitude > 0.1f) {
				characterMoveVelocity.Normalize ();
				characterMoveVelocity *= characterMaxSpeed;
				character.transform.position = new Vector3 (character.transform.position.x + characterMoveVelocity.x, character.transform.position.y + characterMoveVelocity.y, 0f);
				character.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (characterMoveVelocity.y, characterMoveVelocity.x) * 57.2958f);
			}
		}

		Vector2 friendMoveVelocity;
		if (pursue == true) {
			friendMoveVelocity = new Vector2 (character.transform.position.x - friend.transform.position.x, character.transform.position.y - friend.transform.position.y);
		} else {
			friendMoveVelocity = new Vector2 (friend.transform.position.x - character.transform.position.x, friend.transform.position.y - character.transform.position.y);
		}
		friendMoveVelocity = new Vector2(friendMoveVelocity.x + characterMoveVelocity.x * friendMoveVelocity.magnitude, friendMoveVelocity.y + characterMoveVelocity.y * friendMoveVelocity.magnitude);
		if (friendMoveVelocity.magnitude > 0.1f) {
			friendMoveVelocity.Normalize ();
			friendMoveVelocity *= friendMaxSpeed;
			friend.transform.position = new Vector3 (friend.transform.position.x + friendMoveVelocity.x, friend.transform.position.y + friendMoveVelocity.y, 0f);
			friend.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (friendMoveVelocity.y, friendMoveVelocity.x) * 57.2958f);
		}
	}

	public void ChangeToPursueOrEvade(Text pursueOrEvadeText)
	{
		if (pursue == true) {
			pursue = false;
			pursueOrEvadeText.text = "Pursue";
			friend.GetComponent<SpriteRenderer>().color = new Color (1f, 0f, 0f);
		} else {
			pursue = true;
			pursueOrEvadeText.text = "Evade";
			friend.GetComponent<SpriteRenderer> ().color = new Color (0f, 1f, 0f);
		}
	}
}
