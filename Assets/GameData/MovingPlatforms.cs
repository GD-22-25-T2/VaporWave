using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] GameObject[] MovementCoordinates;
    int CurrentWaypoint = 0;
    [SerializeField] float Speed = 10f;
    [SerializeField] bool ChangePlayerParent;

    void Update()
    {
        if (Vector3.Distance(MovementCoordinates[CurrentWaypoint].transform.position, transform.position) < 0.01f)
        {
            CurrentWaypoint++;
            if (CurrentWaypoint >= MovementCoordinates.Length)
            {
                CurrentWaypoint = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, MovementCoordinates[CurrentWaypoint].transform.position, Time.deltaTime * Speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ChangePlayerParent && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (ChangePlayerParent && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
