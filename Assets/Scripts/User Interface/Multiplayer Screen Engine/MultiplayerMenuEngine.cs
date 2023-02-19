using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;

[BurstCompile]
public class MultiplayerMenuEngine : MonoBehaviour
{
    public InputEngine inputEngine = null;

    public TitleScreenEngine titleScreenEngine = null;
    public ClientEngine clientEngine = null;

    public ServerList serverList = null;
    public AddServer addServer = null;

    public GameObject addServerButton = null;

    public enum Menu { ServerList, AddServer }

    public Menu currentMenu = Menu.ServerList;
    //public bool insideMultiplayer = false;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        inputEngine = null;

        titleScreenEngine = null;
        clientEngine = null;

        serverList = null;
        addServer = null;

        addServerButton = null;
    }

    [BurstCompile]
    public void Awake()
    {
        inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();

        titleScreenEngine = GameObject.Find("Title Screen Engine").GetComponent<TitleScreenEngine>();
        clientEngine = GameObject.Find("Client Engine").GetComponent<ClientEngine>();

        serverList = GetComponent<ServerList>();
        addServer = GetComponent<AddServer>();

        serverList.Load();
        serverList.GenerateButtons();
    }

    [BurstCompile]
    public void Update()
    {
        if (inputEngine.boost)
        {
            // if we are on the server list menu then back should return us to the title screen.
            // Logic for that is located in TitleScreenEngine.cs
            if (currentMenu == Menu.ServerList) { return; }

            inputEngine.boost = false;

            switch (currentMenu)
            {
                case Menu.AddServer:

                    addServer.canvas.SetActive(false);
                    serverList.canvas.SetActive(true);                    

                    EventSystem.current.SetSelectedGameObject(null);
                    //EventSystem.current.firstSelectedGameObject = firstSelected;
                    //EventSystem.current.SetSelectedGameObject(firstSelected);

                    currentMenu = Menu.ServerList;
                    break;
            }
        }
    }

    [BurstCompile]
    public void ClickAddServer()
    {
        currentMenu = Menu.AddServer;

        serverList.canvas.SetActive(false);
        addServer.canvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(addServer.firstSelected);
    }
    
    [BurstCompile]
    public void Back()
    {
        // if we are on the server list menu then back should return us to the title screen.
        // Logic for that is located in TitleScreenEngine.cs
        if (currentMenu == Menu.ServerList) { return; }

        inputEngine.boost = false;

        switch (currentMenu)
        {
            case Menu.AddServer:

                addServer.canvas.SetActive(false);
                serverList.canvas.SetActive(true);

                EventSystem.current.SetSelectedGameObject(null);
                //EventSystem.current.firstSelectedGameObject = firstSelected;
                //EventSystem.current.SetSelectedGameObject(firstSelected);

                currentMenu = Menu.ServerList;
                break;
        }
    }
}
