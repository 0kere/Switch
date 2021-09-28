using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Level
{
    public Column levelInfo;
    public bool isCompleted;
    public int movesMade;
    public string name;
    public int par;
}
[System.Serializable]
public class Column
{
    public List<Row> columns;
}
[System.Serializable]
public class Row
{
    public List<Tile> rows;
}
[System.Serializable]
public class Tile
{
    public tileTypes type;
    public bool isSwitchOn;
    public sLevelRefactor.Directions pushDirection;
}


public class sLevelRefactor : MonoBehaviour
{
    [SerializeField] private List<List<GameObject>> tileObjects = new List<List<GameObject>>();
    //private List<GameObject> rowObject;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] List<GameObject> switchTiles = new List<GameObject>(); //switches in this level. List is populated in Init
    bool firstInit = true;
    [System.Serializable]
    public struct Coordinates //adapted from (Kloster & Kemp, 2013)
    {
        public int x;
        public int y;
        public Coordinates(int x_, int y_) { x = x_; y = y_; }
        public static bool operator ==(Coordinates c1, Coordinates c2)
        {
            if (c1.x == c2.x && c1.y == c2.y)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Coordinates c1, Coordinates c2)
        {
            if (c1.x == c2.x && c1.y == c2.y)
            {
                return false;
            }
            return true;
        }
    }
    //end of adapted code
    public Coordinates startCoords;
    public Coordinates endCoords;
    public enum Directions
    {
            left,
            right,
            forward,
            backward,
            startPush,
            endPush,
            none
    }
    public static Directions FlipDirection(Directions dir)
    {
        switch (dir)
        {
            case Directions.left:
                dir = Directions.right;
                break;
            case Directions.right:
                dir = Directions.left;
                break;
            case Directions.forward:
                dir = Directions.backward;
                break;
            case Directions.backward:
                dir = Directions.forward;
                break;
            default:
                break;
        }
        return dir;
    }
    public Level thisLevel = new Level();

    /// <summary>
    /// To be used to load level info. For custom levels this will also need to load tile data
    /// </summary>
    public void Init(bool isDefault, Level levelData)
    {
        thisLevel = levelData;
        if (isDefault)
        {
            thisLevel.isCompleted = levelData.isCompleted;
            thisLevel.movesMade = levelData.movesMade;
        }
        else
        {
            //custom level init
            thisLevel = levelData;
        }

        //initialise tileObject list
        if (firstInit)
        { 
            for (int i = 0; i < thisLevel.levelInfo.columns.Count; i++)
            {
                tileObjects.Add(new List<GameObject>());
                for (int j = 0; j < thisLevel.levelInfo.columns[i].rows.Count; j++)
                {
                    GameObject tempTile = new GameObject();
                    tempTile.name = "Temp Tile";
                    tileObjects[i].Add(tempTile);
                }
            }
        }
        firstInit = false;
        //GameObject tileClone = Instantiate(tilePrefab) as GameObject;
        for (int i = 0; i < thisLevel.levelInfo.columns.Count; i++)
        {
            for (int j = 0; j < thisLevel.levelInfo.columns[i].rows.Count; j++)
            {
                Coordinates thisCoords = new Coordinates(i,j);
                GameObject tempTile = Instantiate(tilePrefab, new Vector3(thisCoords.x, 0, thisCoords.y), Quaternion.identity);
                tempTile.transform.parent = this.gameObject.transform;
                tempTile.GetComponent<sTile>().Init(new Vector3(i, 0, j), thisLevel.levelInfo.columns[i].rows[j]);
                tempTile.name = "Tile [" + i + ", " + j + "]";
                switch (thisLevel.levelInfo.columns[i].rows[j].type)
                {
                    case tileTypes.empty:
                        break;
                    case tileTypes.floorTile:
                        break;
                    case tileTypes.switchTile:
                        if (!switchTiles.Contains(tempTile)) { switchTiles.Add(tempTile); }
                        break;
                    case tileTypes.startTile:
                        startCoords = thisCoords;
                        break;
                    case tileTypes.endTile:
                        endCoords = thisCoords;
                        break;
                    case tileTypes.pushTile:
                        break;
                    default:
                        break;
                }
                Destroy(tileObjects[i][j]);
                tileObjects[i][j] = tempTile;
            }
        }
        //Destroy(tileClone);
    }

    public static Coordinates GetNewCoordinates(Coordinates curLoc, Directions dir)
    {

        switch (dir)
        {
            case Directions.left:
                curLoc.x -= 1;
                break;
            case Directions.right:
                curLoc.x += 1;
                break;
            case Directions.forward:
                curLoc.y += 1;
                break;
            case Directions.backward:
                curLoc.y -= 1;
                break;
            case Directions.none:
                break;
            default:
                break;
        }
        return curLoc;
    }
    public GameObject GetTile(Coordinates loc)
    {
        if (loc.x < 0 || loc.x >= thisLevel.levelInfo.columns.Count || loc.y < 0 || loc.y >= thisLevel.levelInfo.columns[0].rows.Count)
        {
            return null;
        }
        return tileObjects[loc.x][loc.y];
    }

    public GameObject GetNextTile(Directions dir, Coordinates curLoc, out Coordinates output)
    {
        output = curLoc;
        Coordinates newLoc = curLoc;
        switch (dir)
        {
            case Directions.left:
                newLoc.x = newLoc.x - 1;
                if (newLoc.x < 0 || newLoc.x > thisLevel.levelInfo.columns.Count - 1)
                {
                    return null;
                }
                if (GetTile(newLoc) != null)
                {
                    output = newLoc;
                    return GetTile(newLoc);
                }
                return null;
            case Directions.right:
                newLoc.x = newLoc.x + 1;
                if (newLoc.x < 0 || newLoc.x > thisLevel.levelInfo.columns.Count - 1)
                {
                    return null;
                }
                if (GetTile(newLoc) != null)
                {
                    output = newLoc;
                    return GetTile(newLoc);
                }
                return null;
            case Directions.forward:
                newLoc.y = newLoc.y + 1;
                if (newLoc.y < 0 || newLoc.y > thisLevel.levelInfo.columns.Count - 1)
                {
                    return null;
                }
                if (GetTile(newLoc) != null)
                {
                    output = newLoc;
                    return GetTile(newLoc);
                }
                return null;
            case Directions.backward:
                newLoc.y = newLoc.y - 1;
                if (newLoc.y < 0 || newLoc.y > thisLevel.levelInfo.columns.Count - 1)
                {
                    return null;
                }
                if (GetTile(newLoc) != null)
                {
                    output = newLoc;
                    return GetTile(newLoc);
                }
                return null;
            default:
                break;
        }
        return null;
    }

    //returns true if all switches are turned on
    public bool IsAllSwitchOn()
    {
        bool isAllOn = true;
        if (switchTiles.Count > 0)
        {
            for (int i = 0; i < switchTiles.Count; i++)
            {
                if (switchTiles[i])
                { 
                    if (!switchTiles[i].GetComponent<sTile>().switchOn)
                    {
                        isAllOn = false;
                    }
                }
            }
        }
        return isAllOn;
    }
}
