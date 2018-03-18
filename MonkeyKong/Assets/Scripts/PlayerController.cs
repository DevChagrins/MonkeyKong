using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float JumpSpeed = 5f;
    public float Gravity = 9.8f;
    public LayerMask CollisionLayer;

    Rigidbody2D mRigidBody = null;
    bool mWalking, mWalkingLeft, mWalkingRight, mJump;

    private float mWidth, mHeight;
    private Vector3 mPosition;
    private Vector2 mVelocity;
    private Vector2 mDeltaVelocity;
    private bool mGrounded;

    // Use this for initialization
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
        mWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        mHeight = GetComponent<SpriteRenderer>().bounds.size.y;

        Fall();
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

        mVelocity.x = 0f;

        if (mWalking)
        {
            if (mWalkingLeft)
            {
                mVelocity.x -= MoveSpeed;
                scale.x = -1;
            }

            if (mWalkingRight)
            {
                mVelocity.x += MoveSpeed;
                scale.x = 1;
            }
        }

        mDeltaVelocity.x = mVelocity.x * Time.deltaTime;
        mPosition.x += mVelocity.x * Time.deltaTime;

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

    void HandleGroundCollisions()
    {
        Vector2 velocity;
        velocity.y = mVelocity.y < 0f ? mDeltaVelocity.y : -mHeight;

        Vector2 midFallPoint = new Vector2(mPosition.x, mPosition.y + (velocity.y / 2f));
        Vector2 boxSize = new Vector2(mWidth, Mathf.Abs(velocity.y) + mHeight);

        Collider2D collisions = Physics2D.OverlapBox(midFallPoint, boxSize, 0f, CollisionLayer);

        if (collisions)
        {
            mGrounded = true;
            if (mVelocity.y < 0f)
            {
                mVelocity.y = 0f;
                mPosition.y = collisions.bounds.ClosestPoint(mPosition).y + mHeight/2f;
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


    // Move this to extend the debug class
    void DrawDebugBox(Vector2 _point, Vector2 _size)
    {
        float halfWidth = _size.x / 2f;
        float halfHeight = _size.y / 2f;
        Vector3 topLeft = new Vector3(_point.x - halfWidth, _point.y + halfHeight);
        Vector3 bottomLeft = new Vector3(_point.x - halfWidth, _point.y - halfHeight);
        Vector3 topRight = new Vector3(_point.x + halfWidth, _point.y + halfHeight);
        Vector3 bottomRight = new Vector3(_point.x + halfWidth, _point.y - halfHeight);

        Debug.DrawLine(topLeft, topRight, Color.green);
        Debug.DrawLine(topRight, bottomRight, Color.green);
        Debug.DrawLine(bottomRight, bottomLeft, Color.green);
        Debug.DrawLine(bottomLeft, topLeft, Color.green);
    }
}
