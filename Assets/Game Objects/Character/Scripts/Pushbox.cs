using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushbox : MonoBehaviour
{
    const float skinWidth = 0.015f;

    BoxCollider2D boxCollider2D;
    RaycastOrigins raycastOrigins;
    public LayerMask collisionMask;
    public LayerMask pushBoxMask;

    public float rayCount;
    float raySpacing;

    public float pushModifier;
    public float maxPush;

    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }


    public Vector2 PushBox(bool collisionCheck)
    {
        LayerMask mask = collisionMask;
        if (!collisionCheck) { mask = pushBoxMask; }

        UpdateRaycastOrigins();
        CalculateRaySpacing();

        Vector2 force = Vector2.zero;

        float rayLength = boxCollider2D.size.x * transform.lossyScale.x - (skinWidth * 4);
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.up * (raySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, mask);

            if (hit)
            {
                rayLength = hit.distance;
                float distanceInCollision = boxCollider2D.size.x - hit.distance;
                force.x -= distanceInCollision * pushModifier;
            }
        }

        rayLength = boxCollider2D.size.x * transform.lossyScale.x - (skinWidth * 4);
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (raySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayLength, mask);

            Debug.DrawRay(rayOrigin, Vector2.left * rayLength, Color.red);

            if (hit)
            {
                float distanceInCollision = rayLength - hit.distance;
                rayLength = hit.distance;
                force.x += distanceInCollision * pushModifier;
            }
        }

        force.x = Mathf.Min(force.x, maxPush);
        return force;
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -4);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2);

        rayCount = Mathf.Clamp(rayCount, 2, int.MaxValue);
        raySpacing = bounds.size.y / (rayCount - 1);
    }

}
