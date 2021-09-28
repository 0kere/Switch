using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tileTypes
{
    empty,
    floorTile,
    switchTile,
    pushTile,
    startTile,
    endTile
}

public class sTile : MonoBehaviour
{
    [SerializeField] private MeshRenderer rend;

    public tileTypes tileType = tileTypes.empty;
    public bool switchOn = false;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material emptyMaterial;
    [SerializeField] private Material startTileMaterial;
    [SerializeField] private Material endTileMaterial;

    [SerializeField] private sDirectionalArrows directionArrowRef;

    private bool lateSwitch; //set to true when switch is landed on due to an undo move, flips the switch when you come off it instead of landing on it

    public sLevelRefactor.Directions pushDirection; //Direction the tile pushes the player in

    public sLevelRefactor.Coordinates coords;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
    }

    public void Init(Vector3 location, Tile tileData)
    {
        //assign tile state
        tileType = tileData.type;
        pushDirection = tileData.pushDirection;
        switchOn = tileData.isSwitchOn;
        transform.position = location;
        rend.enabled = true;
        directionArrowRef.gameObject.SetActive(true);
        coords.x = (int)location.x;
        coords.y = (int)location.z;
        directionArrowRef.Init(coords);
        switch (tileType)
        {
            case tileTypes.empty:
                rend.material = emptyMaterial;
                directionArrowRef.gameObject.SetActive(false);
                break;
            case tileTypes.floorTile:
                rend.material = tileMaterial;
                break;
            case tileTypes.switchTile:
                if (switchOn)
                {
                    rend.material = onMaterial;
                }
                else
                { 
                    rend.material = offMaterial;
                }
                break;
            case tileTypes.startTile:
                rend.material = startTileMaterial;
                break;
            case tileTypes.endTile:
                rend.material = endTileMaterial;
                break;
            case tileTypes.pushTile:
                rend.material = tileMaterial;
                break;
            default:
                break;
        }
    }

    public void LandedOn(sLevelRefactor.Directions lastMoveDir)
    {
        directionArrowRef.SpreadArrows(lastMoveDir);
        if (tileType == tileTypes.switchTile && !GameManger.instance.playerRef.undoMove)
        {
            FlipSwitch();
        }
    }

    public void TileLeft()
    {
        if (tileType == tileTypes.switchTile && GameManger.instance.playerRef.undoMove)
        {
            FlipSwitch();
        }
        directionArrowRef.UnspreadArrows();
    }

    private void FlipSwitch()
    {
        //flip da switch coede here 
        switchOn = switchOn ? false : true;
        if (switchOn)
        {
            rend.material = onMaterial;
        }
        else
        {
            rend.material = offMaterial;
        }
    }

    private void UpdateDirectionArrow()
    {
        if (tileType != tileTypes.empty)
        { 
            directionArrowRef.gameObject.SetActive(true);
        }
        directionArrowRef.NormalInit(coords);
    }

    private void UpdateSurroundingDirectionArrows()
    {
        sLevelRefactor.Coordinates up;
        sLevelRefactor.Coordinates down;
        sLevelRefactor.Coordinates left;
        sLevelRefactor.Coordinates right;
        UpdateDirectionArrow();
        if (GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.forward, coords, out up))
            GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.forward, coords, out up).GetComponent<sTile>().UpdateDirectionArrow();

        if (GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.backward, coords, out down))
            GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.backward, coords, out down).GetComponent<sTile>().UpdateDirectionArrow();

        if (GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.left, coords, out left))
            GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.left, coords, out left).GetComponent<sTile>().UpdateDirectionArrow();

        if (GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.right, coords, out right))
            GameManger.instance.curLevelRef.GetNextTile(sLevelRefactor.Directions.right, coords, out right).GetComponent<sTile>().UpdateDirectionArrow();
    }

    public void UpdateTile(tileTypes newType, bool isOn, sLevelRefactor.Directions pushDir)
    {
        //assign tile state
        tileType = newType;
        pushDirection = pushDir;
        switchOn = isOn;

        switch (tileType)
        {
            case tileTypes.empty:
                rend.material = emptyMaterial;
                directionArrowRef.gameObject.SetActive(false);
                break;
            case tileTypes.floorTile:
                rend.material = tileMaterial;
                break;
            case tileTypes.switchTile:
                if (switchOn)
                {
                    rend.material = onMaterial;
                }
                else
                {
                    rend.material = offMaterial;
                }
                break;
            case tileTypes.startTile:
                rend.material = startTileMaterial;
                sLevelRefactor.Coordinates oldStart = GameManger.instance.curLevelRef.startCoords;
                GameManger.instance.curLevelRef.startCoords = coords;

                if (GameManger.instance.curLevelRef.GetTile(oldStart).GetComponent<sTile>().tileType != tileTypes.empty)
                    GameManger.instance.curLevelRef.GetTile(oldStart).GetComponent<sTile>().UpdateTile(tileTypes.floorTile, false, sLevelRefactor.Directions.none);
                else
                    GameManger.instance.curLevelRef.GetTile(oldStart).GetComponent<sTile>().UpdateTile(tileTypes.empty, false, sLevelRefactor.Directions.none);

                break;
            case tileTypes.endTile:
                sLevelRefactor.Coordinates oldEnd = GameManger.instance.curLevelRef.endCoords;
                GameManger.instance.curLevelRef.endCoords = coords;
                if (GameManger.instance.curLevelRef.GetTile(oldEnd).GetComponent<sTile>().tileType != tileTypes.empty)
                    GameManger.instance.curLevelRef.GetTile(oldEnd).GetComponent<sTile>().UpdateTile(tileTypes.floorTile, false, sLevelRefactor.Directions.none);
                else
                    GameManger.instance.curLevelRef.GetTile(oldEnd).GetComponent<sTile>().UpdateTile(tileTypes.empty, false, sLevelRefactor.Directions.none);

                rend.material = endTileMaterial;
                break;
            case tileTypes.pushTile:
                rend.material = tileMaterial;
                break;
            default:
                break;
        }

        UpdateSurroundingDirectionArrows();

        //Update LevelData
        sLevelManagerRefactor.instance.customLevels[GameManger.instance.curLevelIndex].levelInfo.columns[coords.x].rows[coords.y].type = tileType;
        sLevelManagerRefactor.instance.customLevels[GameManger.instance.curLevelIndex].levelInfo.columns[coords.x].rows[coords.y].pushDirection = pushDirection;
        sLevelManagerRefactor.instance.customLevels[GameManger.instance.curLevelIndex].levelInfo.columns[coords.x].rows[coords.y].isSwitchOn = isOn;
    }
}
