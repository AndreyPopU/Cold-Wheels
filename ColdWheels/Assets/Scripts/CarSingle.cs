using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSingle : MonoBehaviour
{
    public bool bot;
    public float speed;
    public float brakeForce;
    public float nitroForce;
    public float nitroFuel;
    public float maxSteerAngle;
    public bool canMove;
    public int laps;
    public bool checkpointed;
    public Vector3 COM;

    public float driftTime;
    public bool drifting;

    public Transform[] realWheels;
    public WheelCollider[] wheels;
    public TrailRenderer[] renderers;
    public ParticleSystem[] nitroParticle;
    public ParticleSystem[] roadParticles;

    public Vector3 spawnPosition;
    public Quaternion spawnRotation;

    private Rigidbody rb;
    [HideInInspector]
    public float steerAngle;
    private WheelFrictionCurve frontCurve;
    private CameraManager cam;

    private void Start()
    {
        cam = FindObjectOfType<CameraManager>();
        rb = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        rb.centerOfMass = COM;
        frontCurve = wheels[0].sidewaysFriction;
        if (GameManager.instance != null && GameManager.instance.nitroSlider != null)
        {
            GameManager.instance.nitroSlider.maxValue = nitroFuel;
            GameManager.instance.nitroSlider.value = nitroFuel;
        }
    }

    private void Update()
    {
        if (!canMove) return;

        // Particles
        if (wheels[2].isGrounded)
        {
            if (!roadParticles[0].isPlaying || !roadParticles[1].isPlaying)
            {
                roadParticles[0].Play();
                roadParticles[1].Play();
            }
        }
        else
        {
            if (roadParticles[0].isPlaying || roadParticles[1].isPlaying)
            {
                roadParticles[0].Stop();
                roadParticles[1].Stop();
                renderers[0].emitting = false;
            }
        }

        if (wheels[3].isGrounded)
        {
            if (!roadParticles[2].isPlaying || !roadParticles[3].isPlaying)
            {
                roadParticles[2].Play();
                roadParticles[3].Play();
            }
        }
        else
        {
            if (roadParticles[2].isPlaying || roadParticles[3].isPlaying)
            {
                roadParticles[2].Stop();
                roadParticles[3].Stop();
                renderers[1].emitting = false;
            }
        }

        // Visual Update
        VisualUpdate();

        if (bot) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject waypoint = new GameObject();
            waypoint.transform.position = transform.position;
        }

        if (Input.GetKeyDown(KeyCode.R)) Respawn();

        if (rb.velocity.magnitude > 140) cam.shouldShake = true;
        else cam.shouldShake = false;

        // Nitro particles
        if (nitroFuel <= 0 && nitroParticle[0].isPlaying)
        {
            nitroParticle[0].Stop();
            nitroParticle[1].Stop();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (nitroFuel > 0)
            {
                nitroParticle[0].Play();
                nitroParticle[1].Play();
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            nitroParticle[0].Stop();
            nitroParticle[1].Stop();
        }

        // Breaking
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = brakeForce;
                wheel.motorTorque = 0;
            }

            renderers[0].emitting = true;
            renderers[1].emitting = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            foreach (WheelCollider wheel in wheels)
                wheel.brakeTorque = 0;

            renderers[0].emitting = false;
            renderers[1].emitting = false;
        }
    }

    void FixedUpdate()
    {
        if (transform.position.y < -30) Respawn();

        if (!canMove) return;

        // Drifting
        if (Mathf.Abs(transform.InverseTransformDirection(rb.velocity).x) > 5)
        {
            frontCurve.stiffness = 2.4f;

            for (int i = 0; i < wheels.Length; i++)
            {
                // Front
                if (i <= 1)
                {
                    wheels[i].sidewaysFriction = frontCurve;
                    if (wheels[i + 2].isGrounded) renderers[i].emitting = true;
                }
                else // Rear
                {
                    wheels[i].sidewaysFriction = frontCurve;
                    wheels[i].brakeTorque = brakeForce;
                    wheels[i].motorTorque = 0;
                }
            }
        }
        else
        {
            frontCurve.stiffness = 4;

            if (!Input.GetKey(KeyCode.Space))
            {
                renderers[0].emitting = false;
                renderers[1].emitting = false;
            }

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].sidewaysFriction = frontCurve;
                if (i > 1) wheels[i].brakeTorque = 0;
            }
        }

        if (bot) return;

        // Nitro
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (nitroFuel > 0)
            {
                rb.AddForce(rb.transform.forward * -nitroForce, ForceMode.Impulse);
                nitroFuel -= Time.fixedDeltaTime;
                GameManager.instance.nitroSlider.value = nitroFuel;
            }
        }

        // Moving
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            foreach (WheelCollider wheel in wheels)
                wheel.motorTorque = speed * -Input.GetAxisRaw("Vertical");
        }

        // Steering
        steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
        wheels[0].steerAngle = steerAngle;
        wheels[1].steerAngle = steerAngle;
    }

    public void Respawn()
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (!bot) cam.transform.position = spawnPosition + (Vector3.forward * -cam.distance + Vector3.up * cam.height);
    }

    public void VisualUpdate()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 pos;
            Quaternion rot;
            wheels[i].GetWorldPose(out pos, out rot);
            realWheels[i].rotation = Quaternion.Lerp(realWheels[i].transform.rotation, rot, .125f);
        }
    }
}
