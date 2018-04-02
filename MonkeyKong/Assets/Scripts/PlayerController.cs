using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDebug;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float JumpSpeed = 5f;
    public float Gravity = 9.8f;
    public LayerMask CollisionLayer;

    bool mWalking, mWalkingLeft, mWalkingRight, mJump;

    Vector3 mPosition;
    Vector2 mVelocity;
    Vector2 mDeltaVelocity;
    bool mGrounded;

    Vector2 mPlayerSize;
    Vector2 mHalfPlayerSize;

    // Use this for initialization
    void Start()
    {
        mPlayerSize = new Vector2();
        mPlayerSize.y = GetComponent<BoxCollider2D>().bounds.size.y;
        mPlayerSize.x = GetComponent<BoxCollider2D>().bounds.size.x;

        mHalfPlayerSize = new Vector2();
        mHalfPlayerSize.y = mPlayerSize.y / 2f;
        mHalfPlayerSize.x = mPlayerSize.x / 2f;
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
        mPosition = transform.localPosition;
        Vector3 scale = transform.localScale;

        // Reset x movement so it'll zero out if we don't need it
        mVelocity.x = 0f;

        // Handle walking left/right
        if (mWalking)
        {
            if (mWalkingLeft)
            {
                mVelocity.x = -MoveSpeed;
				scale.x = -1;
            }

            if (mWalkingRight)
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

    void HandleWallCollision()
    {
        Vector2 direction = new Vector2(mDeltaVelocity.x > 0f ? 1f : -1f, 0f);
        RaycastHit2D[] collisionResults = new RaycastHit2D[5];
        int collisionCount = Physics2D.BoxCastNonAlloc(mPosition, mPlayerSize, 0f, direction, collisionResults, Mathf.Abs(mDeltaVelocity.x), CollisionLayer);

        if (collisionCount > 0)
        {
            for(int index = 0; index < collisionCount; index++)
            {
                float yDistance = collisionResults[index].point.y - mPosition.y;
                float xSign = collisionResults[index].point.x < mPosition.x ? -1 : 1;
                if ((Mathf.Abs(yDistance) < (mHalfPlayerSize.y - 0.0001f)) && (xSign == direction.x))
                {
                    mDeltaVelocity.x = mVelocity.x = 0f;
                    mPosition.x = collisionResults[index].point.x - (mHalfPlayerSize.x * direction.x);
                    break;
                }
            }
        }
    }

    void HandleGroundCollisions()
    {
        float ySpeed = mVelocity.y < 0f ? mDeltaVelocity.y : -0.02f;
        Vector2 direction = new Vector2(0f, mDeltaVelocity.y > 0f ? 1f : -1f);

        RaycastHit2D[] collisionResults = new RaycastHit2D[5];
        int collisionCount = Physics2D.BoxCastNonAlloc(mPosition, mPlayerSize, 0f, direction, collisionResults, Mathf.Abs(ySpeed), CollisionLayer);

        bool collisionHappened = false;
        if (collisionCount > 0)
        {
            RaycastHit2D collision;
            mGrounded = true;
            for (int index = 0; index < collisionCount; index++)
            {
                collision = collisionResults[index];
                float xDistance = collision.point.x - mPosition.x;
                float ySign = collision.point.x < mPosition.x ? -1 : 1;
                if (Mathf.Abs(xDistance) < (mHalfPlayerSize.x - 0.0001f))
                {
                    collisionHappened = mGrounded = true;
                    mDeltaVelocity.y = mVelocity.y = 0f;
                    mPosition.y = collision.collider.bounds.max.y + mHalfPlayerSize.y;
                    break;
                }
            }

            if((false == collisionHappened) && (mVelocity.y < 0f))
            {
                mGrounded = false;
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
