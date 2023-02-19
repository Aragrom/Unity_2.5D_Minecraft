using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using System.IO;
using System.Text;

[System.Serializable]
public struct SteamClientData
{
    public CSteamID cSteamID;
    public string name;
}

public class SteamPoweredClient : MonoBehaviour
{
    SteamClientData steamClientData = new SteamClientData();  // Test = 480, MUST REMOVE FROM BUILD (480) SPACE WARS EXAMPLE <<<<< !!!!!!
    public List<SteamClientData> friendsSteamData = new List<SteamClientData>();
    public SteamManager steamManager = null;

    public bool sendMessage = false;
    public string message = "";
    public int targetOfMessage = 0;

    // pubData is the data we want to send, 
    // cubData is the number of bytes we want to send,
    // eP2PSendType is a method of delivery.
    /*public void Awake()
    {
        string hello = "Hello!";

        // allocate new bytes array and copy string characters as bytes
        byte[] bytes = new byte[hello.Length * sizeof(char)];
        System.Buffer.BlockCopy(hello.ToCharArray(), 0, bytes, 0, bytes.Length);

        SteamNetworking.SendP2PPacket(receiver, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable);
    }

    private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;*/

    void Start()
    {
        steamManager = GetComponent<SteamManager>();
        SteamFriends.SetListenForFriendsMessages(true);

        // setup the callback method
        //_p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);

        if (SteamManager.Initialized)
        {
            steamClientData = new SteamClientData();
            steamClientData.name = SteamFriends.GetPersonaName();
            steamClientData.cSteamID = SteamUser.GetSteamID();
            Debug.Log(steamClientData.name);

            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);

            for (int i = 0; i < friendCount; i++)
            {
                SteamClientData newSteamClientData = new SteamClientData(); // do not use simplified object creation!

                newSteamClientData.cSteamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                newSteamClientData.name = SteamFriends.GetFriendPersonaName(newSteamClientData.cSteamID);

                friendsSteamData.Add(newSteamClientData);
            }
        }
    }

    private void Update()
    {
        if (sendMessage)
        {
            sendMessage = false;
            SendMessage(targetOfMessage, message);
        }

        uint msgSize;
        while (SteamNetworking.IsP2PPacketAvailable(out msgSize))
        {
            byte[] byteArray = new byte[msgSize];
            CSteamID remote;
            uint newMsgSize;
            SteamNetworking.ReadP2PPacket(byteArray, msgSize, out newMsgSize, out remote);

            string message = System.Text.Encoding.Default.GetString(byteArray);

            Debug.Log(message);
        }
    }

    private void SendMessage(int friendIndex, string message)
    {
        //CSteamID targetID = friendsSteamData[friendIndex].cSteamID;
        CSteamID targetID = steamClientData.cSteamID;

        byte[] byteArray = Encoding.UTF8.GetBytes(message);
        MemoryStream memoryStream = new MemoryStream(byteArray);
        byte[] pubData = memoryStream.ToArray();

        uint cubData = (uint)memoryStream.Length; 
        EP2PSend eP2PSendType = EP2PSend.k_EP2PSendReliable;
        int nChannel = 0;

        if (SteamNetworking.SendP2PPacket(targetID, pubData, cubData, eP2PSendType, nChannel) == true)
        {
            Debug.Log("Successfully send the message over steam!!");
        }
        else
        {
            Debug.Log("Failed to send the message over steam");
        }
    }    

    /*private void Update()
    {
        uint size;

        // repeat while there's a P2P message available
        // will write its size to size variable
        while (SteamNetworking.IsP2PPacketAvailable(out size))
        {
            // allocate buffer and needed variables
            var buffer = new byte[size];
            uint bytesRead;
            CSteamID remoteId;

            // read the message into the buffer
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
            {
                // convert to string
                char[] chars = new char[bytesRead / sizeof(char)];
                Buffer.BlockCopy(buffer, 0, chars, 0, chars.Length);

                string message = new string(chars, 0, chars.Length);
                Debug.Log("Received a message: " + message);
            }
        }
    }


    void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
        CSteamID clientId = request.m_steamIDRemote;
        if (ExpectingClient(clientId))
        {
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }
        else
        {
            Debug.LogWarning("Unexpected session request from " + clientId);
        }
    }

    bool ExpectingClient(CSteamID clientId)
    {
        return false;
    }*/
}