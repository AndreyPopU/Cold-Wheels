using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;

    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();
    private ParticleSystem pickUp;

    private void Start()
    {
        pickUp = GetComponentInChildren<ParticleSystem>();
        posOffset = transform.position;
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, 5);

        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarSingle car))
        {
            car.nitroFuel += 2;
            if (!car.bot) GameManager.instance.nitroSlider.value = car.nitroFuel;
            pickUp.Play();
            pickUp.transform.SetParent(null);
            Destroy(gameObject);
        }
    }
}
