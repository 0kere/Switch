using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    private static GameManger gm;
    public static GameManger instance
    {
        get
        {
            return gm;
        }
    }

    [SerializeField] public List<GameObject> levels;
    public sLevelRefactor curLevelRef;
    [HideInInspector] public GameObject curLevel;
    public int curLevelIndex;
    [SerializeField] private float delayOnLevelCompletion;
    [SerializeField] private GameObject player;
    private GameObject playerObj;
    [HideInInspector] public sPlayerController playerRef;
    private float playerYValue = 0.375f; //Y value player needs to be spawned out
    [HideInInspector] public bool levelComplete;
    [SerializeField, HideInInspector] public sCameraMovement camMovement;
    [HideInInspector] public IEnumerator cameraStartRoutine;
    [HideInInspector] public IEnumerator cameraEndRoutine;
    private IEnumerator resetRoutine;
    private IEnumerator queueLevelStartRoutine;
    [HideInInspector] public bool isPlayingDefaultLevels = true;
    public bool isLevelEditing;

    UnityEngine.Events.UnityAction<Scene, Scene> startLevelAction;

    public LevelEditorManager levelEdManager;

    private bool inGame;
    public float playerSpeed = 250; //held to be updated through seetings
    public void UpdateSpeed(float newSpeed) { playerSpeed = newSpeed; if (inGame) { playerRef.UpdateSpeed(newSpeed); } }

    [HideInInspector] public sTutorial tutorialRef;

    private void Init()
    { 
        
    }
public void StartGame(int index, bool isDefault) //pass in index of the level to spawn level selctor will correspond to level and next level will use start level only
    {
        curLevelIndex = index;
        SceneManager.activeSceneChanged += startLevelAction;
        isPlayingDefaultLevels = isDefault;
        if (!isLevelEditing)
        {
            SceneManager.LoadScene(1);
        }
        
    }
    public void StartLevel(bool isComplete, int index)//this is default only so yno fix that pls
    {
        levelComplete = false;
        if (SceneManager.GetActiveScene().buildIndex == 0) { return; } //dont start the level when in main menu
        if (camMovement is null) { camMovement = Camera.main.gameObject.GetComponent<sCameraMovement>(); }
        if (cameraStartRoutine is null)
        {
            if (isPlayingDefaultLevels)
            {
                SpawnLevel(index);
            }
            else
            {
                SpawnCustomLevel(index);
            }
            if (!isLevelEditing)
            { 
                cameraStartRoutine = camMovement.LevelStartMove(isComplete);
                StartCoroutine(cameraStartRoutine);
            }
        }
        SceneManager.activeSceneChanged -= startLevelAction;
    }

    private void SpawnLevel(int index)
    {
        if (tutorialRef is object)
        { 
            tutorialRef.NewLevelLoaded();
        }
        Destroy(curLevel);
        Destroy(playerObj);
        curLevel = sLevelManagerRefactor.instance.LoadDefaultLevel(index);
        curLevelRef = curLevel.GetComponent<sLevelRefactor>();
        curLevelRef.Init(true, sLevelManagerRefactor.instance.defaultLevels[index]);
        playerObj = Instantiate(player, new Vector3(curLevelRef.startCoords.x, playerYValue, curLevelRef.startCoords.y), Quaternion.identity);
        playerRef = playerObj.GetComponent<sPlayerController>();
    }

    private void SpawnCustomLevel(int index)
    {
        Destroy(curLevel);
        Destroy(playerObj);
        curLevel = sLevelManagerRefactor.instance.LoadCustomLevel(index);
        curLevelRef = curLevel.GetComponent<sLevelRefactor>();
        curLevelRef.Init(true, sLevelManagerRefactor.instance.customLevels[index]);
        SpawnPlayer();
    }

    public void LevelComplete(int movesMade)
    {
        //transition to next level
        if (isLevelEditing)
        {
            //open level editor specific wondow
            levelComplete = true;
            levelEdManager.LevelComplete(movesMade);
        }
        else
        { 
            if (cameraEndRoutine is null) 
            {
                levelComplete = true;
                sLevelManagerRefactor.instance.UpdateLevel(isPlayingDefaultLevels, curLevelIndex, true, movesMade);
                curLevelRef.thisLevel.isCompleted = true;
                curLevelIndex++;

                cameraEndRoutine = camMovement.LevelEndMove(delayOnLevelCompletion, movesMade, true);
                StartCoroutine(cameraEndRoutine);
            }
        }

    }

    public void SpawnPlayer()
    {
        playerObj = Instantiate(player, new Vector3(curLevelRef.startCoords.x, playerYValue, curLevelRef.startCoords.y), Quaternion.identity);
        playerRef = playerObj.GetComponent<sPlayerController>();
    }

    public void DestroyPlayer()
    {
        if (playerObj is object)
        { 
            Destroy(playerObj);
        }
    }

    public IEnumerator QueueLevelStart() //called when next level clicked
    {
        while (cameraEndRoutine is object)
        {
            yield return null;
        }
        queueLevelStartRoutine = null;
        StartLevel(true, curLevelIndex);
    }

    public void ResetLevel(bool isCompleteMenu)
    {
        if (resetRoutine is null && cameraStartRoutine is null && cameraEndRoutine is null)
        {
            resetRoutine = Reset(isCompleteMenu);
            StartCoroutine(resetRoutine);
        }
    }

    private IEnumerator Reset(bool isCompleteMenu)
    {
        if (!isLevelEditing)
        { 
            if (!isCompleteMenu)
            {
                cameraEndRoutine = camMovement.LevelEndMove(0f, 0, false);
                StartCoroutine(cameraEndRoutine);
            }
            else
            {
                curLevelIndex--;
            }
            while (cameraEndRoutine is object)
            {
                yield return null;
            }
        }
        Destroy(curLevel);
        Destroy(playerObj);
        if (!isLevelEditing)
        { 
            cameraStartRoutine = camMovement.LevelStartMove(false);
            StartCoroutine(cameraStartRoutine);
        }
        if (isPlayingDefaultLevels)
        {
            SpawnLevel(curLevelIndex);
        }
        else if (!isPlayingDefaultLevels && !isLevelEditing)
        {
            SpawnCustomLevel(curLevelIndex);
        }
        else if (isLevelEditing)
        {
            levelEdManager.SpawnLevel(curLevelIndex);
            SpawnPlayer();
        }
        resetRoutine = null;
    }

    private void Awake()
    {
        if (gm is object && gm != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gm = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        sLevelManagerRefactor.LoadDefaultLevelData();
        sLevelManagerRefactor.LoadCustomLevelData();
        camMovement = Camera.main.GetComponent<sCameraMovement>();
        startLevelAction = delegate { StartLevel(false, curLevelIndex); };
    }
    void Update()
    {

    }

    #region Level Editor

    public void StartLevelEditor(int index)
    {
        curLevelIndex = index;
        isLevelEditing = true;
        isPlayingDefaultLevels = false;
        SceneManager.LoadScene(2);
    }

    public void StartEditorGame(int index)
    {
        curLevelIndex = index;
        StartGame(curLevelIndex, false);
    }
    #endregion
}
