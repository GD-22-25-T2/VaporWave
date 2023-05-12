using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] private GameObject[] MovementCoordinates;
    int CurrentWaypoint = 0;
    [SerializeField] private float Speed = 10f;


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
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }
    */
}
