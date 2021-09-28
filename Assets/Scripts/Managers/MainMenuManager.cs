using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;   
    [SerializeField] private GameObject playMenu;   
    [SerializeField] private GameObject settingsMenu;   
    [SerializeField] private GameObject defaultLevelSelectorMenu;
    [SerializeField] private GameObject customLevelSelectorMenu;

    private GameObject currentMenu;
    private GameObject previousMenu;

    public void PlayButtonPressed()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        previousMenu = currentMenu;
        currentMenu = playMenu;
    }

    public void SettingsButtonPressed()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        previousMenu = currentMenu;
        currentMenu = settingsMenu;
    }

    public void LevelsButtonPressed()
    {
        //bring up the level selector for default levels
        currentMenu.SetActive(false);
        defaultLevelSelectorMenu.SetActive(true);
        previousMenu = currentMenu;
        currentMenu = defaultLevelSelectorMenu;
    }


    public void CustomLevelsButtonPressed()
    {
        currentMenu.SetActive(false);
        customLevelSelectorMenu.SetActive(true);
        previousMenu = currentMenu;
        currentMenu = customLevelSelectorMenu;
    }

    public void ReturnToPreviousMenu()
    {
        //to be called on any back button or when esc is pressed
        if (currentMenu == mainMenu) { return; } //cant go back on the first menu
        else if (currentMenu == customLevelSelectorMenu || currentMenu == defaultLevelSelectorMenu)
        {
            //previous menu needs to be set to main menu as there is 2 menus to go through here
            currentMenu.SetActive(false);
            currentMenu = previousMenu;
            previousMenu = mainMenu;
            currentMenu.SetActive(true);

        }
        else
        {
            currentMenu.SetActive(false);
            previousMenu.SetActive(true);
            currentMenu = mainMenu;
        }
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }

    private void Start()
    {
        currentMenu = mainMenu;
        //need the settings menu to initialise on start up. this is the easiest way as all set up runs in the awake function
        settingsMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToPreviousMenu();
        }
    }
}
