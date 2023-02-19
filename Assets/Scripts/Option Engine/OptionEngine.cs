using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.EventSystems;

[BurstCompile]
public class OptionEngine : MonoBehaviour
{
    public InputEngine inputEngine = null;

    public TitleScreenEngine titleScreenEngine = null;
    public GraphicOptions graphicOptions = null;
    public BlockEngineOptions blockEngineOptions = null;

    public GameObject inGameCanvas = null;
    public GameObject canvas = null;

    public GameObject inGameFirstSelected = null;
    public GameObject firstSelected = null; // ui

    // Used to select the correct button to highlight when going back. (in game and on title screen)

    public GameObject graphicsButton = null;
    public GameObject blockEngineButton = null;
    public GameObject inGameGraphicsButton = null;
    public GameObject inGameBlockEngineButton = null;

    public enum Menu { Options, Graphics, BlockEngine }
    public Menu currentMenu = Menu.Options;

    public bool menuOpen = false;
    public bool inGame = false;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        inputEngine = null;

        titleScreenEngine = null;
        graphicOptions = null;
        blockEngineOptions = null;

        inGameCanvas = null;
        canvas = null;
        inGameFirstSelected = null;
        firstSelected = null;

        graphicsButton = null;
        blockEngineButton = null;
        inGameGraphicsButton = null;
        inGameBlockEngineButton = null;
    }

    [BurstCompile]
    void Awake()
    {
        inputEngine = GameObject.Find("Input Engine").GetComponent<InputEngine>();

        titleScreenEngine = GameObject.Find("Title Screen Engine").GetComponent<TitleScreenEngine>();
        graphicOptions = GetComponent<GraphicOptions>();
        blockEngineOptions = GetComponent<BlockEngineOptions>();
    }

    // Update is called once per frame
    [BurstCompile]
    void Update()
    {
        if (inGame == true)
        {
            // IN GAME menu button pressed
            if (inputEngine.menu)
            {
                // cancel input
                inputEngine.menu = false;
                menuOpen = !menuOpen;   // toogle

                if (menuOpen)
                {
                    switch (currentMenu)
                    {
                        case Menu.Options:

                            inGameCanvas.SetActive(true);

                            currentMenu = Menu.Options;
                            EventSystem.current.SetSelectedGameObject(null);
                            EventSystem.current.firstSelectedGameObject = inGameFirstSelected;
                            EventSystem.current.SetSelectedGameObject(inGameFirstSelected);
                            break;
                    }
                }
                else
                {
                    switch (currentMenu)
                    {
                        case Menu.Options:
                            inGameCanvas.SetActive(false);
                            break;

                        case Menu.Graphics:
                            graphicOptions.DisableCanvas();
                            break;

                        case Menu.BlockEngine:
                            blockEngineOptions.DisableCanvas();
                            break;
                    }

                    currentMenu = Menu.Options;
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.firstSelectedGameObject = inGameFirstSelected;
                    EventSystem.current.SetSelectedGameObject(inGameFirstSelected);
                }
            }

            if (menuOpen
                && inputEngine.boost)
            {
                inputEngine.boost = false;

                switch (currentMenu)
                {
                    case Menu.Options:
                                                
                        //close menu
                        inGameCanvas.SetActive(false);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.firstSelectedGameObject = inGameFirstSelected;
                        EventSystem.current.SetSelectedGameObject(inGameFirstSelected);

                        menuOpen = false;

                        break;

                    case Menu.Graphics:

                        graphicOptions.DisableCanvas();
                        inGameCanvas.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.firstSelectedGameObject = inGameGraphicsButton;
                        EventSystem.current.SetSelectedGameObject(inGameGraphicsButton);

                        currentMenu = Menu.Options;
                        break;

                    case Menu.BlockEngine:

                        blockEngineOptions.DisableCanvas();
                        inGameCanvas.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.firstSelectedGameObject = inGameBlockEngineButton;
                        EventSystem.current.SetSelectedGameObject(inGameBlockEngineButton);

                        currentMenu = Menu.Options;
                        break;
                }
            }
        }
        else // Not in game
        {
            if (inputEngine.boost)
            {
                // if we are on the options menu then back should return us to the title screen.
                // Logic for that is located in TitleScreenEngine.cs
                if (currentMenu == Menu.Options) { return; }

                inputEngine.boost = false;

                switch (currentMenu)
                {
                    case Menu.Graphics:

                        graphicOptions.DisableCanvas();
                        canvas.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.firstSelectedGameObject = graphicsButton;
                        EventSystem.current.SetSelectedGameObject(graphicsButton);

                        currentMenu = Menu.Options;
                        break;

                    case Menu.BlockEngine:

                        blockEngineOptions.DisableCanvas();
                        canvas.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.firstSelectedGameObject = blockEngineButton;
                        EventSystem.current.SetSelectedGameObject(blockEngineButton);

                        currentMenu = Menu.Options;
                        break;
                }
            }
        }
    }

    [BurstCompile]
    public void ClickBack()
    {
        if (inGame == false)
        {
            // Do nothing. Different canvas - Different button logic
            //inGameCanvas.SetActive(false);

            canvas.SetActive(false);

            titleScreenEngine.titleCanvas.SetActive(true);

            // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.firstSelectedGameObject = titleScreenEngine.optionsButton;
            EventSystem.current.SetSelectedGameObject(titleScreenEngine.optionsButton);

            titleScreenEngine.currentMenu = TitleScreenEngine.Menu.Title;
        }
    }

    [BurstCompile]
    public void ClickBackGraphicOptions()
    {
        graphicOptions.canvas.SetActive(false);

        if (inGame)
        {
            inGameCanvas.SetActive(true);

            // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.firstSelectedGameObject = inGameGraphicsButton;
            EventSystem.current.SetSelectedGameObject(inGameGraphicsButton);
        }
        else
        {
            canvas.SetActive(true);

            // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.firstSelectedGameObject = graphicsButton;
            EventSystem.current.SetSelectedGameObject(graphicsButton);
        }

        currentMenu = Menu.Options;
    }

    [BurstCompile]
    public void ClickBackBlockEngineOptions()
    {
        blockEngineOptions.canvas.SetActive(false);

        if (inGame)
        {
            inGameCanvas.SetActive(true);

            // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.firstSelectedGameObject = inGameBlockEngineButton;
            EventSystem.current.SetSelectedGameObject(inGameBlockEngineButton);
        }
        else
        {
            canvas.SetActive(true);

            // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.firstSelectedGameObject = blockEngineButton;
            EventSystem.current.SetSelectedGameObject(blockEngineButton);
        }

        currentMenu = Menu.Options;
    }

    [BurstCompile]
    public void SelectGraphicOptions() 
    {
        if (inGame)
        {
            inGameCanvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(false);
        }

        graphicOptions.EnableCanvas();

        // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.firstSelectedGameObject = graphicOptions.firstSelected;
        EventSystem.current.SetSelectedGameObject(graphicOptions.firstSelected);

        currentMenu = Menu.Graphics;
    }

    [BurstCompile]
    public void SelectBlockEngineOptions()
    {
        if (inGame)
        {
            inGameCanvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(false);
        }

        blockEngineOptions.EnableCanvas();

        // User interface management - when its be clicked deselect it so it does not stay "lit" (visuals)
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.firstSelectedGameObject = blockEngineOptions.firstSelected;
        EventSystem.current.SetSelectedGameObject(blockEngineOptions.firstSelected);

        currentMenu = Menu.BlockEngine;
    }
}
