using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public InputField IPField;
    public InputField portField;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance.gameObject);
    }

    public void ConnectToServer()
    {
        //Client.instance.ip = IPField.text;
        //Client.instance.port = int.Parse(portField.text);

        Client.instance.ip = "127.0.0.1";
        Client.instance.port = 7766;
        portField.transform.parent.gameObject.SetActive(false);
        Client.instance.ConnectToServer();
    }
    
    public void StartRace()
    {

    }
}
