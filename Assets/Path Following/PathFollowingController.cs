using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingController : MonoBehaviour {

	public GameObject pathPoint;
	public GameObject characterPrefab;
	public GameObject pathPointHolder;
	public float pointDrawingTime;
	public float characterMaxSpeed;

	private bool pathDrawingState;
	private bool firstTargetFound;
	private bool pathFollowingState;
	private bool goingToTarget;
	private float pointDrawingTimer;
	private int prevPointIndex;
	private Vector3 nextTargetPosition;
	private GameObject character;

	void Start()
	{
		pathDrawingState = true;
		pathFollowingState = false;
		firstTargetFound = false;
		goingToTarget = false;
		pointDrawingTimer = pointDrawingTime;
	}

	void Reset()
	{
		for (int i = 0; i < pathPointHolder.transform.childCount; i++) {
			Destroy (pathPointHolder.transform.GetChild (i).gameObject);
		}
		Destroy (character);
		Start ();
	}

	void Update ()
	{
		if (Input.GetMouseButton (0) && pathDrawingState == true) {
			pointDrawingTimer -= Time.deltaTime;
			if (pointDrawingTimer <= 0f) {
				Vector3 pointPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Instantiate (pathPoint, new Vector3 (pointPosition.x, pointPosition.y, 0f), Quaternion.identity, pathPointHolder.transform);
				pointDrawingTimer = pointDrawingTime;
			}
		}

		if (Input.GetMouseButtonUp (0) && pathDrawingState == true) {
			pathDrawingState = false;
			pathFollowingState = true;
			character = Instantiate (characterPrefab, new Vector3 (-7f, 0f, 0f), Quaternion.identity);
		}

		if (pathFollowingState == true) {
			Debug.Log ("1");
			if (goingToTarget == false) {
				Debug.Log ("2");
				if (firstTargetFound == false) {
					Debug.Log ("3");
					nextTargetPosition = FindFirstTarget ();
					firstTargetFound = true;
					goingToTarget = true;
				} else {
					Debug.Log ("4");
					bool next;
					nextTargetPosition = FindNextTarget (out next);
					if (next == false) {
						Reset ();
					} else {
						goingToTarget = true;
					}
				}
			} else {
				Debug.Log ("5");
				Vector2 characterVelocity = new Vector2 (nextTargetPosition.x - character.transform.position.x, nextTargetPosition.y - character.transform.position.y);
				if (characterVelocity.magnitude < 0.1f) {
					Debug.Log ("6");
					goingToTarget = false;
				} else {
					Debug.Log ("7");
					characterVelocity.Normalize ();
					characterVelocity *= characterMaxSpeed;
					character.transform.position = new Vector3 (character.transform.position.x + characterVelocity.x, character.transform.position.y + characterVelocity.y, 0f);
					character.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2 (characterVelocity.y, characterVelocity.x) * 57.2958f);
				}
			}
		}
	}

	Vector3 FindFirstTarget()
	{
		Vector3 firstTargetPosition;
		float minDistance = Vector3.Distance (pathPointHolder.transform.GetChild (0).position, character.transform.position);
		int minDistanceIndex = 0;
		for (int i = 1; i < pathPointHolder.transform.childCount; i++) {
			if (minDistance > Vector3.Distance (pathPointHolder.transform.GetChild (i).position, character.transform.position)) {
				minDistance = Vector3.Distance (pathPointHolder.transform.GetChild (i).position, character.transform.position);
				minDistanceIndex = i;
			}
		}
		prevPointIndex = minDistanceIndex;
		firstTargetPosition = pathPointHolder.transform.GetChild (prevPointIndex).position;
		return firstTargetPosition;
	}

	Vector3 FindNextTarget(out bool foundNext)
	{
		Vector3 targetPosition = Vector3.zero;
		pathPointHolder.transform.GetChild (prevPointIndex).gameObject.GetComponent<SpriteRenderer> ().color = new Color (0f, 1f, 0f);
		prevPointIndex++;
		if (pathPointHolder.transform.childCount > prevPointIndex) {
			targetPosition = pathPointHolder.transform.GetChild (prevPointIndex).position;
			foundNext = true;
		} else {
			foundNext = false;
		}
		return targetPosition;
	}
}
