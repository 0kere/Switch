using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager um;
    public static UIManager instance
    {
        get
        {
            if (!um)
            {
                um = FindObjectOfType(typeof(UIManager)) as UIManager;
                if (!um)
                {
                    Debug.LogError("UI Manager required in scene.");
                }
                else
                {
                    instance.Init();
                }
            }
            return um;
        }
    }

    [SerializeField] private GameObject gameMenuObject;
    [SerializeField] private GameObject levelCompleteMenuObject;

    [SerializeField] private sLevelCompleteMenu levelComepleteRef;

    private bool isLevelCompleteActive;

    [SerializeField] private GameObject inGameMenuObject;
    [SerializeField] private GameObject nextLevelButton;

    [SerializeField] private TMPro.TextMeshProUGUI movesMadeText;

    public void Init()
    { 

    }

    public void ResetButtonClicked()
    {
        GameManger.instance.ResetLevel(isLevelCompleteActive);
        if (isLevelCompleteActive) { ToggleLevelCompleteMenu(0); }
    }

    public void UndoButtonClicked()
    {
        GameManger.instance.playerRef.undoMove = true;
        GameManger.instance.playerRef.UndoMove();
    }

    public void MenuButtonClicked()
    {
        //open menu panel
        ToggleGameMenu();
    }

    public void NextLevelClicked()
    {
        StartCoroutine(GameManger.instance.QueueLevelStart());
    }

    public void SettingsButtonClicked()
    { 

    }

    public void ReturnToMenu()
    {
        //return to main menu scene
        //sLevelManagerRefactor.SaveDefaultLevel(); //should save custom levels if currently playing custom levels why woukd u save this here
        GameManger.instance.isLevelEditing = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);//load menu scene
    }

    public void ToggleGameMenu()
    {
        gameMenuObject.SetActive(!gameMenuObject.activeSelf);
    }

    public void ToggleLevelCompleteMenu(float movesMade)
    {
        if (isLevelCompleteActive)
        {
            levelComepleteRef.DisableMenu();
        }
        else
        {
            levelComepleteRef.EnableMenu(movesMade);
            UpdateNextLevelButton();
        }
        isLevelCompleteActive = !isLevelCompleteActive;
    }

    public void UpdateNextLevelButton()
    {
        if ((sLevelManagerRefactor.instance.defaultLevels.Count == GameManger.instance.curLevelIndex && !GameManger.instance.isLevelEditing) || (sLevelManagerRefactor.instance.customLevels.Count == GameManger.instance.curLevelIndex && !GameManger.instance.isPlayingDefaultLevels && !GameManger.instance.isLevelEditing))//curLevelIndex is incremented before this runs
        {
            nextLevelButton.SetActive(false); //there are no more levels
        }
        else if (!GameManger.instance.isLevelEditing)
        {
            nextLevelButton.SetActive(true);
        }
    }

    public void DisableLevelCompleteMenu()
    {
        levelComepleteRef.DisableMenu();
        isLevelCompleteActive = false;
    }

    public void SetActiveInGameMenu(bool active)
    {
        inGameMenuObject.SetActive(active);
    }

    public void UpdateMovesMade(float movesMade)
    {
        if (!GameManger.instance.isLevelEditing)
            movesMadeText.text = "Moves Made: " + movesMade.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleGameMenu();
        }
    }

    private void Start()
    {
        SetActiveInGameMenu(false);
    }
}
