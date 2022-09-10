using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour {

    const int MIN_RAY_COUNT = 2;
    const float SKIN_WIDTH = .015f;

    [SerializeField] int horizontalRays = MIN_RAY_COUNT;
    [SerializeField] int verticalRays = MIN_RAY_COUNT;
    [SerializeField] LayerMask collisionMask;

    private new BoxCollider2D collider;
    private RaycastOrigins raycastOrigins;
    private int horizontalRayCount;
    private int verticalRayCount;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private bool isGrounded;
    private CollisionInfo collisionInfo;

    public CollisionInfo Collisions => collisionInfo;

    void Start() {
        collider = GetComponent<BoxCollider2D>();

        collisionInfo.reset();
    }

    public void move(Vector2 positionDelta) {
        updateRaycastData();

        collisionInfo.reset();
        checkVerticalCollisions(ref positionDelta);

        transform.Translate(positionDelta);
    }

    private void updateRaycastData() {
        Bounds bounds = collider.bounds;
        bounds.Expand(SKIN_WIDTH * -2f);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

        horizontalRayCount = Mathf.Max(MIN_RAY_COUNT, horizontalRays);
        verticalRayCount = Mathf.Max(MIN_RAY_COUNT, verticalRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    private void checkVerticalCollisions(ref Vector2 positionDelta) {
        float yDirection = Mathf.Sign(positionDelta.y);
        float rayLength = Mathf.Abs(positionDelta.y) + SKIN_WIDTH;

        Vector2 rayStartingPosition = yDirection < 0 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
        Vector2 rayDirection = Vector2.up * yDirection;
        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayPosition = rayStartingPosition + Vector2.right * verticalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, rayDirection, rayLength, collisionMask);
            if (hit.collider != null) {
                positionDelta.y = yDirection * (hit.distance - SKIN_WIDTH);
                rayLength = hit.distance;

                collisionInfo.below = yDirection == -1;
                collisionInfo.above = !collisionInfo.below;
            }
        }
    }

    private struct RaycastOrigins {
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
        public Vector2 topLeft;
        public Vector2 topRight;
    }
}

public struct CollisionInfo {
    public bool above;
    public bool below;
    public bool left;
    public bool right;

    public void reset() {
        above = below = left = right = false;
    }
}
