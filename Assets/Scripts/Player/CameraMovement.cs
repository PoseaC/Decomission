﻿using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Rigidbody player; //player, used for horizontal movement of the camera
    public Transform head; //player's head position, this will be where the camera will stay relative to the player
    public Camera overlay;
    public bool isAgent = true;

    //mouse sensitivity
    public float sensitivity = 100f;

    //maximum vertical angle the camera can look up or down to
    public float maxAngle = 90f;

    //rotation of the camera
    [HideInInspector] public float yRotation;
    [HideInInspector] public float xRotation;

    float xMouse;
    float yMouse;

    [HideInInspector] public float wallrunTilt = 0;
    private void Start()
    {
        xRotation = yRotation = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 500);
    }

    void Update()
    {
        //get the mouse input from the player
        xMouse = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        yMouse = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if (isAgent)
        {
            xMouse = 0;
            yMouse = 0;
        }

        //rotate the head for up and down movement
        yRotation -= yMouse;
        yRotation = Mathf.Clamp(yRotation, -maxAngle, maxAngle);
        head.localRotation = Quaternion.Euler(yRotation, 0, wallrunTilt);

        //rotate the player for left and right movement
        xRotation += xMouse;
        player.transform.rotation = Quaternion.Euler(0, xRotation, 0);
        if (isAgent)
        {
            head.localRotation = Quaternion.Euler(AgentIO.output.CameraOrientation.x * 180, AgentIO.output.CameraOrientation.y * 180, 0);
        }

        //camera follows the head's position and rotation
        transform.SetPositionAndRotation(head.position, head.rotation); 
        AgentIO.input.CameraRotation = Quaternion.Euler(head.localEulerAngles.x, player.transform.rotation.eulerAngles.y, 0);
    }
}
