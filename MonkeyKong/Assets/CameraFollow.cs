using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Transform LeftBounds;
    public Transform RightBounds;

    public float SmoothDampTime = 0.15f;
    private Vector3 mSmoothDampVelocity = Vector3.zero;

    private float mCameraWidth, mCameraHeight, mLevelMinX, mLevelMaxX;

    // Use this for initialization
    void Start()
    {
        mCameraHeight = Camera.main.orthographicSize * 2f;
        mCameraWidth = mCameraHeight * Camera.main.aspect;

        float leftBoundsWidth = LeftBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2f;
        float rightBoundsWidth = RightBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2f;

        mLevelMinX = LeftBounds.position.x + leftBoundsWidth + (mCameraWidth / 2f);
        mLevelMaxX = RightBounds.position.x - rightBoundsWidth - (mCameraWidth / 2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Target)
        {
            float targetX = Mathf.Max(mLevelMinX, Mathf.Min(mLevelMaxX, Target.position.x));
            float x = Mathf.SmoothDamp(transform.position.x, targetX, ref mSmoothDampVelocity.x, SmoothDampTime);

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
