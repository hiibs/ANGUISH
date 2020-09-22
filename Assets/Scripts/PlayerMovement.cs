using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public UnityEvent Crouch;
    public UnityEvent Stand;

    // Movement vars
    private float accel = 50f;
    private float slideAccel = 10f;
    private float runSpeed = 6f;
    private float crouchSpeed = 3f;
    private float gravity = 16f;
    private float friction = 6f;
    private float jumpSpeed = 6f;
    private float crouchHeight = 1f;
    private float slideTime = 0.5f;


    private float speed;
    private float defaultHeight;
    private float slideElapsed = 0f;
    private float jumpElapsed = 0f;
    private float jumpDelay = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector3 vel;
    private Vector3 inputDir;
    private Vector3 groundNormal;
    private bool onGround = false;
    private bool slide = false;
    private bool jump = false;

    private float turnDelta;
    private Quaternion oldRot;

    void Start() {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        defaultHeight = capsule.height;
        speed = runSpeed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        if (Input.GetButtonDown("Crouch")) {
            if (!onGround) {
                slide = true;
                slideElapsed = 0f;
            }
            speed = crouchSpeed;
            SetHeight(crouchHeight);
            Crouch.Invoke();
        }
        if (Input.GetButtonUp("Crouch")) {
            slide = false;
            speed = runSpeed;
            SetHeight(defaultHeight);
            Stand.Invoke();
        }

        // Jump
        if (Input.GetButtonDown("Jump")) {
            jump = true;
            slide = false;
        }
            
        if (Input.GetButtonUp("Jump"))
            jump = false;

        GetMovementInput();
    }

    private void FixedUpdate() {
        vel = rb.velocity;


        if (jumpElapsed < jumpDelay) {
            jumpElapsed += Time.deltaTime;
        }  
        else if (jump && onGround) {
            if (vel.y < 0f)
                vel.y = 0f;
            vel.y += jumpSpeed;
            jump = false;
            onGround = false;
        }
            


        if (onGround) {
            if (slide) {
                if (slideElapsed < slideTime) {
                    SlideBoost();
                    slideElapsed += Time.deltaTime;
                }

                Slide();
                Fall();

                if (vel.magnitude < crouchSpeed)
                    slide = false;
            }
            else
                Run();

            ApplyFriction();
        }
        else {
            AirControl();
            Fall();
        }

        


        rb.velocity = vel;
        ResetContacts();
    }

    private void Run() {
        Vector3 hVel = vel;
        hVel.y = 0;

        // Rotate movement vector to match ground tangent
        inputDir = Vector3.Cross(Vector3.Cross(groundNormal, inputDir), groundNormal);
        Debug.DrawLine(transform.position, transform.position + inputDir);

        // Acceleration
        if (Vector3.Dot(hVel, inputDir) < speed)
            vel += inputDir * accel * Time.deltaTime;
    }

    private void Fall() {
        vel.y -= gravity * Time.deltaTime;
    }

    private void AirControl() {
        Vector3 hVel = vel;
        hVel.y = 0;

        if (Vector3.Dot(hVel, inputDir) < 0.5f)
            vel += inputDir * accel * Time.deltaTime * 0.5f;
    }

    private void SlideBoost() {
        if (vel.magnitude > 0.1f)
            vel += vel.normalized * slideAccel * Time.deltaTime;
        else
            vel += transform.forward * slideAccel * Time.deltaTime;
    }

    private void Slide() {
        Vector3 hVel = vel;
        hVel.y = 0;

        if (Vector3.Dot(hVel, inputDir) < 0.3f)
            vel += inputDir * accel * Time.deltaTime;
    }

    private void ApplyFriction() {
        float mod = 1f;
        if (slide)
            mod = 0.1f;

        vel *= Mathf.Clamp01(1 - Time.deltaTime * friction * mod);
    }

    private void SetHeight(float height) {
        transform.position -= Vector3.up * ((capsule.height - height) / 2f);
        capsule.height = height;
    }

    void GetMovementInput() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        inputDir = transform.rotation * new Vector3(x, 0f, z).normalized;
    }

    private void OnCollisionStay(Collision other) {
        // Find the ground contact closest to horizontal
        foreach (ContactPoint contact in other.contacts) {
            if (contact.normal.y > groundNormal.y && contact.normal.y > 0.7f) {
                groundNormal = contact.normal;
                onGround = true;
            }
        }

        if (onGround)
            return;
    }

    private void ResetContacts() {
        onGround = false;
        groundNormal = Vector3.zero;
    }

    public bool IsCrouched() {
        return capsule.height == crouchHeight;
    }
}
