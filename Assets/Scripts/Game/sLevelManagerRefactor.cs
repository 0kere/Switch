using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class sLevelManagerRefactor : MonoBehaviour
{
    private static sLevelManagerRefactor lm;
    public static sLevelManagerRefactor instance
    {
        get
        {
            if (!lm)
            {
                lm = FindObjectOfType<sLevelManagerRefactor>();
                if (!lm)
                {
                    Debug.Log("There is no level manager in scene");
                }
                else
                {
                    instance.Init();
                }
            }
            return lm;
        }
    }

    private void Start()
    {

    }
    private void Init()
    { 

    }

    [SerializeField] public List<Level> defaultLevels = new List<Level>();
    [SerializeField] public List<Level> customLevels = new List<Level>();
    [SerializeField] private GameObject templateLevelPrefab;
    private GameObject curLevel;

    public void UpdateLevel(bool isDefaultLevel, int levelIndex, bool isCompleted, int movesMade)
    {
        if (isDefaultLevel)
        {
            defaultLevels[levelIndex].isCompleted = isCompleted;
            int moves = defaultLevels[levelIndex].movesMade;
            moves = moves == 0 ? 100000 : moves;
            defaultLevels[levelIndex].movesMade = Mathf.Min(movesMade, moves);
            SaveDefaultLevel();
        }
        else
        {
            customLevels[levelIndex].isCompleted = isCompleted;
            customLevels[levelIndex].movesMade = Mathf.Min(movesMade, customLevels[levelIndex].movesMade);
            SaveCustomLevel();
        }
    }

    public static void SaveDefaultLevel()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.data");
        bf.Serialize(file, sLevelManagerRefactor.instance.defaultLevels);
        file.Close();
        Debug.Log("saved at: " + Application.persistentDataPath);
    }

    public static void SaveCustomLevel()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/customLevels.data");
        bf.Serialize(file, sLevelManagerRefactor.instance.customLevels);
        file.Close();
        Debug.Log("saved at: " + Application.persistentDataPath);
    }

    public static void LoadDefaultLevelData() //loads level data from s ave file directiley to default level var in game manager
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (!File.Exists(Application.persistentDataPath + "/save.data"))
        {
            Debug.Log("No Save Data on Device");
            return;
        }
        FileStream file = File.Open(Application.persistentDataPath + "/save.data", FileMode.Open);
        List<Level> temp = (List<Level>)bf.Deserialize(file);
        for (int i = 0; i < temp.Count; i++)//dont apply level info to default levels
        {
            sLevelManagerRefactor.instance.defaultLevels[i].isCompleted = temp[i].isCompleted;
            sLevelManagerRefactor.instance.defaultLevels[i].movesMade = temp[i].movesMade;
        }
        //sLevelManagerRefactor.instance.defaultLevels = (List<Level>)bf.Deserialize(file);
        file.Close();
    }
    public static void LoadCustomLevelData() //loads level data from s ave file directiley to default level var in game manager
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (!File.Exists(Application.persistentDataPath + "/customLevels.data"))
        {
            Debug.Log("No Custom Levels Data on Device");
            return;
        }
        FileStream file = File.Open(Application.persistentDataPath + "/customLevels.data", FileMode.Open);
        sLevelManagerRefactor.instance.customLevels = (List<Level>)bf.Deserialize(file);
        file.Close();
    }

    public static void DeleteCustomLevel(int index)
    {
        sLevelManagerRefactor.instance.customLevels.RemoveAt(index);
        SaveCustomLevel();
    }

    public GameObject LoadDefaultLevel(int index)
    {
        if (defaultLevels.Count <= index) { return null; } // if index is out of range check
        //instantiate the default level at the given index and set moves made and if completed - level data should already known
        curLevel = Instantiate(templateLevelPrefab, Vector3.zero, Quaternion.identity);
        //curLevel.GetComponent<sLevelRefactor>().Init(true, defaultLevels[index]);//this is done in gm rn
        return curLevel;
    }

    public GameObject LoadCustomLevel(int index)
    {
        if (customLevels.Count <= index) { return null; } // if index is out of range check
        //instantiate the default level at the given index and set moves made and if completed - level data should already known
        curLevel = Instantiate(templateLevelPrefab, Vector3.zero, Quaternion.identity);
        //curLevel.GetComponent<sLevelRefactor>().Init(true, customLevels[index]);
        return curLevel;
    }

    public static void AddBlankCustomLevel()
    {
        Level blankLevel = new Level();
        blankLevel.levelInfo = new Column();
        blankLevel.levelInfo.columns = new List<Row>();
        blankLevel.isCompleted = false;
        blankLevel.name = "New Level";

        for (int i = 0; i < 5; i++)//populate the colums with rows of blank tiles
        {
            Row blankRow = new Row();
            blankRow.rows = new List<Tile>();
            for (int j = 0; j < 5; j++)//populate a row of 5 blank tiles
            {
                //create a default blank tile
                Tile blankTile = new Tile();
                blankTile.isSwitchOn = false;
                blankTile.pushDirection = sLevelRefactor.Directions.none;
                blankTile.type = tileTypes.empty;
                blankRow.rows.Add(blankTile);
            }

            blankLevel.levelInfo.columns.Add(blankRow);
        }
        sLevelManagerRefactor.instance.customLevels.Add(blankLevel);
    }
}
