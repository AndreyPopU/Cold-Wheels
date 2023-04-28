using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int bufferSize = 4096;

    public string username;
    public string ip = "127.0.0.1";
    public int port = 7766;
    public int id = 0;
    public TCP tcp;
    public UDP udp;
    public bool isConnected;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else { instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void ConnectToServer()
    {
        tcp = new TCP();
        udp = new UDP();

        InitializeClientData();

        tcp.Connect();
        isConnected = true;
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = bufferSize,
                SendBufferSize = bufferSize
            };

            receiveBuffer = new byte[bufferSize];
            socket.BeginConnect(instance.ip, instance.port, OnConnect, null);
        }

        public void OnConnect(IAsyncResult ar)
        {
            socket.EndConnect(ar);

            if (!socket.Connected) return;

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null) stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception e) { Debug.LogError(e); }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int byteLength = stream.EndRead(ar);
                if (byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));

                stream.BeginRead(receiveBuffer, 0, byteLength, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Disconnect();
                Debug.LogError(e);
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();
                if (packetLength < 1) return true;
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        packetHandlers[packetId](packet);
                    }
                });

                packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();
                    if (packetLength < 1) return true;
                }
            }

            if (packetLength <= 1) return true;

            return false;
        }

        public void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint ipep;

        public UDP()
        {
            ipep = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort);

            socket.Connect(ipep);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet())
            {
                SendData(packet);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.id);
                if (socket != null) socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
            }
            catch (Exception e) { Debug.LogError(e); }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                byte[] data = socket.EndReceive(ar, ref ipep);

                socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(data);
            }
            catch (Exception e) { Disconnect(); Debug.LogError(e); }
        }

        public void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(data))
                {
                    int packetId = _packet.ReadInt();
                    packetHandlers[packetId](_packet);
                }
            });
        }

        public void Disconnect()
        {
            instance.Disconnect();
            ipep = null;
            socket = null;
        }
    }

    public void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.Nitro, ClientHandle.Nitro },
        };
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            tcp.socket.Close();
            udp.socket.Close();
            isConnected = false;

            print("Disconnected");
        }
    }

    public void OnApplicationQuit()
    {
        Disconnect();
    }
}
