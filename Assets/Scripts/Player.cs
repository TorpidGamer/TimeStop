using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [HideInInspector] public delegate void Stop();
    [HideInInspector] public static Stop onStop;

    [HideInInspector] public delegate void Resume();
    [HideInInspector] public static Resume onResume;

    [HideInInspector] public delegate void Rewind();
    [HideInInspector] public static Rewind onRewind;

    [HideInInspector] public delegate void StopAfterRewind();
    [HideInInspector] public static StopAfterRewind onStopAfterRewind;

    [SerializeField] bool isRewinding, isStopped;
    bool hasChangedThisFrame;
    public float acceleration, deceleration, topSpeed, maxSpeed, jumpForce;
    public float airAccel, airDecel;
    Rigidbody rb;
    Vector2 moveAxis;

    [SerializeField] float radius;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        TimeInputs();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void TimeInputs()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isRewinding) { onRewind(); isRewinding = true; }
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (isRewinding)
            {
                onResume();
                isRewinding = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isRewinding)
            {
                if (!isStopped) { onStop(); isStopped = true; hasChangedThisFrame = true; }
                if (isStopped && !hasChangedThisFrame) { onResume(); isStopped = false; }
                hasChangedThisFrame = false;
            }
            else
            {
                onStopAfterRewind();
                isStopped = true;
            }
        }
    }

    private void Move()
    {
        moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveAxis.Normalize();
        float currentSpeed = rb.velocity.magnitude;
        if (isGrounded())
        {
            if (moveAxis == Vector2.zero)
            {
                Vector3 decelVec = new Vector3(-rb.velocity.x / 2 * deceleration * Time.deltaTime, 0, -rb.velocity.z / 2 * deceleration * Time.deltaTime);
                rb.AddForce(decelVec, ForceMode.Acceleration);
                //rb.velocity = new Vector3((rb.velocity.x * deceleration) * Time.deltaTime, rb.velocity.y, (rb.velocity.z * deceleration) * Time.deltaTime);
            }
            else
            {
                if (currentSpeed < topSpeed)
                {
                    rb.AddForce(transform.forward * moveAxis.y * acceleration * Time.deltaTime + transform.right * moveAxis.x * acceleration * Time.deltaTime);
                }
            }
        }
        else
        {
            if (moveAxis == Vector2.zero)
            {
                Vector3 decelVec = new Vector3(-rb.velocity.x / 2 * airDecel * Time.deltaTime, 0, -rb.velocity.z / 2 * airDecel * Time.deltaTime);
                rb.AddForce(decelVec, ForceMode.Acceleration);
                //rb.velocity = new Vector3((rb.velocity.x * airDecel) * Time.deltaTime, rb.velocity.y, (rb.velocity.z * airDecel) * Time.deltaTime);
            }
            else
            {
                if (currentSpeed < topSpeed)
                {
                    rb.AddForce(transform.forward * moveAxis.y * airAccel * Time.deltaTime + transform.right * moveAxis.x * airAccel * Time.deltaTime);
                }
            }
        }
        if (currentSpeed > maxSpeed)
        {
            Vector3 decelVec = new Vector3(-rb.velocity.x/ 2 * airDecel * Time.deltaTime, 0, -rb.velocity.z / 2 * airDecel * Time.deltaTime);
            rb.AddForce(decelVec, ForceMode.Force);
        }
        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
    }

    bool isGrounded()
    {
        RaycastHit hit;
        return Physics.SphereCast(transform.position, radius, -Vector3.up, out hit, 1f);
    }
}
