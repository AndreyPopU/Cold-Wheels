using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    public Transform[] waypoints;

    [HideInInspector]
    public CarSingle car;
    public int currWayPoint;
    public int lastWayPoint;

    void Awake()
    {
        car = GetComponent<CarSingle>();
        Invoke("ResetBot", 15);
        if (CanvasManager.multiplayer == 1) gameObject.SetActive(false);
    }

    void Update()
    {
        if (!car.canMove) return;

        if (Vector3.Distance(transform.position, waypoints[currWayPoint].position) < 35) NextDestination();
    }

    private void FixedUpdate()
    {
        if (!car.canMove) return;

        // Gas
        foreach (WheelCollider wheel in car.wheels)
            wheel.motorTorque = -car.speed;

        Debug.DrawRay(transform.position, (waypoints[currWayPoint].position - transform.position) * 50000, Color.red);

        // Steer
        Vector3 relativeVector = transform.InverseTransformPoint(waypoints[currWayPoint].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * -car.maxSteerAngle;

        car.wheels[0].steerAngle = newSteer;
        car.wheels[1].steerAngle = newSteer;
    }

    private void NextDestination()
    {
        CancelInvoke("ResetBot");
        Invoke("ResetBot", 7);
        currWayPoint++;
        if (currWayPoint >= waypoints.Length) currWayPoint = 0;
    }

    private void ResetBot()
    {
        car.Respawn();
        currWayPoint = lastWayPoint;
    }
}
