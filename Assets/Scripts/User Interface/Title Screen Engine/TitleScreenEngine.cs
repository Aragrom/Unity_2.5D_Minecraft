using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;

[BurstCompile]
public class TitleScreenEngine : MonoBehaviour
{
    public InputEngine inputEngine = null;

    public enum Menu { Title, Multiplayer, Options, CharacterCreation }
    public Menu currentMenu = Menu.Title;
    public bool onTitleScreen = true;

    public GameObject titleCanvas = null;

    public MovingTitleText movingTitleText = null;

    public GameObject newButton = null;
    public GameObject loadButton = null;
    public GameObject multiplayerButton = null;
    public GameObject optionsButton = null;

    public CharacterCreationEngine characterCreationEngine = null;
    public OptionEngine optionEngine = null;
    public MultiplayerMenuEngine multiplayerMenuEngine = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        inputEngine = null;

        titleCanvas = null;

        movingTitleText = null;

        newButton = null;
        loadButton = null;
        multiplayerButton = null;
        optionsButton = null;

        optionEngine = null;
        multiplayerMenuEngine = null;
    }

    [BurstCompile]
    public void Awake()
    {
        movingTitleText = GetComponent<MovingTitleText>();

        inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();

        characterCreationEngine = GameObject.Find("Character Creation Engine").GetComponent<CharacterCreationEngine>();
        multiplayerMenuEngine = GameObject.Find("Multiplayer Screen Engine").GetComponent<MultiplayerMenuEngine>();
        optionEngine = GameObject.Find("Option Engine").GetComponent<OptionEngine>();
    }

    [BurstCompile]
    public void ClickNewGame()
    {
        currentMenu = Menu.CharacterCreation;

        titleCanvas.SetActive(false);
        characterCreationEngine.EnableCanvas();
        characterCreationEngine.isCreatingCharacter = true;

        // enable sprite to edit.

        characterCreationEngine.characterImage.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(multiplayerMenuEngine.serverList.firstSelected);
    }

    [BurstCompile]
    public void ClickMultiplayer()
    {
        currentMenu = Menu.Multiplayer;

        titleCanvas.SetActive(false);
        multiplayerMenuEngine.serverList.canvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(multiplayerMenuEngine.serverList.firstSelected);
    }

    [BurstCompile]
    public void ClickOptions()
    {
        currentMenu = Menu.Options;

        titleCanvas.SetActive(false);
        optionEngine.canvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(optionEngine.firstSelected);
    }

    [BurstCompile]
    public void Update()
    {
        if (inputEngine.boost)
        {
            inputEngine.boost = false;

            switch (currentMenu)
            {
                case Menu.Multiplayer:

                    if (multiplayerMenuEngine.currentMenu == MultiplayerMenuEngine.Menu.ServerList)
                    {
                        multiplayerMenuEngine.serverList.canvas.SetActive(false);

                        //close menu
                        titleCanvas.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.firstSelectedGameObject = multiplayerButton;
                        EventSystem.current.SetSelectedGameObject(multiplayerButton);

                        currentMenu = Menu.Title;
                    }

                    break;

                case Menu.Options:

                    if (optionEngine.currentMenu == OptionEngine.Menu.Options)
                    {
                        optionEngine.canvas.SetActive(false);

                        //close menu
                        titleCanvas.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.firstSelectedGameObject = optionsButton;
                        EventSystem.current.SetSelectedGameObject(optionsButton);

                        currentMenu = Menu.Title; 
                    }

                    break;
            }
        }
    }


    // Navigate back depending on current menu
    [BurstCompile]
    public void Back()
    {
        switch (currentMenu)
        {
            case Menu.Multiplayer:

                // back to title screen
                currentMenu = Menu.Title;

                // Deactive Multiplayer canvas
                multiplayerMenuEngine.serverList.canvas.SetActive(false);

                // Activate title screen canvas
                titleCanvas.SetActive(true);

                EventSystem.current.firstSelectedGameObject = multiplayerButton;
                EventSystem.current.SetSelectedGameObject(multiplayerButton);

                break;

            case Menu.Options:

                // back to title screen
                currentMenu = Menu.Title;

                // Deactive Options canvas
                optionEngine.canvas.SetActive(false);

                // Activate title screen canvas
                titleCanvas.SetActive(true);

                EventSystem.current.firstSelectedGameObject = optionsButton;
                EventSystem.current.SetSelectedGameObject(optionsButton);

                break;

            /*case Menu.AddServer:

                // back to muliplayer
                currentMenu = Menu.Multiplayer;

                // Deactive Add Server canvas
                addServerCanvas.SetActive(false);

                // Activate multiplayer canvas
                multiplayerCanvas.SetActive(true);

                EventSystem.current.SetSelectedGameObject(multiplayerButton);
                EventSystem.current.firstSelectedGameObject = multiplayerButton;

                break;*/
        }
    }
}
