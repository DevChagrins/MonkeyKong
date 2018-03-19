using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySideWalkLoop : MonoBehaviour {

	public float moveSpeed = 5f;
	public bool doesMove = true;

	// ONLY TEMPORARILY A STRING.  should be an enum or something eventually.
	public string startingWalkDirection;

	float enemyHeight;
	float enemyWidth;
	string walkDirection; // THIS TOO SCREAMS

	// Use this for initialization
	void Start () {

		this.enemyHeight = GetComponent<BoxCollider2D> ().size.y;
		this.enemyWidth = GetComponent<BoxCollider2D> ().size.x;
		this.walkDirection = startingWalkDirection;
	}

	// Update is called once per frame
	void Update () {

		// Enemy is ALWAYS moving.  Either goes left or right.

		// Calculate new position in 'pos' variable
		// Starts from the current position
		Vector3 pos = transform.localPosition;

		if (this.walkDirection == "left") {
			pos = new Vector2 (pos.x - this.moveSpeed * Time.deltaTime, pos.y);
		} else {
			// DOIJFASOIDJFASODFASODJFASDF
		}
	}
}
