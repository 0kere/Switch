using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEditorManager : MonoBehaviour
{
    private int editingLevelIndex;
    private sLevelRefactor curLevelRef;
    [HideInInspector] public GameObject curLevel;
    private sCameraMovement cam;
    [HideInInspector] public bool isEditing = true;
    [SerializeField] private GameObject paintBrush;

    [SerializeField] private GameObject editingMenuItems;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private GameObject levelCompleteWindow;
    [SerializeField] private sLevelCompleteMenu levelCompleteMenu;

    [HideInInspector] public bool isTyping; //Dont want to take inputs while player is typing the level name

    int movesMade;

    [SerializeField] private TextMeshProUGUI savedText;
    public void SpawnLevel(int index)
    {
        editingLevelIndex = index;
        curLevel = sLevelManagerRefactor.instance.LoadCustomLevel(index);
        GameManger.instance.curLevel = curLevel;
        curLevelRef = curLevel.GetComponent<sLevelRefactor>();
        GameManger.instance.curLevelRef = curLevelRef;
        curLevelRef.Init(true, sLevelManagerRefactor.instance.customLevels[index]);
    }

    public void EnterTestMode()
    {
        //spawn player
        editingMenuItems.SetActive(false);
        paintBrush.SetActive(false);
        isEditing = false;
        curLevelRef.Init(false, sLevelManagerRefactor.instance.customLevels[editingLevelIndex]);
        GameManger.instance.SpawnPlayer();
        cam.toTestingRoutine = cam.LevelEditorToTest();
        UIManager.instance.SetActiveInGameMenu(true);
        StartCoroutine(cam.toTestingRoutine);
    }

    public void EnterEditMode()
    {
        editingMenuItems.SetActive(true);
        paintBrush.SetActive(true);
        UIManager.instance.SetActiveInGameMenu(false);
        isEditing = true;
        GameManger.instance.DestroyPlayer();
        curLevelRef.Init(false, sLevelManagerRefactor.instance.customLevels[editingLevelIndex]);
        cam.toEditorRoutine = cam.LevelEditorFromTest();
        StartCoroutine(cam.toEditorRoutine);
    }

    public void ToggleMode()
    {
        if (cam.toEditorRoutine is null && cam.toTestingRoutine is null)
        { 
            if (isEditing)
            {
                EnterTestMode();
            }
            else
            {
                EnterEditMode();
            }
        }
    }

    public void LevelComplete(int moves)
    {
        movesMade = moves;
        levelCompleteWindow.SetActive(true);
        UIManager.instance.UpdateNextLevelButton();
        curLevelRef.thisLevel.isCompleted = true; //level will now be playable without entering edit mode
        StartCoroutine(cam.LevelComplete());
        UIManager.instance.SetActiveInGameMenu(false);
        levelCompleteMenu.EnableMenu(0);
    }


    public void ContinueEditing()
    {
        levelCompleteMenu.DisableMenu();
        levelCompleteWindow.SetActive(false);
        curLevelRef.thisLevel.isCompleted = false;//make sure they have to submit the level to be able to play it outside of edit mode
        curLevelRef.Init(false, sLevelManagerRefactor.instance.customLevels[editingLevelIndex]);
        SaveLevel();
        EnterEditMode();
    }

    public void SaveLevel() //saves the level in its current state
    {
        sLevelManagerRefactor.instance.UpdateLevel(false, editingLevelIndex, curLevelRef.thisLevel.isCompleted, 0);
        StartCoroutine(SaveLevelTextFade());
    }

    private IEnumerator SaveLevelTextFade()
    {
        float t = 0f;
        Color col = savedText.color;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            col.a = Mathf.Lerp(1, 0, t);
            savedText.color = col;

            yield return null;
        }
    }
    public void SubmitLevel() //saves the level and sets complete to true so the level can be played normally outside of edit/test mode
    {
        curLevelRef.thisLevel.par = movesMade;
        curLevelRef.thisLevel.isCompleted = true;
        sLevelManagerRefactor.instance.UpdateLevel(false, editingLevelIndex, curLevelRef.thisLevel.isCompleted, 0);
        UIManager.instance.ReturnToMenu();
    }

    public void UpdateName(string name)
    {
        curLevelRef.thisLevel.name = name;
    }

    private void Start()
    {
        cam = Camera.main.GetComponent<sCameraMovement>();
        SpawnLevel(GameManger.instance.curLevelIndex);
        nameField.text = curLevelRef.thisLevel.name;
        nameField.onValueChanged.AddListener(delegate { UpdateName(nameField.text); });
        GameManger.instance.levelEdManager = this;
    }

    private void Update()
    {
        isTyping = nameField.isFocused;
        if (Input.GetKeyDown(KeyCode.T) && !isTyping) //Toggle in and out of test mode
        {
            ToggleMode();
        }
    }
}
