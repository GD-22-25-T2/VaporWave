using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField] internal float MaxSpeed = 1000;
    [SerializeField] internal float JumpForce = 200;
    [SerializeField] internal LayerMask JumpableGrounds;
    Rigidbody rb;
    public bool IsGrounded;
    public bool CanDoublejump;
    




    void Start()
    {
        TryGetComponent(out CharacterInputs inputs);
        inputs.WalkEvent += WalkAction;
        inputs.JumpEvent += JumpAction;

        TryGetComponent(out Rigidbody rigidbody);
        rb = rigidbody;
    }

    void WalkAction(float speed)
    {
        rb.AddForce(Vector3.forward * speed * MaxSpeed * Time.deltaTime, ForceMode.Acceleration);
        if(speed < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    void JumpAction(bool tryToJump)
    {

        if (IsGrounded && tryToJump)
        {
            rb.AddForce(0, JumpForce * Time.deltaTime, 0, ForceMode.Impulse);

        }

        if (IsGrounded is false && CanDoublejump && Input.GetKey(KeyCode.S)/*provvisorio*/)
        {
            rb.AddForce(0, JumpForce * 4 * Time.deltaTime, 0, ForceMode.Impulse);
            CanDoublejump = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 6)
        {
            IsGrounded = true;
            CanDoublejump = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            IsGrounded = false;
        }
    }
}
