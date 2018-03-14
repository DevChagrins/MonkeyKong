using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSoSimpleController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float JumpSpeed = 5f;

    Rigidbody2D mRigidBody = null;
    bool mWalking, mWalkingLeft, mWalkingRight, mJump;

    // Use this for initialization
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        UpdatePlayer();
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

    void UpdatePlayer()
    {
        Vector3 pos = transform.localPosition;
        Vector3 scale = transform.localScale;

        if (mWalking)
        {
            if (mWalkingLeft)
            {
                pos.x -= MoveSpeed * Time.deltaTime;
                scale.x = -1;
            }

            if (mWalkingRight)
            {
                pos.x += MoveSpeed * Time.deltaTime;
                scale.x = 1;
            }
        }

        transform.localPosition = pos;
        transform.localScale = scale;
    }
}
