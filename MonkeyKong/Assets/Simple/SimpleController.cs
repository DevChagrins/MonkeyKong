using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float JumpSpeed = 5f;

    Rigidbody2D mRigidBody = null;

    // Use this for initialization
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float xMovement = 0f;

        if(Input.GetKey(KeyCode.A))
        {
            xMovement += -MoveSpeed;
        }

        if(Input.GetKey(KeyCode.D))
        {
            xMovement += MoveSpeed;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            mRigidBody.AddForce(new Vector2(0f, JumpSpeed), ForceMode2D.Impulse);
        }

        mRigidBody.velocity = new Vector2(xMovement, mRigidBody.velocity.y);

    }
}
