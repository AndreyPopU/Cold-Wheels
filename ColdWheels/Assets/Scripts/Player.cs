using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float turnSpeed;

    private Rigidbody rb;

    
    void Start()
    {
        speed *= Time.fixedDeltaTime;
        turnSpeed *= Time.fixedDeltaTime;
        rb = GetComponent<Rigidbody>();   
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        // Move only if grounded
        // Suspension - bounciness

        if (Input.GetButton("Vertical")) rb.AddForce(transform.forward * Time.deltaTime * speed * Input.GetAxisRaw("Vertical"), ForceMode.VelocityChange);

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.Rotate(new Vector3(0, turnSpeed * Input.GetAxisRaw("Horizontal"), 0));
        }

        //Vector3 forward = cam.transform.forward;
        //Vector3 right = cam.transform.right;

        //forwardDirection = new Vector3(forward.x, 0, forward.z).normalized;
        //rightDirection = new Vector3(right.x, 0, right.z).normalized;

        //// Camera rotation
        //if (Input.GetAxisRaw("Vertical") != 0)
        //{
        //    if (Input.GetAxisRaw("Vertical") > 0)
        //        camRotation = Quaternion.Euler(0, cam.holder.transform.localEulerAngles.y + transform.eulerAngles.y, 0);
        //    //else if (Input.GetAxisRaw("Vertical") < 0)
        //    //    camRotation = Quaternion.Euler(0, cam.holder.transform.localEulerAngles.y + transform.eulerAngles.y - 180, 0);

        //    transform.rotation = Quaternion.Lerp(transform.rotation, camRotation, .125f);
        //}

        ////if (Input.GetAxisRaw("Horizontal") != 0)
        ////{
        ////    Quaternion turnRotation = Quaternion.Euler(new Vector3(0, cam.holder.transform.eulerAngles.y + 90, 0));
        ////    transform.rotation = Quaternion.Lerp(transform.rotation, turnRotation, .125f);
        ////}

        
    }
}
