﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class KLD_PlayerController : SerializedMonoBehaviour
{


    //[SerializeField] playerin
    [SerializeField] Transform axisTransform;
    Rigidbody rb;
    CapsuleCollider col;

    public enum ControllerMode
    {
        GRAVITY,
        NO_GRAVITY
    }

    [SerializeField]
    ControllerMode controllerMode;

    //AXIS
    [SerializeField, Header("Axis")] float axisDeadZoneMagnitude = 0.1f;
    Vector2 rawAxis; //raw unity axis
    Vector2 deadZonedRawAxis; //raw axis that are only 0 or 1
    Vector2 timedAxis = Vector2.zero; //axis that are timed with acceleration and deceleration times
    [SerializeField] float accelerationTime = 0.3f;
    [SerializeField] float decelerationTime = 0.2f;
    [Tooltip("When the axis is less than this, zero it")]
    [SerializeField] float axisZeroingDeadzone = 0.05f;
    [SerializeField] bool snapAxis = true;

    Vector2 axisVector; //normalized direction where the player moves

    [SerializeField, Header("Movement")]
    float speed = 10f;

    [SerializeField, Header("Jump")]
    float jumpSpeed = 10f;
    [SerializeField] float fallMultiplier = 2f; //more means a faster fall
    [SerializeField] float lowJumpMultiplier = 3f; //more means a lower minimal jump

    [SerializeField] LayerMask groundLayer;
    [SerializeField, ReadOnly] bool m_isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerJump();
    }

    private void FixedUpdate()
    {
        //axis
        DoDeadZoneRawAxis();
        DoTimedAxis();


        DoPlayerMove();
        DoPlayerRotation();

        m_isGrounded = isGrounded();

        CheckFall();
    }

    void DoDeadZoneRawAxis()
    {
        //rawAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); OLD INPUT

        float hori = (Mathf.Abs(rawAxis.x) >= axisDeadZoneMagnitude ? 1f : 0f) * Mathf.Sign(rawAxis.x);
        float vert = (Mathf.Abs(rawAxis.y) >= axisDeadZoneMagnitude ? 1f : 0f) * Mathf.Sign(rawAxis.y);

        deadZonedRawAxis = new Vector2(hori, vert);
    }

    void DoTimedAxis()
    {

        float hori = timedAxis.x;
        float vert = timedAxis.y;

        if (deadZonedRawAxis.x != 0f)
        {
            hori += deadZonedRawAxis.x > 0f ? 1f / accelerationTime * Time.fixedDeltaTime : 1f / accelerationTime * -Time.fixedDeltaTime;
            if (snapAxis && hori != 0f && (Mathf.Sign(hori) != Mathf.Sign(deadZonedRawAxis.x)))
            {
                hori = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(hori) >= axisZeroingDeadzone)
            {
                hori += hori > 0f ? 1f / decelerationTime * -Time.fixedDeltaTime : 1f / decelerationTime * Time.fixedDeltaTime;
            }
            else
            {
                hori = 0f;
            }
        }

        if (deadZonedRawAxis.y != 0f)
        {
            vert += deadZonedRawAxis.y > 0f ? 1f / accelerationTime * Time.fixedDeltaTime : 1f / accelerationTime * -Time.fixedDeltaTime;
            if (snapAxis && vert != 0f && (Mathf.Sign(vert) != Mathf.Sign(deadZonedRawAxis.y)))
            {
                vert = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(vert) >= axisZeroingDeadzone)
            {
                vert += vert > 0f ? 1f / decelerationTime * -Time.fixedDeltaTime : 1f / decelerationTime * Time.fixedDeltaTime;
            }
            else
            {
                vert = 0f;
            }
        }

        hori = Mathf.Clamp(hori, -1f, 1f);
        vert = Mathf.Clamp(vert, -1f, 1f);

        timedAxis = new Vector2(hori, vert);
    }

    void DoPlayerMove()
    {
        //float xSpeed = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed * 30f;
        //float zSpeed = Input.GetAxis("Vertical") * Time.fixedDeltaTime * speed * 30f;
        float xSpeed = timedAxis.x * Time.fixedDeltaTime * speed * 30f;
        float zSpeed = timedAxis.y * Time.fixedDeltaTime * speed * 30f;

        Vector3 flatSpeedVector = axisTransform.right * xSpeed + axisTransform.forward * zSpeed;
        axisVector = new Vector2(flatSpeedVector.x, flatSpeedVector.z).normalized;
        rb.velocity = new Vector3(flatSpeedVector.x, rb.velocity.y, flatSpeedVector.z);
    }

    void CheckPlayerJump()
    {
        if (/*Input.GetButtonDown("Jump")*/false && isGrounded()) //OLD INPUT
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity += Vector3.up * jumpSpeed;
        }
    }

    bool isGrounded()
    {
        float radius = col.radius * 0.9f;
        return Physics.CheckSphere(transform.position + Vector3.up * radius * 0.9f, radius, groundLayer);
    }

    void CheckFall()
    {
        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))  //check if we're jumping and gaining height
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void DoPlayerRotation()
    {
        if (timedAxis.magnitude > 0.1f)
        {
            float angleToLook = Vector3.SignedAngle(Vector3.forward, new Vector3(axisVector.x, 0f, axisVector.y), Vector3.up);
            //print(axisVector + "\n" + angleToLook);
            transform.rotation = Quaternion.Euler(0f, angleToLook, 0f);
        }
    }
}
