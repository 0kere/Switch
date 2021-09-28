//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.IO;

//public class sLevelManager : MonoBehaviour //Level data handler
//{
//    private static sLevelManager lm;
//    public static sLevelManager instance
//    {
//        get
//        {
//            if (!lm)
//            {
//                lm = FindObjectOfType(typeof(sLevelManager)) as sLevelManager;
//                if (!lm)
//                {
//                    Debug.LogError("Game Manager required in scene.");
//                }
//                else
//                {
//                    instance.Init();
//                }
//            }
//            return lm;
//        }
//    }
//    [System.Serializable]
//    public class Level
//    {
//        public Queue<Tile> tileData; //1 queue to hold all tile data
//        public bool complete;
//        public int movesCompleted;
//        public sLevel.Coordinates startPos;
//    }
//    [System.Serializable]
//    public class Tile
//    {
//        public tileTypes type;
//        public bool switchOn;
//        public sLevel.Directions pushDirection;
//    }

//    //Queue<Tile> tileData = new Queue<Tile>();//queue of tile data to hold levels object data to be saved then reloaded on start up
//    [SerializeField] private GameObject levelTemplate;
//    [SerializeField] private GameObject tileTemplate;

//    [SerializeField] public static List<Level> defaultLevels = new List<Level>();
//    public static List<Level> customLevels = new List<Level>();

//    public static List<Level> TranslateLevelsObjectToData(List<GameObject> objebctList)
//    {
//        List<Level> levelsData = new List<Level>();
//        //PrintTileData(objebctList);
//        for (int i = 0; i < objebctList.Count; i++)
//        {
//            Level tempLevel = new Level();
//            levelsData.Add(tempLevel);

//            sLevel curLevelRef = objebctList[i].GetComponent<sLevel>();
//            PrintTileData(curLevelRef);
//            Queue<Tile> tileData = new Queue<Tile>();
//            for (int j = 0; j < curLevelRef.columns.Count; j++)
//            {
//                for (int k = 0; k < curLevelRef.columns[j].row.Count; k++)
//                {

//                    Tile data = new Tile();
//                    sTile tileRef = curLevelRef.columns[j].row[k].tile.GetComponent<sTile>();
//                    data.pushDirection = tileRef.pushDirection;
//                    data.switchOn = tileRef.switchOn;
//                    data.type = tileRef.tileType;
//                    Debug.Log(tileRef.tileType.ToString());
//                    tileData.Enqueue(data);
//                }
//            }
//            levelsData[i].tileData = tileData;
//            levelsData[i].complete = curLevelRef.completed;
//            levelsData[i].movesCompleted = curLevelRef.movesComletedIn;
//            levelsData[i].startPos = curLevelRef.startTile;
//        }
//        return levelsData;
//    }
//    public static List<GameObject> TranslateLevelsDataToObject(List<Level> dataList) 
//    {
//        List<GameObject> levelsObject = new List<GameObject>();
//        for (int i = 0; i < dataList.Count; i++)
//        {
//            GameObject newLevel = Instantiate(sLevelManager.instance.levelTemplate) as GameObject;
//            newLevel.transform.position = new Vector3(1000, 0, 0);
//            newLevel.name = "Level " + i;
//            sLevel newLevelRef = newLevel.GetComponent<sLevel>();
//            newLevelRef.startTile = dataList[i].startPos;
//            newLevelRef.movesComletedIn = dataList[i].movesCompleted;
//            newLevelRef.completed = dataList[i].complete;
//            Debug.Log(dataList[i].startPos.x.ToString() + ", " + dataList[i].startPos.y.ToString());
//            for (int j = 0; j < newLevelRef.columns.Count; j++)
//            {
//                for (int k = 0; k < newLevelRef.columns[j].row.Count; k++)
//                {
//                    GameObject newTile = Instantiate(sLevelManager.instance.tileTemplate) as GameObject;
//                    newTile.transform.parent = newLevel.transform;
//                    newTile.transform.localPosition = Vector3.zero;
//                    newTile.name = "Tile [" + j + "," + k + "]";
//                    newLevelRef.columns[j].row[k].tile = newTile;
//                    Tile data = dataList[i].tileData.Dequeue();
//                    //Debug.Log(data.type.ToString());
//                    newLevelRef.columns[j].row[k].tile.GetComponent<sTile>().tileType = data.type;
//                    newLevelRef.columns[j].row[k].tile.GetComponent<sTile>().switchOn = data.switchOn;
//                    newLevelRef.columns[j].row[k].tile.GetComponent<sTile>().pushDirection = data.pushDirection;
//                    Debug.Log(newLevelRef.columns[j].row[k].tile.GetComponent<sTile>().tileType + " data type: " + data.type.ToString() + " index: " + j + "," + k + " Level: " + i);//this comes out with corrent data types   
//                    //somehow the tiles get overriden on the next loop. think somehow the level init is setting it all to empty
//                }
//            }
//            levelsObject.Add(newLevel);
//            //Debug.Log("2, 1: " + newLevel.GetComponent<sLevel>().columns[2].row[2].tile.GetComponent<sTile>().tileType.ToString()); //comes through empty
//        }
//        PrintTileData(levelsObject); //by the time it gets to here the final tile element in the last level will overite all other tiles beause it is enditing one tile entity
//        return levelsObject;
//    }

//    //debug function
//    private static void PrintTileData(List<Level> data)
//    {
//        foreach (Level item in data)
//        {
//        Debug.Log(item.tileData.Count);
//            foreach (Tile tile in item.tileData)
//            {
//                Debug.Log("Tile type" +tile.type.ToString());
//            }
//        }
//    }

//    private static void PrintTileData(List<GameObject> data)
//    {
//        for (int i = 0; i < data.Count; i++)
//        {
//            for (int j = 0; j < data[i].GetComponent<sLevel>().columns.Count; j++)
//            {
//                for (int k = 0; k < data[i].GetComponent<sLevel>().columns[j].row.Count; k++)
//                {
//                    Debug.Log(data[i].GetComponent<sLevel>().columns[j].row[k].tile.GetComponent<sTile>().tileType.ToString());
//                }
//            }
//        }
//    }

//    private static void PrintTileData(sLevel data)
//    {
//        for (int i = 0; i < data.columns.Count; i++)
//        {
//            for (int j = 0; j < data.columns[i].row.Count; j++)
//            {
//                Debug.Log(data.columns[i].row[j].tile.GetComponent<sTile>().tileType.ToString());
//            }
//        }
//    }


//    private void Init()
//    {

//    }


//    public void UpdateLevel(bool isDefaultLevel, int levelIndex, bool isCompleted, int movesMade)
//    {
//        if (isDefaultLevel)
//        {
//            defaultLevels[levelIndex].complete = isCompleted;
//            defaultLevels[levelIndex].movesCompleted = movesMade;
//        }
//        else
//        {
//            customLevels[levelIndex].complete = isCompleted;
//            customLevels[levelIndex].movesCompleted = movesMade;
//        }
//    }

//    public static void SaveDefaultLevel() //saves default levels from game manager 
//    {
//        BinaryFormatter bf = new BinaryFormatter();
//        FileStream file = File.Create(Application.persistentDataPath + "/save.data");
//        List<Level> savedata = TranslateLevelsObjectToData(GameManger.instance.levels);
//        //PrintTileData(savedata);
//        bf.Serialize(file, savedata);
//        file.Close();
//        Debug.Log("saved at: " + Application.persistentDataPath);
//    }

//    public static void SaveCustomLevel()
//    { 

//    }

//    public static void LoadLevelData() //loads level data from s ave file directiley to default level var in game manager
//    {
//        BinaryFormatter bf = new BinaryFormatter();
//        FileStream file = File.Open(Application.persistentDataPath + "/save.data", FileMode.Open);
//        defaultLevels = (List<Level>)bf.Deserialize(file);
//        GameManger.instance.levels = TranslateLevelsDataToObject(defaultLevels);
//        file.Close();
//    }

//    public bool LoadDefaultLevel(int index)
//    {
//        //instantiate the default level at the given index and set moves made and if completed - level data should already known
//        return false;
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        //List<GameObject> defLevels = GameManger.instance.levels;
//        //for (int i = 0; i < defLevels.Count; i++)
//        //{
//        //    Level tempLevel = new Level();
//        //    tempLevel.levels = defLevels[i];
//        //    defaultLevels.Add(tempLevel);
//        //}
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.P))
//        {
//            SaveDefaultLevel();
//        }
//        if (Input.GetKeyDown(KeyCode.O))
//        {
//            PrintTileData(defaultLevels);
//        }
//    }
//}
