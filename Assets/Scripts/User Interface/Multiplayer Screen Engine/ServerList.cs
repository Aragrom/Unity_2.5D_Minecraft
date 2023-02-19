using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[BurstCompile]
public struct ServerInformation
{
    public string name;
    public string description;
    public string ipv4;
    public int port;
}

// use playerprefs to locally save server list
[BurstCompile]
public class ServerList : MonoBehaviour
{
    public ClientEngine clientEngine = null;

    public TitleScreenEngine titleScreenEngine = null;

    public GameObject canvas = null;
    public GameObject firstSelected = null;

    public GameObject content = null;
    public GameObject buttonPrefab = null;

    public ScrollRect scrollRect = null;
    public List<ServerInformation> servers = new List<ServerInformation>();
    public int maxLength = 100;
    public int previousCount = 0;      // need to know the old size of the server list to delete it before saving the fresh one.

    public bool isTryingToConnect = true;

    [BurstCompile]
    public void Awake()
    {
        titleScreenEngine = GameObject.Find("Title Screen Engine").GetComponent<TitleScreenEngine>();

        clientEngine = GameObject.Find("Client Engine").GetComponent<ClientEngine>();
    }

    [BurstCompile]
    public void OnApplicationQuit()
    {
        Save();
    }

    [BurstCompile]
    public void GenerateButtons()
    {
        // Create and for each button get and set the server information

        GameObject button;

        for (int i = 0; i < servers.Count; i++)
        {
            // add each button created to the content gameobject (parent)
            button = Instantiate(buttonPrefab);

            if (i == 0) firstSelected = button;

            Button b = button.GetComponent<Button>();
            b.onClick.AddListener(() => ClickOnServerButton(i - 1));

            // put ip and port as name
            button.transform.Find("Text IP and Port").GetComponent<TMP_Text>().text = servers[i].ipv4 + " " + servers[i].port;

            button.SetActive(true);
            button.transform.SetParent(content.transform, false);
            //tempButton.transform.Find("Text Server Name").GetComponent<TMP_Text>().text = "Server " + i;
        }
    }

    [BurstCompile]
    public void Add(string ipv4, int port)
    {
        ServerInformation serverInformation = new ServerInformation
        {
            ipv4 = ipv4,
            port = port,
            name = "Name",
            description = "Description"
        };

        servers.Add(serverInformation);

        // Need to create a button in the server list canvas.

        // add each button created to the content gameobject (parent)
        GameObject button = Instantiate(buttonPrefab);

        // put ip and port as name
        button.transform.Find("Text IP and Port").GetComponent<TMP_Text>().text = ipv4 + " " + port;

        Button b = button.GetComponent<Button>();
        b.onClick.AddListener(() => ClickOnServerButton(servers.Count - 1));

        button.SetActive(true);
        button.transform.SetParent(content.transform, false);
        //tempButton.transform.Find("Text Server Name").GetComponent<TMP_Text>().text = "Server " + i;
    }

    [BurstCompile]
    public void ClickOnServerButton(int index)
    {
        if (clientEngine.isTryingToConnect
            || clientEngine.connected) return;

        clientEngine.isTryingToConnect = true;

        Debug.Log("Connect to Server: IP Address " + servers[index].ipv4 + " Port: " + servers[index].port);

        clientEngine.Connect(servers[index].ipv4, servers[index].port);
    }

    [BurstCompile]
    public void Save()
    {
        // use current number of servers in list as new key
        // use order 0 > 1.. etc of server list to store

        // save name.
        // save description.
        // save ip.
        // save port.

        // Use convert to json to place all of this information for a server in a playerpref string.

        // Before saving though the server information layout and positions could have changed.
        // to reflect these changes perfectly delete all previously saved server information.

        for (int i = 0; i < previousCount; i++)
        {
            PlayerPrefs.DeleteKey("server" + i);
        }

        for (int i = 0; i < servers.Count; i++)
        {
            PlayerPrefs.SetString("server" + i.ToString(), JsonUtility.ToJson(servers[i]));
        }

        // save the number of servers
        PlayerPrefs.SetInt("numberOfServers", servers.Count);

        PlayerPrefs.SetInt("previousCount", servers.Count);
    }

    [BurstCompile]
    public void Load()
    {
        // check hard coded value for amount of server to load
        int serverCount = PlayerPrefs.GetInt("numberOfServers");

        previousCount = PlayerPrefs.GetInt("previousCount");

        // load all the server information
        for (int i = 0; i < serverCount; i++)
        {
            servers.Add(JsonUtility.FromJson<ServerInformation>(PlayerPrefs.GetString("server" + i.ToString())));
        }
    }

    [BurstCompile]
    public void ClickBack()
    {
        canvas.SetActive(false);
        titleScreenEngine.titleCanvas.SetActive(true);

        EventSystem.current.firstSelectedGameObject = titleScreenEngine.multiplayerButton;
        EventSystem.current.SetSelectedGameObject(titleScreenEngine.multiplayerButton);

        titleScreenEngine.currentMenu = TitleScreenEngine.Menu.Title;
    }
}
