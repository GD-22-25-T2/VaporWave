using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float TimeBeforeFalling = 3;
    [SerializeField] float Countdown = 0;
    [SerializeField] bool IsFalling = false;
    [SerializeField] GameObject Platform;

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
                //other.TryGetComponent(out Rigidbody playerRB);
                //playerRB.AddForce(0, -0.1f, 0);
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
