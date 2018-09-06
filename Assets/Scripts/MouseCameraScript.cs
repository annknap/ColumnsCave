using UnityEngine;
using LZWPlib;

public class MouseCameraScript : MonoBehaviour
{

    float speed = 1.0f;
    Vector3 rotation = new Vector3(0, 0, 0);

    void Start()
    {

    }

    void Update()
    {
        if (Core.Instance.isServer)
        {
            rotation = this.transform.eulerAngles;

            if (Input.GetKey(KeyCode.D))
                rotation.y += speed;
            //this.transform.Rotate(0, speed, 0);
            if (Input.GetKey(KeyCode.A))
                rotation.y -= speed;
            //this.transform.Rotate(0, -speed, 0);
            if (Input.GetKey(KeyCode.W))
                rotation.x -= speed;
            //this.transform.Rotate(-speed, 0, 0);
            if (Input.GetKey(KeyCode.S))
                rotation.x += speed;
            //this.transform.Rotate(speed, 0, 0);

            this.transform.eulerAngles = rotation;
        }
    }

    

}