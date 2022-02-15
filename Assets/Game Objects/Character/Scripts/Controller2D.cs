using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour
{

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    Rigidbody2D rb;
    BoxCollider2D boxCollider2D;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    public LayerMask mainCollisionMask;
    public LayerMask platformCollisionMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        CalcualteRaySpacing();
    }

    public void Move(Vector2 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        Vector2 distanceToMove = velocity * Time.fixedDeltaTime;
        distanceToMove = HorizontalCollisons(distanceToMove);
        distanceToMove = VerticalCollisons(distanceToMove);

        rb.transform.Translate(distanceToMove, Space.World);
    }

    Vector2 HorizontalCollisons(Vector2 distanceToMove)
    {
        // Check collision in both directions.
        // If there is a collision in one direction, push character so there is no longer a collision.

        float directionX = Mathf.Sign(distanceToMove.x);
        float rayLength = Mathf.Abs(distanceToMove.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, mainCollisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                distanceToMove.x = (hit.distance - skinWidth) * directionX;

                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }

        return distanceToMove;
    }

    Vector2 VerticalCollisons(Vector2 distanceToMove)
    {
        float directionY = (distanceToMove.y > 0) ? 1 : -1;
        float rayLength = Mathf.Abs(distanceToMove.y) + skinWidth;

        LayerMask whichMask = mainCollisionMask;
        if (directionY == -1) { whichMask = platformCollisionMask; }

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + distanceToMove.x * Time.fixedDeltaTime);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, whichMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.blue);

            if (hit)
            {                
                distanceToMove.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        return distanceToMove;
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalcualteRaySpacing()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }

}
