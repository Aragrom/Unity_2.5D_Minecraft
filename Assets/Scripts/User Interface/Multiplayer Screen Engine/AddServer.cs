using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddServer : MonoBehaviour
{
    public TitleScreenEngine titleScreenEngine = null;
    public MultiplayerMenuEngine multiplayerMenuEngine = null;

    public GameObject canvas = null;
    public GameObject firstSelected = null; // ip address input field

    public TMP_InputField tmpInputIPAddress = null;
    public TMP_InputField tmpInputPort = null;
    public GameObject addServerButton = null;
    public GameObject back = null;

    [BurstCompile]
    public void Awake()
    {
        titleScreenEngine = GameObject.Find("Title Screen Engine").GetComponent<TitleScreenEngine>();
        multiplayerMenuEngine = GetComponent<MultiplayerMenuEngine>();
    }

    [BurstCompile]
    public void ClickAddServer()
    {
        // TO DO: Check server isn't already in list.
        // TO DO: Should try to connect to the server to get server name and description.

        //titleScreenEngine.multiplayerMenuEngine.clientEngine

        int.TryParse(tmpInputPort.text, out int port);

        multiplayerMenuEngine.serverList.Add(tmpInputIPAddress.text, port);

        // Return to the multiplayer canvas.
        ClickBack();
    }

    [BurstCompile]
    public void ClickBack()
    {
        //titleScreenEngine.currentMenu = TitleScreenEngine.Menu.Multiplayer;
        multiplayerMenuEngine.currentMenu = MultiplayerMenuEngine.Menu.ServerList;

        multiplayerMenuEngine.addServer.canvas.SetActive(false);
        multiplayerMenuEngine.serverList.canvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(titleScreenEngine.multiplayerMenuEngine.addServerButton);
    }
}
