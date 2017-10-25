using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingController : MonoBehaviour {

	public GameObject character;
	public float maxSpeed;
	public float maxRotation;

	private float timer = 0f;
	private bool clockwise;
	private float rotation;

	void Update ()
	{
		timer -= Time.deltaTime;

		if (timer <= 0f) {
			timer = Random.Range (0f, 3f);

			if (Random.Range (0, 2) == 0)
				clockwise = true;
			else
				clockwise = false;
			
			rotation = Random.Range (0f, maxRotation);
			if (clockwise)
				rotation = -rotation;
		} 

		Vector3 characterVelocity = new Vector2(Mathf.Cos(character.transform.eulerAngles.z / 57.2958f), Mathf.Sin(character.transform.eulerAngles.z / 57.2958f));
		characterVelocity.Normalize ();
		characterVelocity *= maxSpeed;
		character.transform.position = new Vector3 (character.transform.position.x + characterVelocity.x, character.transform.position.y + characterVelocity.y, character.transform.position.z);
		character.transform.eulerAngles = new Vector3 (0f, 0f, character.transform.eulerAngles.z + rotation);
	}
}
