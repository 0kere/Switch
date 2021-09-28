using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public enum Directions
//{
//    left,
//    right,
//    forward,
//    backward
//}
[Serializable]
public class sGrid : MonoBehaviour
{
    [SerializeField] private GameObject exTile; //example tile to take width and height from 
    [Serializable]
    public struct Coordinates
    {
        public int x;
        public int y;
    }
    [Serializable]
    public struct Column
    {
        public List<Row> rows;
    }
    [Serializable]
    public struct Row
    {
        public GameObject tile;
    }
    [SerializeField] private List<Column> Grid;

    private float tileWidth;
    private float tileHeight;
    [SerializeField] private float tilePadding = 0.0f; //Space between tiles if any
    public Coordinates startLoc;

    // Start is called before the first frame update
    void Start()
    {
        tileHeight = exTile.transform.localScale.z;
        tileWidth = exTile.transform.localScale.x;
        Vector3 tileLoc = Vector3.zero;
        for (int i = 0; i < Grid.Count; i++)
        {
            for (int j = 0; j < Grid[i].rows.Count; j++)
            {
                if (Grid[i].rows[j].tile != null)
                { 
                    GameObject temp = Instantiate(Grid[i].rows[j].tile, tileLoc, Quaternion.identity);
                    Row tempRow = Grid[i].rows[j];
                    tempRow.tile = temp;
                    Grid[i].rows[j] = tempRow;
                }
                tileLoc.x += tileWidth + tilePadding;
            }
            tileLoc.x = 0f;
            tileLoc.z +=  tileHeight + tilePadding;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public Coordinates GetNextTile(Directions dir, Coordinates curLoc)
    //{
    //    switch (dir)
    //    {
    //        case Directions.left:
    //            curLoc.x -= 1;
    //            return curLoc;
    //        case Directions.right:
    //            curLoc.x += 1;
    //            return curLoc;
    //        case Directions.forward:
    //            curLoc.y += 1;
    //            return curLoc;
    //        case Directions.backward:
    //            curLoc.y -= 1;
    //            return curLoc;
    //        default:
    //            break;
    //    }
    //    Debug.Log("Direction invalid");
    //    return curLoc;
    //}
    //public bool GetNextTile(Directions dir, Coordinates curLoc, out Coordinates output)
    //{
    //    output = curLoc;
    //    Coordinates newLoc = curLoc;
    //    switch (dir)
    //    {
    //        case Directions.left:
    //            newLoc.x -= 1;
    //            if (Grid[newLoc.x].rows[newLoc.y].tile != null)
    //            {
    //                output = newLoc;
    //                return true;
    //            }
    //            return false;
    //        case Directions.right:
    //            newLoc.x += 1;
    //            if (GetTile(newLoc) != null)
    //            {
    //                output = newLoc;
    //                return true;
    //            }
    //            return false;
    //        case Directions.forward:
    //            newLoc.y += 1;
    //            if (GetTile(newLoc) != null)
    //            {
    //                output = newLoc;
    //                return true;
    //            }
    //            return false;
    //        case Directions.backward:
    //            newLoc.y -= 1;
    //            if (GetTile(newLoc) != null)
    //            {
    //                output = newLoc;
    //                return true;
    //            }
    //            Debug.Log(GetTile(newLoc));
    //            return false;
    //        default:
    //            break;
    //    }
    //    Debug.Log("Direction invalid");
    //    return false;
    //}

    public GameObject GetTile(Coordinates loc)
    {
        Debug.Log(Grid[loc.x].rows[loc.y].tile);
        Debug.Log(loc.x + ", " + loc.y);
        return Grid[loc.x].rows[loc.y].tile;
    }

   
}
