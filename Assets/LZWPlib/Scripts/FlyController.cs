using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    public float moveThreshold = 0.03f;
    public float lookSensitivity = 3.0f;


    [Header("In editor")]
    public float cameraSensitivity = 90;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    public bool HoldRMBToMove = true;


    void Update()
    {

        if (Application.isEditor)
        {
            if (Input.GetMouseButton(1) || !HoldRMBToMove)
            {

                rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
                rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, -90, 90);
                
                transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

                float speed = normalMoveSpeed;

                if (Input.GetKey(KeyCode.LeftShift))
                    speed *= fastMoveFactor;
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightShift))
                    speed *= slowMoveFactor;

                float ver = 0f;
                float hor = 0f;

                if (Input.GetKey(KeyCode.W))
                    ver = 1f;
                else if (Input.GetKey(KeyCode.S))
                    ver = -1f;

                if (Input.GetKey(KeyCode.D))
                    hor = 1f;
                else if (Input.GetKey(KeyCode.A))
                    hor = -1f;

                transform.position += transform.forward * speed * ver * Time.deltaTime;
                transform.position += transform.right * speed * hor * Time.deltaTime;


                if (Input.GetKey(KeyCode.E)) { transform.position += transform.up * speed * Time.deltaTime; }
                if (Input.GetKey(KeyCode.Q)) { transform.position -= transform.up * speed * Time.deltaTime; }
            }
        }
        else
        {
            if (LZWPlib.LzwpTracking.Instance.flysticks.Count < 1)
                return;

            float hor = LZWPlib.LzwpTracking.Instance.flysticks[0].joystickHorizontal;
            float ver = LZWPlib.LzwpTracking.Instance.flysticks[0].joystickVertical;


            Vector3 rotation = new Vector3(0, hor, 0) * lookSensitivity * Time.deltaTime;
            
            transform.Rotate(rotation);


            Vector3 pos = transform.position;

            if (Mathf.Abs(ver) >= moveThreshold)
                pos += transform.rotation * (LZWPlib.LzwpTracking.Instance.flysticks[0].rotation * Vector3.forward) * moveSpeed * ver * Time.deltaTime;

            transform.position = pos;
        }
    }
}
