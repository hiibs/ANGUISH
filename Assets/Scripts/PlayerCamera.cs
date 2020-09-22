using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private float mouseSens = 1f;
    private float moveRate = 100f;
    private float turnRate = 200f;
    private float eyeHeightStanding = 0.6f;
    private float eyeHeightCrouched = 0.3f;
    private float crouchTime = 0.1f;
    private float standTime = 0.1f;


    private float eyeHeight;
    private float crouchElapsed;
    private float standElapsed;

    private Vector3 inputRot;

    private Transform player;
    private PlayerMovement pMove;
    private float dampVel;

    private void Start() {
        player = GameObject.FindWithTag("Player").transform;
        pMove = player.GetComponent<PlayerMovement>();

        eyeHeight = eyeHeightStanding;
        crouchElapsed = crouchTime;
        standElapsed = standTime;

        pMove.Crouch.AddListener(Crouch);
        pMove.Stand.AddListener(Stand);
    }

    private void Update() {
        // Get mouse input
        inputRot.x += -Input.GetAxisRaw("Mouse Y") * mouseSens;
        inputRot.y += Input.GetAxisRaw("Mouse X") * mouseSens;

        // Limit yaw
        inputRot.x = Mathf.Clamp(inputRot.x, -85f, 85f);

        // Update
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(inputRot), turnRate * Time.deltaTime);

        // Animate crouch
        if (crouchElapsed < crouchTime) {
            float startHeight = eyeHeightStanding * 2 - eyeHeightCrouched;
            eyeHeight = Mathf.Lerp(startHeight, eyeHeightCrouched, crouchElapsed / crouchTime);
            crouchElapsed += Time.deltaTime;
        }

        // Animate stand
        if (standElapsed < standTime) {
            float startHeight = eyeHeightCrouched * 2 - eyeHeightStanding;
            eyeHeight = Mathf.Lerp(startHeight, eyeHeightStanding, standElapsed / standTime);
            standElapsed += Time.deltaTime;
        }

        Vector3 eyePos = player.position + Vector3.up * eyeHeight;

        transform.position = Vector3.Lerp(transform.position, eyePos, moveRate * Time.deltaTime);
        player.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    private void Crouch() {
        crouchElapsed = 0;
        standElapsed = standTime;
    }

    private void Stand() {
        standElapsed = 0;
        crouchElapsed = crouchTime;
    }
}
