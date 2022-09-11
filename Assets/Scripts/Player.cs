using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float secondsToJumpApex = 0.5f;
    [SerializeField] float fallingTerminalVelocity = 5f;
    [SerializeField] float horizontalVelocity = 1f;

    private Controller2D controller;

    private float jumpInitialVelocity;
    private Vector2 gravity;
    private Vector2 fallingGravity;

    private Vector2 oldVelocity;
    private Vector2 velocity;

    void Start() {
        controller = GetComponent<Controller2D>();

        setupPhysicsValues();
    }

    void Update() {
        if (controller.Collisions.above || controller.Collisions.below) {
            velocity.y = 0f;
        }

        if (controller.Collisions.below && Input.GetKeyDown(KeyCode.Space)) {
            velocity.y = jumpInitialVelocity;
        }

        Vector2 selectedGravity = gravity;
        if (velocity.y < 0 || (velocity.y > 0f && !Input.GetKey(KeyCode.Space))) {
            selectedGravity = fallingGravity;
        }

        oldVelocity = velocity;
        velocity += selectedGravity * Time.deltaTime;
        if (velocity.y < 0) {
            velocity.y = Mathf.Max(velocity.y, -fallingTerminalVelocity);
        }

        int xInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        velocity.x = xInput * horizontalVelocity;

        Vector2 positionDelta = (oldVelocity + velocity) * 0.5f * Time.deltaTime;
        controller.move(positionDelta);
    }

    private void setupPhysicsValues() {
        jumpInitialVelocity = 2 * jumpHeight / secondsToJumpApex;
        gravity.y = -2 * jumpHeight / (secondsToJumpApex * secondsToJumpApex);
        fallingGravity = 2f * gravity;
    }
}
