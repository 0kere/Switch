using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPaintBrush : MonoBehaviour
{
    public class StrokeInfo //Stores each stroke (dragging with lmb down)
    {
        public List<PaintData> updates = new List<PaintData>();
        public void AddDataPoint(sTile tileRef, tileTypes newTileType, tileTypes prevTileType, bool isOn, bool prevOn, 
            sLevelRefactor.Directions pushDir_, sLevelRefactor.Directions pushDirPrev) 
        {
            updates.Add(new PaintData(tileRef, prevTileType, isOn, prevOn, pushDir_, pushDirPrev));
            tileRef.UpdateTile(newTileType, isOn, pushDir_);
        }
        public void ResetStroke()
        {
            for (int i = 0; i < updates.Count; i++) { updates[i].Reset(); }
        }
    }
    public class PaintData //Stores indivdual updates
    {
        public PaintData(sTile tileRef, tileTypes tileType, bool isOn, bool prevOn, sLevelRefactor.Directions pushDir_, 
            sLevelRefactor.Directions pushPrev_) 
        { 
            updateTile = tileRef;
            previousTileType = tileType; 
            prevSwitchOn = prevOn;
            pushDir = pushDir_;
            pushPrev = pushPrev_;
        }
        public sTile updateTile;
        public tileTypes previousTileType;
        public bool prevSwitchOn;
        public sLevelRefactor.Directions pushDir;
        public sLevelRefactor.Directions pushPrev;
        public void Reset() { updateTile.UpdateTile(previousTileType, prevSwitchOn, pushPrev); }
    }

    [HideInInspector] public sTile prevStart; //Holds the original start and end tiles when they are updated so they can be reset if the move is undone
    sTile prevEnd;

    private List<StrokeInfo> strokesMade = new List<StrokeInfo>();
    private StrokeInfo newStroke;
    private tileTypes brushType = tileTypes.empty;

    private bool leftMouseDown;
    private bool rightMouseDown;

    private LevelEditorManager levelEd;

    [SerializeField] private List<sOnHoverText> brushTexts;

    public void UpdateBrushType(int newBrushType)
    {
        brushTexts[(int)brushType].ToggleSelected();
        brushType = (tileTypes)newBrushType;
        brushTexts[newBrushType].ToggleSelected();
    }

    private void StrokeBegan()
    {
        newStroke = new StrokeInfo();
    }

    private void StrokeOver()
    {
        strokesMade.Add(newStroke);
    }

    private void CheckBrushHit()
    {
        //raycast from mouse point to world and check if tile type is the same as brush type if not then update the tile and add a data point to strokes
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<sTile>())
            {
                sTile tile = hit.transform.gameObject.GetComponent<sTile>();
                if (tile.tileType == brushType) { return; }
                if (brushType == tileTypes.startTile)//store original tile before updating start and end tile
                {
                    if (GameManger.instance.curLevelRef.GetTile(GameManger.instance.curLevelRef.startCoords).GetComponent<sTile>().tileType == tileTypes.startTile)
                    {
                        prevStart = GameManger.instance.curLevelRef.GetTile(GameManger.instance.curLevelRef.startCoords).GetComponent<sTile>();
                    }
                }
                if (brushType == tileTypes.endTile)
                {
                    if (GameManger.instance.curLevelRef.GetTile(GameManger.instance.curLevelRef.endCoords).GetComponent<sTile>().tileType == tileTypes.endTile)
                    {
                        prevEnd = GameManger.instance.curLevelRef.GetTile(GameManger.instance.curLevelRef.endCoords).GetComponent<sTile>();
                    }
                }

                if (brushType != tileTypes.pushTile)
                {
                    newStroke.AddDataPoint(tile, brushType, tile.tileType, tile.switchOn, tile.switchOn, tile.pushDirection, tile.pushDirection);
                }
                else if (brushType == tileTypes.pushTile)
                {
                    //default push direction: left
                    newStroke.AddDataPoint(tile, brushType, tile.tileType, tile.switchOn, tile.switchOn, sLevelRefactor.Directions.left, tile.pushDirection); 
                }
            }
        }
    }

    private void CheckAlternateHit()
    {
        //raycast from mouse point to world and chedk if is switch or push and then update them accordingly
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<sTile>())
            {
                sTile tile = hit.transform.gameObject.GetComponent<sTile>();
                if (tile.tileType == tileTypes.pushTile)
                {
                    newStroke.AddDataPoint(tile, tile.tileType, tile.tileType, tile.switchOn, tile.switchOn, GetNextDirection(tile.pushDirection), tile.pushDirection);
                }
                else if (tile.tileType == tileTypes.switchTile)
                {
                    newStroke.AddDataPoint(tile, tile.tileType, tile.tileType, !tile.switchOn, tile.switchOn, tile.pushDirection, tile.pushDirection);
                }
            }
        }
    }

    private sLevelRefactor.Directions GetNextDirection(sLevelRefactor.Directions dir)
    {
        switch (dir)
        {
            case sLevelRefactor.Directions.left:
                return sLevelRefactor.Directions.forward;
            case sLevelRefactor.Directions.right:
                return sLevelRefactor.Directions.backward;
            case sLevelRefactor.Directions.forward:
                return sLevelRefactor.Directions.right;
            case sLevelRefactor.Directions.backward:
                return sLevelRefactor.Directions.left;
            default:
                return sLevelRefactor.Directions.left;
        }
    }

    public void UndoLastStroke()
    {
        if (strokesMade.Count != 0)
        {
            for (int i = 0; i < strokesMade[strokesMade.Count-1].updates.Count; i++)
            {
                if (strokesMade[strokesMade.Count - 1].updates[i].updateTile.tileType == tileTypes.endTile) //if end tile is undone then we need to reset the previous end tile
                {
                    prevEnd.UpdateTile(tileTypes.endTile, prevEnd.switchOn, prevEnd.pushDirection);
                }
                if (strokesMade[strokesMade.Count - 1].updates[i].updateTile.tileType == tileTypes.startTile) //if end tile is undone then we need to reset the previous end tile
                {
                    prevStart.UpdateTile(tileTypes.startTile, prevStart.switchOn, prevStart.pushDirection);
                }
            }
            
            

            strokesMade[strokesMade.Count - 1].ResetStroke();
            strokesMade.RemoveAt(strokesMade.Count - 1);
        }
    }

    private void Start()
    {
        levelEd = GetComponentInParent<LevelEditorManager>();
        brushTexts[(int)brushType].ToggleSelected();
        prevEnd = GameManger.instance.curLevelRef.GetTile(GameManger.instance.curLevelRef.endCoords).GetComponent<sTile>();
        prevStart = GameManger.instance.curLevelRef.GetTile(GameManger.instance.curLevelRef.startCoords).GetComponent<sTile>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !rightMouseDown)
        {
            if (!leftMouseDown)
            {
                leftMouseDown = true;
                StrokeBegan();
            }
            CheckBrushHit();
        }
        else if (Input.GetMouseButtonDown(1) && !leftMouseDown)
        {
            if (!rightMouseDown)
            {
                rightMouseDown = true;
                StrokeBegan();
            }
            CheckAlternateHit();
        }
        else if (Input.GetMouseButtonUp(0) && leftMouseDown)
        {
            leftMouseDown = false;
            StrokeOver();
        }
        else if (Input.GetMouseButtonUp(1) && rightMouseDown)
        {
            rightMouseDown = false;
            StrokeOver();
        }

        if (leftMouseDown)
        {
            CheckBrushHit();
        }

        if (!levelEd.isTyping)
        { 
            if (Input.GetKeyDown(KeyCode.R) && levelEd.isEditing)
            {
                UndoLastStroke();
            }

            #region Numerical KeyBinds for Updating Brush Type
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UpdateBrushType((int)tileTypes.empty);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UpdateBrushType((int)tileTypes.floorTile);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UpdateBrushType((int)tileTypes.switchTile);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                UpdateBrushType((int)tileTypes.pushTile);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                UpdateBrushType((int)tileTypes.startTile);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                UpdateBrushType((int)tileTypes.endTile);
            }
            #endregion
        }
    }

}
