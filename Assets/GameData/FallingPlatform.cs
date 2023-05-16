using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float TimeBeforeFalling = 3;
    [SerializeField] float Countdown = 0;
    [SerializeField] float Countdown1 = 0;
    [SerializeField] bool IsFalling = false;
    [SerializeField] float TimeToRespawn = 3;
    [SerializeField] GameObject Platform;
    Vector3 RespawnPos;

    void Start()
    {
        RespawnPos = transform.position;
    }


    void Update()
    {
        if (IsFalling)
        {
            //float counter = 0;
            //counter += Time.deltaTime;
            Countdown1 += Time.deltaTime;
            if(Countdown1 >= TimeToRespawn)
            {
                Platform.transform.position = RespawnPos; 
                transform.position = RespawnPos;
                Platform.TryGetComponent(out BoxCollider collider);
                collider.enabled = true;
                Platform.TryGetComponent(out Rigidbody rb);
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                IsFalling = false;
                Countdown1 = 0;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Countdown += Time.deltaTime;
            if (Countdown >= TimeBeforeFalling)
            {
                IsFalling = true;
                Platform.TryGetComponent(out BoxCollider collider);
                collider.enabled = false;
                Platform.TryGetComponent(out Rigidbody rb);
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                other.TryGetComponent(out Rigidbody playerRB);
                playerRB.AddForce(0, -0.1f, 0);
                Countdown = 0;

            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && Countdown < TimeBeforeFalling)
        {
            Countdown = 0;
        }
    }
}
