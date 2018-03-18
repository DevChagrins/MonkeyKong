using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float JumpSpeed = 5f;
	public LayerMask CollisionLayer;

    Rigidbody2D mRigidBody = null;
    bool mWalking, mWalkingLeft, mWalkingRight, mJump;

	float playerHeight;
	float playerWidth;

    // Use this for initialization
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();

		float playerHeight = GetComponent<SpriteRenderer> ().bounds.size.y;
		float playerWidth = GetComponent<SpriteRenderer> ().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        UpdatePlayerPosition();
    }

    void CheckInput()
    {
        bool inputLeft = Input.GetKey(KeyCode.A);
        bool inputRight = Input.GetKey(KeyCode.D);
        bool inputJump = Input.GetKey(KeyCode.Space);

        mWalking = inputLeft || inputRight;
        mWalkingLeft = inputLeft && !inputRight;
        mWalkingRight = !inputLeft && inputRight;
        mJump = inputJump;
    }

    void UpdatePlayerPosition()
    {
        Vector3 pos = transform.localPosition;
        Vector3 scale = transform.localScale;

        if (mWalking)
        {
            if (mWalkingLeft)
            {
				
				Vector2 newPoint = new Vector2 (pos.x - MoveSpeed * Time.deltaTime, pos.y);

				float width = newPoint.x - pos.x + playerWidth;
				float height = playerHeight;

				Vector2 newSize = new Vector2 (width, height);

				float angle = 0;

				int layerMask = CollisionLayer.value;

				if (!Physics2D.OverlapBox(newPoint, newSize, angle, layerMask)) {
					
					pos.x = newPoint.x;
					scale.x = -1;
				}
            }

            if (mWalkingRight)
            {
                
				Vector2 newPoint = new Vector2 (pos.x + MoveSpeed * Time.deltaTime, pos.y);

				float width = newPoint.x - pos.x + playerWidth;
				float height = playerHeight;

				Vector2 newSize = new Vector2 (width, height);

				float angle = 0;

				int layerMask = CollisionLayer.value;

				if (!Physics2D.OverlapBox(newPoint, newSize, angle, layerMask)) {

					pos.x = newPoint.x;
					scale.x = 1;
				}
            }
        }

        transform.localPosition = pos;
        transform.localScale = scale;
    }
}
