using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class DotNetTcpClient : MonoBehaviour
{
    public TcpClient tcpClient = null;                                 // Functionality for clients to Connect - Receive and Send messages.

    public ClientEngine clientEngine = null;
    
    private int messageLength = 0;                              // Used in checking and storing the length of messages.
    const int MAX_MESSAGE_SIZE = 256;                           // In Bytes.
    private Byte[] bytes = new Byte[MAX_MESSAGE_SIZE];          // Buffer to store the response bytes.
    private String data = String.Empty;                         // String to store the response ASCII representation.

    public StringBuilder stringBuilder = null;

    [BurstCompile]
    private void Awake()
    {
        clientEngine = GameObject.Find("Client Engine").GetComponent<ClientEngine>();

        stringBuilder = new StringBuilder();
    }

    // On application quit or returning to main menu
    [BurstCompile]
    private void OnDestroy()
    {
        if (tcpClient != null)
        {
            Disconnect();
        }
    }

    [BurstCompile]
    public void ConnectToServer(ServerInformation serverInformation)
    {
        Thread connectThread = new Thread(() => Connect(serverInformation.ipv4, serverInformation.port));
        connectThread.Start();
    }

    [BurstCompile]
    public void Connect(string address, int port)
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.

            Debug.Log("Attempting to connect to Server: " + address + " on Port: " + port);

            tcpClient = new TcpClient(address, port);

            if (tcpClient.Connected)
            {
                clientEngine.isTryingToConnect = false;
                clientEngine.connected = true;
                Debug.Log("Success connecting to the server. Client IP Address: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString() + " Port: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString());
            }
            else
            {
                clientEngine.isTryingToConnect = false;
                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpClient = null;

                Debug.Log("Failure connecting to the server");
            }
        }
        catch (Exception e)
        {
            clientEngine.isTryingToConnect = false;
            Debug.Log("Client.Connect - Exception: " + e);
        }
    }

    [BurstCompile]
    public void Receive()
    {
        try
        {
            if (tcpClient.Connected == false) return;

            stringBuilder.Clear();

            // Loop to receive all the data sent by the server.
            while (tcpClient.GetStream().DataAvailable)
            {
                // Read the first batch of the TcpServer response bytes.
                messageLength = tcpClient.GetStream().Read(bytes, 0, MAX_MESSAGE_SIZE);

                data = Encoding.ASCII.GetString(bytes, 0, messageLength);

                stringBuilder.Append(data);

                Debug.Log("Received: " + data);
            }

            clientEngine.Parse(stringBuilder.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Client.Receive - Exception: " + e);
        }
    }

    [BurstCompile]
    public void Send(string message)
    {
        try
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            if (bytes.Length <= MAX_MESSAGE_SIZE)
            {
                tcpClient.GetStream().Write(bytes, 0, bytes.Length);

                Debug.Log("Sent: " + message);
            }
            else
            {
                Debug.Log("Message size to large... (" + bytes.Length + "/" + MAX_MESSAGE_SIZE + ")");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Client.Send - Exception: " + e);
        }
    }

    [BurstCompile]
    public void Disconnect()
    {
        tcpClient.GetStream().Close();
        tcpClient.Close();
        tcpClient = null;
    }
}
