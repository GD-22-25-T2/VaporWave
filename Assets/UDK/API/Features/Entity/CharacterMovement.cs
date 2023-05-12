using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField] internal float MaxSpeed = 1000;
    [SerializeField] internal float JumpForce = 200;
    [SerializeField] internal LayerMask JumpableGrounds;
    [SerializeField] internal bool CanJump;
    [SerializeField] internal bool CanDoublejump;
    [SerializeField] internal bool CanWalljump;
    [SerializeField] internal bool CanStickToWalls = true;
    Rigidbody rb;

    [SerializeField] internal float TimeStickedToWalls = 1;


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
        
        if (CanJump && CanWalljump is false && tryToJump) //Jump
        {
            rb.AddForce(0, JumpForce * Time.deltaTime, 0, ForceMode.Impulse);
        }

        if (!CanJump && CanDoublejump && Input.GetKey(KeyCode.S)/*provvisorio*/) //DoubleJump
        {
            rb.AddForce(0, JumpForce * 4 * Time.deltaTime, 0, ForceMode.Impulse);
            CanDoublejump = false;
        }
        

        if (!CanJump && CanWalljump && tryToJump) //WallJump
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.AddForce(0, JumpForce * 8 * Time.deltaTime, 0, ForceMode.Impulse);
            CanWalljump = false;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 6) //Ground
        {
            CanJump = true;
            CanDoublejump = true;
        } 
        if(collision.gameObject.layer == 7) //Wall
        {
            StartCoroutine(StickToWalls());
        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6) //Ground
        {
            CanJump = false;
        }
        if (collision.gameObject.layer == 7) //Wall
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            CanWalljump = false;
            CanStickToWalls = true;
        }
    }

    IEnumerator StickToWalls()
    {
        if (CanStickToWalls)
        {
            CanWalljump = true;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            yield return new WaitForSeconds(TimeStickedToWalls);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            CanStickToWalls = false;
        }

    }
}
