using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
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
    private CameraManager cam;

    private void Start()
    {
        cam = FindObjectOfType<CameraManager>();
        rb = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        rb.centerOfMass = COM;
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
            renderers[0].emitting = true;
            renderers[1].emitting = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            renderers[0].emitting = false;
            renderers[1].emitting = false;
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // Drifting
        if (Mathf.Abs(transform.InverseTransformDirection(rb.velocity).x) > 5)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                // Front
                if (i <= 1)
                {
                    if (wheels[i+2].isGrounded) renderers[i].emitting = true;
                }
            }
        }
        else
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                renderers[0].emitting = false;
                renderers[1].emitting = false;
            }
        }

        if (bot) return;
    }

    public void Respawn()
    {
        transform.rotation = spawnRotation;
        cam.transform.position = spawnPosition + (Vector3.forward * -cam.distance + Vector3.up * cam.height);
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

    public void UpdateNitro(float nitro)
    {
        nitroFuel = nitro;
        GameManager.instance.nitroSlider.value = nitro;
    }
}
