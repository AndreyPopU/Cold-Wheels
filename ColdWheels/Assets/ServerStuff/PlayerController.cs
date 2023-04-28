using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void FixedUpdate() { SendInput(); }

    public void SendInput()
    {
        bool[] inputs = new bool[]
        {
            Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0,
            Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0,
            Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0,
            Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0,
            Input.GetButton("Jump"),
            Input.GetKey(KeyCode.LeftShift),
            Input.GetKeyDown(KeyCode.R),
        };

        ClientSend.PlayerMovement(inputs, transform.rotation);
    }
}
