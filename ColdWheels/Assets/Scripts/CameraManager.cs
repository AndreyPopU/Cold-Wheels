using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera cam;
    public Transform car;
    public bool shouldShake;
    public float shakeForce = .2f;
    public float distance = 6.4f;
    public float height = 1.4f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;
    public float zoomRatio = 0.5f;
    public float defaultFOV = 60f;
    private Vector3 rotationVector;
    private Vector3 shakeVector;

    void FixedUpdate()
    {
        Vector3 localVelocity = car.InverseTransformDirection(car.GetComponent<Rigidbody>().velocity);
        if (localVelocity.z < 10.5f)
        {
            Vector3 temp = rotationVector; 
            temp.y = car.eulerAngles.y + 180;
            rotationVector = temp;
        }
        else
        {
            Vector3 temp = rotationVector;
            temp.y = car.eulerAngles.y;
            rotationVector = temp;
        }

        float wantedAngle = rotationVector.y;
        float wantedHeight = car.position.y + height; //car.position.y + height;
        float myAngle = transform.eulerAngles.y;
        float myHeight = transform.position.y;

        myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping * Time.fixedDeltaTime);
        myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping * Time.fixedDeltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);
        
        transform.position = car.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        Vector3 temp1 = transform.position; //temporary variable so Unity doesn't complain
        temp1.y = myHeight;

        

        if (!shouldShake)
        {
            transform.position = temp1;
            if (cam.fieldOfView > 63) cam.fieldOfView -= Time.fixedDeltaTime * 8;
            else
            {
                float acc = car.GetComponent<Rigidbody>().velocity.magnitude;
                cam.fieldOfView = defaultFOV + acc * zoomRatio * Time.deltaTime;
            }
        }
        else
        {
            shakeVector = new Vector3(Random.Range(-1, 1) * shakeForce, Random.Range(-1, 1) * shakeForce, 0);
            if (cam.fieldOfView < 70) cam.fieldOfView += Time.fixedDeltaTime * 8;
            transform.position = temp1 + shakeVector;
        }

        transform.LookAt(car);
    }
}
