using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    public Text testText;
    public static MultiplayerManager instance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static PlayerManager playerReference;

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else { instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void SpawnPlayer(int id, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        // If spawning local player
        if (id == Client.instance.id)
        {
            player = Instantiate(localPlayerPrefab, position, rotation);
            playerReference = player.GetComponent<PlayerManager>();
            Camera.main.GetComponent<CameraManager>().car = player.transform;
        }
        else player = Instantiate(playerPrefab, position, rotation);

        player.GetComponent<PlayerManager>().id = id;
        players.Add(id, player.GetComponent<PlayerManager>());
    }
}
