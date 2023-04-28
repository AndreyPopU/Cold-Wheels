using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        int id = packet.ReadInt();
        Client.instance.id = id;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        MultiplayerManager.instance.SpawnPlayer(id, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        MultiplayerManager.players[id].transform.position = position;
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        MultiplayerManager.players[id].transform.rotation = rotation;
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();

        Destroy(MultiplayerManager.players[id].gameObject);
        MultiplayerManager.players.Remove(id);
    }

    public static void Nitro(Packet packet)
    {
        int id = packet.ReadInt();
        float nitro = packet.ReadFloat();

        MultiplayerManager.players[id].car.UpdateNitro(nitro);
    }
}
