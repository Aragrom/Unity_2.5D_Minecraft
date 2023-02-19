using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using System.Threading;

[BurstCompile]
public class ClientEngine : MonoBehaviour
{
    public PortMap portMap = null;
    public InternetAccess internetAccess = null;
    public LocalAddresses localAddresses = null;
    public PublicAddress publicAddress = null;

    public DotNetTcpClient dotNetTcpClient = null;

    private NetworkedActorMovement networkedActorMovement = null;
    private TreadmillEngine treadmillEngine = null;                  // need to true position for player position

    public Type type = Type.DotNET;
    public Protocol protocol = Protocol.TCP;

    public string ipv4 = "127.0.0.1";
    public int port = 13000;
    public string message = "yo";                               // Input/message sent to the Game Server.

    public bool isTryingToConnect = false;
    public bool connected = false;                              // true when tcpClient successfully connects and stays connected. (tcpClient.Connected)

    // Inspector manual controls
    public bool sendMessage = false;                            // Toggle true to send message to server. (Only works once connected)
    public bool connectToGameServer = false;                    // Toggle true to connect to the Game Server.
    public bool disconnect = false;

    public bool openPort = false;
    public bool closePort = false;

    public Transform player = null;

    public List<Vector3> positions = new List<Vector3>();

    public float SEND_RATE = 0.05f;
    public float sendTimer = 0.05f;

    public enum Type 
    {
        DotNET  // c# .NET library used for client functionality. Connect, send, recieve etc.
    }

    public enum Protocol
    { 
        TCP,
        UDP
    }

    [BurstCompile]
    public void OnDestroy()
    {
        portMap = null;
        internetAccess = null;
        localAddresses = null;
        publicAddress = null;

        dotNetTcpClient = null;

        networkedActorMovement = null;
        treadmillEngine = null;                  // need to true position for player position

        player = null;

        positions.Clear();
        positions = null;
    }

    [BurstCompile]
    public void Awake()
    {
        portMap = GetComponent<PortMap>();
        internetAccess = GetComponent<InternetAccess>();
        localAddresses = GetComponent<LocalAddresses>();
        publicAddress = GetComponent<PublicAddress>();
        dotNetTcpClient = GetComponent<DotNetTcpClient>();

        networkedActorMovement = GameObject.Find("Networked Actor Engine").GetComponent<NetworkedActorMovement>();
        treadmillEngine = GameObject.Find("Treadmill Engine").GetComponent<TreadmillEngine>();

        player = GameObject.Find("Player").transform;

        internetAccess.CheckAccess();
        localAddresses.GetLocalDnsData();

        // Static
        PublicAddress.GetExternalIp(publicAddress.publicAddressData);
        PortMap.GetPortMaps(portMap.portMapData);
    }

    [BurstCompile]
    public void Update()
    {
        // CHANGE from being an update check to called from Canvas UI <<<<<<<<<<<<<<<<<<<<<============

        if (openPort)
        {
            openPort = false;
            PortMap.CreatePortMap(portMap.port, portMap.ipAddress, portMap.portMapData);
        }

        if (closePort)
        {
            closePort = false;
            PortMap.DeletePortMap(portMap.port, portMap.portMapData);
        }

        if (connectToGameServer
            && connected == false
            && isTryingToConnect == false)
        {
            connectToGameServer = false;
            isTryingToConnect = true;

            Thread connectThread = new Thread(() => Connect(ipv4, port));
            connectThread.Start();
        }

        Connected();

        if (connected == false) return;

        sendTimer -= Time.deltaTime;

        if (sendTimer < 0)
        {
            sendTimer = SEND_RATE;

            SendPlayerPositionToServer();
        }

        // if not connected this code will never be reached.

        if (sendMessage)
        {
            sendMessage = false;
            Send(message);
        }

        Receive();

        if (disconnect)
        {
            disconnect = false;
            Disconnect();
        }
    }

    [BurstCompile]
    public void SendPlayerPositionToServer()
    {
        // Calculate and send playing position.
        // actual position is both player position plus the treadmill position.

        Send((player.position + treadmillEngine.truePosition).ToString());
    }

    [BurstCompile]
    public void Connected()
    {
        switch (type)
        {
            case Type.DotNET:
                if (dotNetTcpClient.tcpClient != null) connected = dotNetTcpClient.tcpClient.Connected;
                else connected = false;
                break;
        }
    }

    [BurstCompile]
    public void ConnectToServer(ServerInformation serverInformation)
    {
        switch (type)
        {
            case Type.DotNET:

                dotNetTcpClient.ConnectToServer(serverInformation);
                break;
        }
    }

    [BurstCompile]
    public void Connect(string ipv4, int port)
    {
        switch (type)
        {
            case Type.DotNET:

                dotNetTcpClient.Connect(ipv4, port);
                break;
        }
    }

    [BurstCompile]
    public void Send(string message)
    {
        switch (type)
        {
            case Type.DotNET:

                dotNetTcpClient.Send(message);
                break;
        }
    }

    [BurstCompile]
    public void Receive()
    {
        switch (type)
        {
            case Type.DotNET:

                dotNetTcpClient.Receive();
                break;
        }
    }

    [BurstCompile]
    public void Disconnect()
    {
        switch (type)
        {
            case Type.DotNET:

                dotNetTcpClient.Disconnect();
                break;
        }
    }

    // Called from inside (as an example) DotNetTcpClient.Receive()
    [BurstCompile]
    public void Parse(string data)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 tempVector3;

        int index = 0;
        int range = 0;

        for (int i = 0; i < data.Length; i++)
        {
            range++;

            switch (data[i])
            {
                case '{':

                    // Start of message (message can have multiple positions)

                    positions.Clear();

                    break;

                case '}':

                    // End of message

                    break;

                case '<':

                    // Start of position

                    index = i + 1;
                    range = 0;

                    break;

                case '>':

                    // End of position

                    tempVector3 = StringToVector3(data.Substring(index, range - 1));

                    positions.Add(tempVector3);

                    break;
            }
        }

        // update position

        networkedActorMovement.SetTargets(positions);
    }

    [BurstCompile]
    public Vector3 StringToVector3(string vector3)
    {
        // Remove the parentheses
        /*if (vector3.StartsWith("<") && vector3.EndsWith(">"))
        {
            vector3 = vector3.Substring(1, vector3.Length - 2);
        }*/

        // split the items
        string[] sArray = vector3.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
}
