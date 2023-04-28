using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static int multiplayer;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Multiplayer(int mult)
    {
        multiplayer = mult;
    }

    public void ChooseMap(int index)
    {
        SceneManager.LoadScene(index);

        if (multiplayer == 1)
        {
            // Send to server to teleport all clients
        }
    }
}
