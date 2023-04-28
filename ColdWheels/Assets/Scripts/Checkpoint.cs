using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool finish;
    public bool checkpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarSingle car))
        {
            car.spawnPosition = other.transform.position;
            car.spawnRotation = other.transform.rotation;

            if (car.bot) car.GetComponent<Bot>().lastWayPoint = car.GetComponent<Bot>().currWayPoint;

            if (finish && car.laps < 3 && car.checkpointed)
            {
                car.laps++;
                if (!car.bot) GameManager.instance.lapsText.text = "Laps: " + car.laps + "/3";
                car.checkpointed = false;

                if (car.laps == 3) GameManager.instance.StartCoroutine(GameManager.instance.FinishRace());
            }

            if (checkpoint)
            {
                car.checkpointed = true;
                if (car.laps == 1) GameManager.instance.ActivatePowerUps();
            }
        }
    }
}
