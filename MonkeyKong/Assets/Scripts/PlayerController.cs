using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonkeyKongExtensions.Debug;
using MonkeyKongExtensions.Math;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        STILL = 0,
        WALKING = 1,
        FALLING = 2,
        WALL_JUMPING = 4,
        WALL_SLIDING = 8,
    };

    public float MoveSpeed = 5f;
    public float JumpSpeed = 5f;
    public float WallJumpSpeed = 5f;
    public float HrJumpSpeed = 5f;
    public float JumpTime = 0.5f;
    public float WallJumpTime = 0.5f;
    public float Gravity = 9.8f;
    public float WallSlideSpeed = 3f;
    public LayerMask CollisionLayer;

    bool mWalking, mWalkingLeft, mWalkingRight, mJump, mWallCollision;

    int mWallDirection = 0;
    float mJumpTimer = -0f;
    float mWallJumpTimer = -0f;

    Vector3 mPosition;
    Vector2 mVelocity;
    Vector2 mDeltaVelocity;
    bool mGrounded;

    Vector2 mPlayerSize;
    Vector2 mHalfPlayerSize;

    Animator mAnimator;

    PlayerState mPreviousState = PlayerState.STILL;
    PlayerState mPlayerState = PlayerState.STILL;

    // Use this for initialization
    void Start()
    {
        mPlayerSize = new Vector2();
        mPlayerSize.y = GetComponent<BoxCollider2D>().bounds.size.y;
        mPlayerSize.x = GetComponent<BoxCollider2D>().bounds.size.x;

        mHalfPlayerSize = new Vector2();
        mHalfPlayerSize.y = mPlayerSize.y / 2f;
        mHalfPlayerSize.x = mPlayerSize.x / 2f;

        mAnimator = GetComponent<Animator>();
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

        // Handle walking left/right
        if (mWalking && (mPlayerState != PlayerState.WALL_JUMPING))
        {
            if (mWalkingLeft)
            {
                mVelocity.x += -MoveSpeed;
				scale.x = -1;
            }

            if (mWalkingRight)
            {
                mVelocity.x += MoveSpeed;
                scale.x = 1;
            }
        }
        else if(mGrounded)
        {
            mVelocity.x = 0f;
        }

        mDeltaVelocity.x = mVelocity.x * Time.deltaTime;
        HandleWallCollision();

        // Handle jumping and falling
        CheckTopBottomCollisions();

        if ((mPlayerState == PlayerState.WALL_SLIDING) && (mPreviousState != PlayerState.WALL_SLIDING))
        {
            mVelocity.y = 0f;
        }

        if (mJump && mGrounded)
        {
            mVelocity.y += JumpSpeed;
            mGrounded = false;
            mJumpTimer = JumpTime;
        }
        else if(mJump && (mPlayerState == PlayerState.WALL_SLIDING))
        {
            mVelocity.y += WallJumpSpeed;
            mVelocity.x += HrJumpSpeed * (mWallDirection * -1);
            mWallCollision = false;
            mPlayerState = PlayerState.WALL_JUMPING;
            mWallJumpTimer = WallJumpTime;
        }

        // Cap movement
        mVelocity.x = Mathf.Max(-MoveSpeed, Mathf.Min(MoveSpeed, mVelocity.x));
        // Handle this after wall jumping...because i'm too lazy to implement proper systems for a small game like this. :P
        mDeltaVelocity.x = mVelocity.x * Time.deltaTime;
        mPosition.x += mDeltaVelocity.x;

        /*if(mPlayerState == PlayerState.WALL_SLIDING)
        {
            mVelocity.y += WallJumpSpeed;
        }*/

        mDeltaVelocity.y = mVelocity.y * Time.deltaTime;

        if (!mGrounded)
        {
            mPosition.y += mDeltaVelocity.y;

            if(mPlayerState != PlayerState.WALL_SLIDING)
            {
                mVelocity.y -= Gravity * Time.deltaTime;
            }
            else
            {
                mVelocity.y = (-WallSlideSpeed) * Time.deltaTime;
            }
        }

        if(mJumpTimer > 0f)
        {
            mJumpTimer -= Time.deltaTime;
        }

        if(mWallJumpTimer > 0f)
        {
            mWallJumpTimer -= Time.deltaTime;
        }

        if(mWallJumpTimer <= 0f && (mPlayerState == PlayerState.WALL_JUMPING))
        {
            mPlayerState = PlayerState.FALLING;
        }

        // Update position
        transform.localPosition = mPosition;
        transform.localScale = scale;

        mPreviousState = mPlayerState;
        if (mPlayerState != PlayerState.WALL_JUMPING)
        {
            if (mGrounded)
            {
                if(mWalking)
                {
                    mPlayerState = PlayerState.WALKING;
                }
                else
                {
                    mPlayerState = PlayerState.STILL;
                }
            }
            else
            {
                if (mWallCollision && mJumpTimer <= 0f)
                {
                    mPlayerState = PlayerState.WALL_SLIDING;
                }
                else
                {
                    mPlayerState = PlayerState.FALLING;
                }
            }
        }

        Debug.Log("Player State: " + mPlayerState);

        mAnimator.SetInteger("State", (int)mPlayerState);
    }

    void HandleWallCollision()
    {
        Vector2 direction = new Vector2(mDeltaVelocity.x > 0f ? 1f : -1f, 0f);
        RaycastHit2D[] collisionResults = new RaycastHit2D[5];
        int collisionCount = Physics2D.BoxCastNonAlloc(mPosition, mPlayerSize, 0f, direction, collisionResults, Mathf.Abs(mDeltaVelocity.x), CollisionLayer);

        bool collisionHappened = false;
        if (collisionCount > 0)
        {
            for(int index = 0; index < collisionCount; index++)
            {
                float yDistance = collisionResults[index].point.y - mPosition.y;
                float xSign = collisionResults[index].point.x < mPosition.x ? -1f : 1f;
                if ((Mathf.Abs(yDistance) < (mHalfPlayerSize.y - 0.0001f)) && (xSign == direction.x))
                {
                    mWallDirection = (int)xSign;

                    collisionHappened = mWallCollision = true;
                    mDeltaVelocity.x = mVelocity.x = 0f;
                    mPosition.x = MathFloat.Round(collisionResults[index].point.x - (mHalfPlayerSize.x * direction.x), 2);
                    break;
                }
            }
        }

        if(!collisionHappened)
        {
            mWallCollision = false;
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
                if (Mathf.Abs(xDistance) < (mHalfPlayerSize.x - 0.0001f))
                {
                    collisionHappened = mGrounded = true;
                    mDeltaVelocity.y = mVelocity.y = 0f;
                    mPosition.y = MathFloat.Round(collision.collider.bounds.max.y + mHalfPlayerSize.y, 2);
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
