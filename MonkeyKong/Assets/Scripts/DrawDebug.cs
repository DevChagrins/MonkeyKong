using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDebug
{
    public static class DrawDebug
    {
        public static void DrawBox(Vector2 _point, Vector2 _size)
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
}
