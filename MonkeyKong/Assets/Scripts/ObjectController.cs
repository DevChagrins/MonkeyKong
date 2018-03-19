using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
	public float MoveSpeed = 5f;
	public float JumpSpeed = 5f;
	public float Gravity = 9.8f;
	public LayerMask CollisionLayer;

	Rigidbody2D mRigidBody = null;
	bool mMoving, mMovingLeft, mMovingRight, mJump;

	Vector3 mPosition;
	Vector2 mVelocity;
	Vector2 mDeltaVelocity;
	bool mGrounded;

	Vector2 mObjectSize;
	Vector2 mHalfObjectSize;

	// Use this for initialization
	void Start()
	{
		mRigidBody = GetComponent<Rigidbody2D>();

		mObjectSize = new Vector2();
		mObjectSize.y = GetComponent<SpriteRenderer>().bounds.size.y;
		mObjectSize.x = GetComponent<SpriteRenderer>().bounds.size.x;

		mHalfObjectSize = new Vector2();
		mHalfObjectSize.y = mObjectSize.y / 2f;
		mHalfObjectSize.x = mObjectSize.x / 2f;

		Fall();
	}

	// Update is called once per frame
	void Update()
	{
		UpdatePosition();
	}

	void UpdatePosition()
	{
		mPosition = transform.localPosition;
		Vector3 scale = transform.localScale;

		mVelocity.x = 0f;

		if (mMoving)
		{
			if (mMovingLeft)
			{
				mVelocity.x = -MoveSpeed;
				scale.x = -1;
			}

			if (mMovingRight)
			{
				mVelocity.x = MoveSpeed;
				scale.x = 1;
			}
		}

		mDeltaVelocity.x = mVelocity.x * Time.deltaTime;
		HandleWallCollision();

		mPosition.x += mDeltaVelocity.x;

		// Handle jumping and falling
		CheckTopBottomCollisions();

		if (mJump && mGrounded)
		{
			mVelocity.y += JumpSpeed;
			mGrounded = false;
		}

		mDeltaVelocity.y = mVelocity.y * Time.deltaTime;

		if (!mGrounded)
		{
			mPosition.y += mDeltaVelocity.y;

			mVelocity.y -= Gravity * Time.deltaTime;
		}

		// Update position
		transform.localPosition = mPosition;
		transform.localScale = scale;
	}

	void Fall()
	{
		mVelocity.y = 0;

		mGrounded = false;
	}

	void HandleWallCollision()
	{
		Vector2 midWalkPoint = new Vector2(mPosition.x + (mDeltaVelocity.x / 2f), mPosition.y);
		Vector2 boxSize = new Vector2(Mathf.Abs(mDeltaVelocity.x) + mObjectSize.x, mObjectSize.y);

		Vector2 direction = new Vector2(mDeltaVelocity.x > 0f ? 1f : -1f, 0f);

		// Array of objects that would be collided with
		RaycastHit2D[] collisionResults = Physics2D.BoxCastAll(mPosition, mObjectSize, 0f, direction, mDeltaVelocity.x, CollisionLayer);

		if (collisionResults.Length > 0)
		{
			for(int index = 0; index < collisionResults.Length; index++)
			{
				float yDistance = collisionResults[index].point.y - mPosition.y;
				float xSign = collisionResults[index].point.x < mPosition.x ? -1 : 1;

				// Does the object actually matter
				if ((Mathf.Abs(yDistance) < (mHalfObjectSize.y - 0.0001f)) && (xSign == direction.x))
				{
					// Quit moving and set yourself as close to the wall as possible

					mDeltaVelocity.x = mVelocity.x = 0f;
					mPosition.x = collisionResults[index].point.x - (mHalfObjectSize.x * direction.x);
					break;
				}
			}
		}
	}

	void HandleGroundCollisions()
	{
		Vector2 velocity;
		velocity.y = mVelocity.y < 0f ? mDeltaVelocity.y : -mObjectSize.y;

		Vector2 midFallPoint = new Vector2(mPosition.x, mPosition.y + (velocity.y / 2f));
		Vector2 boxSize = new Vector2(mObjectSize.x, Mathf.Abs(velocity.y) + mObjectSize.y);

		Collider2D collisions = Physics2D.OverlapBox(midFallPoint, boxSize, 0f, CollisionLayer);

		if (collisions)
		{
			mGrounded = true;
			if (mVelocity.y < 0f)
			{
				mVelocity.y = 0f;
				mPosition.y = collisions.bounds.ClosestPoint(mPosition).y + mHalfObjectSize.y;
			}
		}
		else
		{
			mGrounded = false;
		}
	}

	void CheckTopBottomCollisions()
	{
		if(mVelocity.y > 0)
		{
			// Check for ceiling
		}
		else
		{
			HandleGroundCollisions();
		}
	}
}