using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        transform.LookAt(transform.position + transform.forward);
    }
}
