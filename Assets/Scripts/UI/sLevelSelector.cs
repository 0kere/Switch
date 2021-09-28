using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sLevelSelector : MonoBehaviour //ad;apted from 
{
    public GameObject levelHolder;
    public GameObject levelIcon;
    public GameObject addNewIcon;
    public GameObject thisCanvas;
    public int numberOfLevels = 50;
    public Vector2 iconSpacing;
    private Rect panelDimensions;
    private Rect iconDimensions;
    private int amountPerPage;
    private int currentLevelCount;
    [SerializeField] private bool isDefaultSelector;

    private GameObject lastPanel; //store parent object for new icon

    private List<GameObject> cusutomIcons = new List<GameObject>();//needed to support deleting levels from the menu

    // Start is called before the first frame update
    void Start()
    {
        if (isDefaultSelector) { numberOfLevels = sLevelManagerRefactor.instance.defaultLevels.Count; }
        else { numberOfLevels = sLevelManagerRefactor.instance.customLevels.Count; }
        panelDimensions = levelHolder.GetComponent<RectTransform>().rect;
        iconDimensions = levelIcon.GetComponent<RectTransform>().rect;
        int maxInARow = Mathf.FloorToInt((panelDimensions.width + iconSpacing.x) / (iconDimensions.width + iconSpacing.x));
        int maxInACol = Mathf.FloorToInt((panelDimensions.height + iconSpacing.y) / (iconDimensions.height + iconSpacing.y));
        amountPerPage = maxInARow * maxInACol;
        int totalPages = Mathf.CeilToInt((float)numberOfLevels / amountPerPage);
        LoadPanels(totalPages);
    }
    void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(levelHolder) as GameObject;

        for (int i = 1; i <= numberOfPanels; i++)
        {
            GameObject panel = Instantiate(panelClone) as GameObject;
            lastPanel = panel;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(levelHolder.transform);
            panel.name = "Page-" + i;
            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
            SetUpGrid(panel);
            int numberOfIcons = i == numberOfPanels ? numberOfLevels - currentLevelCount : amountPerPage;
            LoadIcons(numberOfIcons, panel);
        }
        //Add an extra button on the end of custom levels to add new
        if (!isDefaultSelector) { SetUpAddNewIcon(lastPanel); }
        Destroy(panelClone);
    }
    void SetUpGrid(GameObject panel)
    {
        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.spacing = iconSpacing;
    }
    void LoadIcons(int numberOfIcons, GameObject parentObject)
    {
        for (int i = 1; i <= numberOfIcons; i++)
        {
            currentLevelCount++;
            bool iconCompleted = SetUpIcon(numberOfIcons, parentObject, i);
            if (!iconCompleted && isDefaultSelector) { break; } //only break for default levels as users will want access to all custom levels
        }
    }
    //returns if the level the icon represents is true so that the icon set up can be stopped on the last unlocked level
    private bool SetUpIcon(int numberOfIcons, GameObject parentObject, int index)
    {
        if (!isDefaultSelector) { return SetUpEditorIcon(numberOfIcons, parentObject, index); }
        GameObject icon = Instantiate(levelIcon) as GameObject;
        icon.transform.SetParent(thisCanvas.transform, false);
        icon.transform.SetParent(parentObject.transform);
        icon.name = "Level " + index;

        Level thisLevel = sLevelManagerRefactor.instance.defaultLevels[index - 1];

        icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Level " + currentLevelCount);
        icon.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("Par " + thisLevel.par);

        icon.GetComponent<Button>().onClick.AddListener(delegate { LevelPressed(index, true); });
        bool isCompleted = thisLevel.isCompleted;
        icon.transform.GetChild(1).gameObject.SetActive(isCompleted);
        //icon.transform.GetChild(2).gameObject.SetActive(isCompleted);
        if (isCompleted)
        {
            //icon.GetComponentInChildren<Toggle>().isOn = isCompleted;
            icon.transform.GetChild(2).gameObject.SetActive(true);
            icon.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Moves Made: " + thisLevel.movesMade; //set moves made text if level has been completed
        }
        return isCompleted;
    }

    //end of adapted code

    private bool SetUpEditorIcon(int numberOfIcons, GameObject parentObject, int index) //wont have moves made or completed. If level hasnt been completed the levelpressed event should not be bound
    {//name should be set to the indexed levels name. Needs an edit button and a delete button to be added
        GameObject icon = Instantiate(levelIcon) as GameObject;
        cusutomIcons.Add(icon);
        icon.transform.SetParent(thisCanvas.transform, false);
        icon.transform.SetParent(parentObject.transform);
        Level thisLevel = sLevelManagerRefactor.instance.customLevels[index - 1];

        icon.name = thisLevel.name;
        icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(thisLevel.name);

        bool isCompleted = thisLevel.isCompleted;
        if (isCompleted)
        {
            icon.GetComponent<Button>().onClick.AddListener(delegate { LevelPressed(index, false); });
            icon.transform.GetChild(1).gameObject.SetActive(true);
            icon.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("Par " + thisLevel.par);
        }
        else if (thisLevel.movesMade != 0)
        {
            icon.transform.GetChild(2).gameObject.SetActive(true);
            icon.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Moves Made: " + thisLevel.movesMade; //set moves made text if level has been completed
        }
        else
        {
            icon.transform.GetChild(0).GetComponent<sOnHoverText>().disableHover = true;
        }
        icon.transform.GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { EditPressed(index); });
        icon.transform.GetChild(3).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { DeletePressed(index); });
        return isCompleted;
    }

    private void SetUpAddNewIcon(GameObject parentObj)
    {

        if (parentObj is null)
        {
            GameObject panel = Instantiate(levelHolder) as GameObject;
            lastPanel = panel;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(levelHolder.transform);
            panel.name = "Page-" + 1;
            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (1 - 1), 0);
            SetUpGrid(panel);
            parentObj = panel;
        }
        GameObject icon = Instantiate(addNewIcon) as GameObject;
        icon.transform.SetParent(thisCanvas.transform, false);
        icon.transform.SetParent(parentObj.transform, false);
        icon.GetComponent<Button>().onClick.AddListener(delegate { NewLevelPressed(); });
    }

    private void LevelPressed(int index, bool isDefualt)
    {
        GameManger.instance.StartGame(index-1, isDefualt);
    }

    private void EditPressed(int index)
    {
        GameManger.instance.isLevelEditing = true;
        GameManger.instance.StartLevelEditor(index - 1);
    }

    private void DeletePressed(int index)
    {
        sLevelManagerRefactor.DeleteCustomLevel(index - 1);
        Destroy(cusutomIcons[index - 1]);
        cusutomIcons.RemoveAt(index - 1);
    }

    private void NewLevelPressed()
    {
        sLevelManagerRefactor.AddBlankCustomLevel();
        GameManger.instance.StartLevelEditor(sLevelManagerRefactor.instance.customLevels.Count - 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

