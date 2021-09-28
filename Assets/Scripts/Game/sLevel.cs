//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class sLevel : MonoBehaviour
//{

//    public enum Directions
//    {
//        left,
//        right,
//        forward,
//        backward,
//        startPush,
//        endPush,
//        none
//    }
//    public static Directions FlipDirection(sLevel.Directions dir)
//    {
//        switch (dir)
//        {
//            case Directions.left:
//                dir = Directions.right;
//                break;
//            case Directions.right:
//                dir = Directions.left;
//                break;
//            case Directions.forward:
//                dir = Directions.backward;
//                break;
//            case Directions.backward:
//                dir = Directions.forward;
//                break;
//            default:
//                break;
//        }
//        return dir;
//    }

//    [Serializable]
//    public class Column
//    {
//        public List<Tile> row;
//    }
//    [Serializable]
//    public class Tile
//    {
//        public GameObject tile;
//    }

//    [SerializeField] public List<Column> columns;
//    [SerializeField] List<GameObject> switchTiles = new List<GameObject>(); //switches in this level. List is populated in Init
//    [Serializable]
//    public struct Coordinates //adapted from (Kloster & Kemp, 2013)
//    {
//        public int x;
//        public int y;

//        public static bool operator ==(Coordinates c1, Coordinates c2)
//        {
//            if (c1.x == c2.x && c1.y == c2.y)
//            {
//                return true;
//            }
//            return false;
//        }

//        public static bool operator !=(Coordinates c1, Coordinates c2)
//        {
//            if (c1.x == c2.x && c1.y == c2.y)
//            {
//                return false;
//            }
//            return true;
//        }
//    }

//    public Coordinates startTile;
//    public Coordinates endTile;

//    [HideInInspector] public int movesComletedIn;
//    [HideInInspector] public bool completed;


//    private void Start()
//    {
//        //Init();
//    }
//    public void Init()
//    {
//        switchTiles.Clear();
//        for (int i = 0; i < columns.Count; i++)
//        {
//            for (int j = 0; j < columns[i].row.Count; j++)
//            {
//                Tile tempItem = columns[i].row[j];
//                GameObject tempTile = columns[i].row[j].tile;
//                sLevelManager.Tile tileData = new sLevelManager.Tile();
//                if (!tempTile.name.Contains("["))
//                {
//                    tempTile = Instantiate(columns[i].row[j].tile, transform) as GameObject; //this is spawning in prefab empty tiles
//                    tempItem.tile = tempTile;
//                    columns[i].row[j] = tempItem;
//                    tempTile.name = "Tile [" + i + "," + j + "]";

//                }
//                if (tempTile.GetComponent<sTile>().tileType == tileTypes.startTile)
//                {
//                    GetTile(startTile).GetComponent<sTile>().tileType = tileTypes.floorTile;
//                    startTile.x = i;
//                    startTile.y = j;
//                }
//                if (tempTile.GetComponent<sTile>().tileType == tileTypes.endTile)
//                {
//                    GetTile(startTile).GetComponent<sTile>().tileType = tileTypes.floorTile;
//                    endTile.x = i;
//                    endTile.y = j;
//                }
//                if (tempTile.GetComponent<sTile>().tileType == tileTypes.switchTile && !switchTiles.Contains(tempTile))
//                {
//                    switchTiles.Add(tempTile);
//                }
//                tempTile.GetComponent<sTile>().Init(new Vector3(i, 0, j));
//            }
//        }
//    }

//    public static Coordinates GetNewCoordinates(Coordinates curLoc, Directions dir)
//    {

//        switch (dir)
//        {
//            case Directions.left:
//                curLoc.x -= 1;
//                break;
//            case Directions.right:
//                curLoc.x += 1;
//                break;
//            case Directions.forward:
//                curLoc.y += 1;
//                break;
//            case Directions.backward:
//                curLoc.y -= 1;
//                break;
//            case Directions.none:
//                break;
//            default:
//                break;
//        }
//        return curLoc;
//    }

//    public GameObject GetTile(Coordinates loc)
//    {
//        if (loc.x < 0 || loc.x >= columns.Count || loc.y < 0 || loc.y >= columns[0].row.Count)
//        {
//            return null;
//        }
//        return columns[loc.x].row[loc.y].tile;
//    }

//    public GameObject GetNextTile(Directions dir, Coordinates curLoc, out Coordinates output)
//    {
//        output = curLoc;
//        Coordinates newLoc = curLoc;
//        switch (dir)
//        {
//            case Directions.left:
//                newLoc.x = newLoc.x - 1;
//                if (newLoc.x < 0 || newLoc.x > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    output = newLoc;
//                    return GetTile(newLoc);
//                }
//                return null;
//            case Directions.right:
//                newLoc.x = newLoc.x + 1;
//                if (newLoc.x < 0 || newLoc.x > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    output = newLoc;
//                    return GetTile(newLoc);
//                }
//                return null;
//            case Directions.forward:
//                newLoc.y = newLoc.y + 1;
//                if (newLoc.y < 0 || newLoc.y > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    output = newLoc;
//                    return GetTile(newLoc);
//                }
//                return null;
//            case Directions.backward:
//                newLoc.y = newLoc.y - 1;
//                if (newLoc.y < 0 || newLoc.y > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    output = newLoc;
//                    return GetTile(newLoc);
//                }
//                Debug.Log(GetTile(newLoc));
//                return null;
//            default:
//                break;
//        }
//        Debug.Log("Direction invalid");
//        return null;
//    }
//    public GameObject GetNextTile(Directions dir, Coordinates curLoc)
//    {
//        Coordinates newLoc = curLoc;

//        switch (dir)
//        {
//            case Directions.left:
//                newLoc.x = newLoc.x - 1;
//                if (newLoc.x < 0 || newLoc.x > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    return GetTile(newLoc);
//                }
//                return null;
//            case Directions.right:
//                newLoc.x = newLoc.x + 1;
//                if (newLoc.x < 0 || newLoc.x > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    return GetTile(newLoc);
//                }
//                return null;
//            case Directions.forward:
//                newLoc.y = newLoc.y + 1;
//                if (newLoc.y < 0 || newLoc.y > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    return GetTile(newLoc);
//                }
//                return null;
//            case Directions.backward:
//                newLoc.y = newLoc.y - 1;
//                if (newLoc.y < 0 || newLoc.y > columns.Count - 1)
//                {
//                    return null;
//                }
//                if (GetTile(newLoc) != null)
//                {
//                    return GetTile(newLoc);
//                }
//                Debug.Log(GetTile(newLoc));
//                return null;
//            default:
//                break;
//        }
//        Debug.Log("Direction invalid");
//        return null;
//    }

//    //returns true if all switches are turned on
//    public bool IsAllSwitchOn()
//    {
//        bool isAllOn = true;
//        if (switchTiles.Count > 0)
//        { 
//            for (int i = 0; i < switchTiles.Count; i++)
//            {
//                if (!switchTiles[i].GetComponent<sTile>().switchOn)
//                {
//                    isAllOn = false;
//                }
//            }
//        }
//        return isAllOn;
//    }
//}
