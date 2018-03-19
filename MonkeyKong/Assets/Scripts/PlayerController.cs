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

    Vector3 mPosition;
    Vector2 mVelocity;
    Vector2 mDeltaVelocity;
    bool mGrounded;

    Vector2 mPlayerSize;
    Vector2 mHalfPlayerSize;

    // Use this for initialization
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();

        mPlayerSize = new Vector2();
        mPlayerSize.y = GetComponent<SpriteRenderer>().bounds.size.y;
        mPlayerSize.x = GetComponent<SpriteRenderer>().bounds.size.x;

        mHalfPlayerSize = new Vector2();
        mHalfPlayerSize.y = mPlayerSize.y / 2f;
        mHalfPlayerSize.x = mPlayerSize.x / 2f;

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
                mVelocity.x = -MoveSpeed;
				scale.x = -1;
            }

            if (mWalkingRight)
            {
                mVelocity.x = MoveSpeed;
                scale.x = 1;

                /*Vector2 newPoint = new Vector2(mPosition.x + MoveSpeed * Time.deltaTime, mPosition.y);

                float width = newPoint.x - mPosition.x + mPlayerWidth;
                float height = mPlayerHeight;

                Vector2 newSize = new Vector2(width, height);

                float angle = 0;

                int layerMask = CollisionLayer.value;

                if (!Physics2D.OverlapBox(newPoint, newSize, angle, layerMask))
                {

                    mPosition.x = newPoint.x;
                    scale.x = 1;
                }*/
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
        Vector2 boxSize = new Vector2(Mathf.Abs(mDeltaVelocity.x) + mPlayerSize.x, mPlayerSize.y);

        DrawDebugBox(midWalkPoint, boxSize);

        //Collider2D collisions = Physics2D.OverlapBox(midWalkPoint, boxSize, 0f, CollisionLayer);

        Vector2 direction = new Vector2(mDeltaVelocity.x > 0f ? 1f : -1f, 0f);
        //RaycastHit2D[] collisionResults = new RaycastHit2D[5];
        //int collisionCount = Physics2D.BoxCastNonAlloc(mPosition, mPlayerSize, 0f, direction, collisionResults, mVelocity.x, CollisionLayer);

        RaycastHit2D[] collisionResults = Physics2D.BoxCastAll(mPosition, mPlayerSize, 0f, direction, mDeltaVelocity.x, CollisionLayer);
        if (collisionResults.Length > 0)
        {
            Debug.Log(collisionResults.Length);
            for(int index = 0; index < collisionResults.Length; index++)
            {
                float yDistance = collisionResults[index].point.y - mPosition.y;
                float xSign = collisionResults[index].point.x < mPosition.x ? -1 : 1;
                if ((Mathf.Abs(yDistance) < (mHalfPlayerSize.y - 0.0001f)) && (xSign == direction.x))
                {
                    Debug.Log("WALL COLLISIONS!");
                    mDeltaVelocity.x = mVelocity.x = 0f;
                    mPosition.x = collisionResults[index].point.x - (mHalfPlayerSize.x * direction.x);
                    break;
                }
            }
            /*if (mVelocity.x != 0f)
            {
                Debug.Log("Size: " + mPlayerSize.x + ", " + mPlayerSize.y);
                Debug.Log("Point: " + collisions.point.x + ", " + collisions.point.y);
                //Debug.Log(collisions.point);
                mVelocity.x = 0f;
                mPosition.x = collisions.point.x + (mHalfPlayerSize.x * direction.x);
                //mPosition.x = collisions.bounds.ClosestPoint(mPosition).x + (mPlayerWidth / 2f);
                //Debug.Log(collisions.bounds.ClosestPoint(mPosition).y);
            }*/
        }
    }

    void HandleGroundCollisions()
    {
        Vector2 velocity;
        velocity.y = mVelocity.y < 0f ? mDeltaVelocity.y : -mPlayerSize.y;

        Vector2 midFallPoint = new Vector2(mPosition.x, mPosition.y + (velocity.y / 2f));
        Vector2 boxSize = new Vector2(mPlayerSize.x, Mathf.Abs(velocity.y) + mPlayerSize.y);

        Collider2D collisions = Physics2D.OverlapBox(midFallPoint, boxSize, 0f, CollisionLayer);

        if (collisions)
        {
            mGrounded = true;
            if (mVelocity.y < 0f)
            {
                mVelocity.y = 0f;
                mPosition.y = collisions.bounds.ClosestPoint(mPosition).y + mHalfPlayerSize.y;
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
