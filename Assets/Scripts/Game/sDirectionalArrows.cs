using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DirectionArrow
{
    public GameObject arrow;
    public bool active;
    public sLevelRefactor.Directions direction;
    public Vector3 startPos;
}

public class sDirectionalArrows : MonoBehaviour
{

    [SerializeField] List<DirectionArrow> directionArrows = new List<DirectionArrow>();
    [SerializeField] private float distToMove;
    [SerializeField] private float spreadSpeed;
    [SerializeField] private float unspreadSpeedModifier;
    [SerializeField] private string canMoveHex;
    [SerializeField] private string cantMoveHex;
    [SerializeField] private string unspreadHex;
    [SerializeField] private string endTileHex;
    private bool arrowsSpread;
    
    private Vector3 startPos;
    private IEnumerator spreadRoutine;
    private IEnumerator endSpreadRoutine;
    private sLevelRefactor.Coordinates thisCoords;

    private bool levelComplete; //local level complete bool to handle end tile color switching


    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        if (GameManger.instance.curLevelRef.startCoords == thisCoords)
        {
            SpreadArrows(sLevelRefactor.Directions.none);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(sLevelRefactor.Coordinates location)
    {
        StartCoroutine(LateInit(location));
    }

    private IEnumerator LateInit(sLevelRefactor.Coordinates location)
    {
        yield return 0;
        NormalInit(location);
    }

    public void NormalInit(sLevelRefactor.Coordinates location)
    {
        Color newCol;
        thisCoords = location;
        for (int i = 0; i < directionArrows.Count; i++)
        {
            if (ColorUtility.TryParseHtmlString('#' + endTileHex, out newCol) && thisCoords == GameManger.instance.curLevelRef.endCoords)
            {
                directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
            }
            else if (ColorUtility.TryParseHtmlString('#' + canMoveHex, out newCol) && thisCoords == GameManger.instance.curLevelRef.startCoords)
            {
                directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
            }
            else if (ColorUtility.TryParseHtmlString('#' + unspreadHex, out newCol))
            {
                directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
            }
            //directionArrows[i].arrow.SetActive(ShouldBeOn(directionArrows[i].direction, location));
            directionArrows[i].active = ShouldBeOn(directionArrows[i].direction, location, i);
            directionArrows[i].startPos = directionArrows[i].arrow.transform.position;
        }
        sTile thisTile = GameManger.instance.curLevelRef.GetTile(location).GetComponent<sTile>();
    }

    private bool ShouldBeOn(sLevelRefactor.Directions dir, sLevelRefactor.Coordinates location, int index)
    {
        bool shouldBeOn = false;
        GameObject tile = GameManger.instance.curLevelRef.GetTile(location);
        sTile thisTile = tile.GetComponent<sTile>();
        SpriteRenderer thisSRend = directionArrows[index].arrow.GetComponent<SpriteRenderer>();
        if (thisTile.tileType == tileTypes.pushTile && dir != thisTile.pushDirection)
        {
            shouldBeOn = false;
            thisSRend.enabled = false;
        }
        else if (thisSRend.enabled == false && thisTile.tileType != tileTypes.empty)
        {
            thisSRend.enabled = true;
        }
        switch (dir)
        {
            case sLevelRefactor.Directions.left:
                location.x -= 1;
                tile = GameManger.instance.curLevelRef.GetTile(location);
                if (tile != null)
                { 
                    if (isValidTile(tile.GetComponent<sTile>()))
                    {
                        shouldBeOn = true;
                    }
                }
                break;
            case sLevelRefactor.Directions.right:
                location.x += 1;
                tile = GameManger.instance.curLevelRef.GetTile(location);
                if (tile != null)
                {
                    if (isValidTile(tile.GetComponent<sTile>()))
                    {
                        shouldBeOn = true;
                    }
                }
                break;
            case sLevelRefactor.Directions.forward:
                location.y += 1;
                tile = GameManger.instance.curLevelRef.GetTile(location);
                if (tile != null)
                {
                    if (isValidTile(tile.GetComponent<sTile>()))
                    {
                        shouldBeOn = true;
                    }
                }
                break;
            case sLevelRefactor.Directions.backward:
                location.y -= 1;
                tile = GameManger.instance.curLevelRef.GetTile(location);
                if (tile != null)
                {
                    if (isValidTile(tile.GetComponent<sTile>()))
                    {
                        shouldBeOn = true;
                    }
                }
                break;
            default:
                break;
        }
        return shouldBeOn;
    }

    private bool isValidTile(sTile thisTile)
    {
        if (thisTile.tileType == tileTypes.endTile || thisTile.tileType == tileTypes.floorTile || thisTile.tileType == tileTypes.startTile || thisTile.tileType == tileTypes.switchTile || thisTile.tileType == tileTypes.pushTile)
        {
            return true;
        }
        return false;
    }

    public void SpreadArrows(sLevelRefactor.Directions curMoveDirection)
    {
        if (thisCoords == GameManger.instance.curLevelRef.endCoords)
        {
            arrowsSpread = true;
            if (endSpreadRoutine is object)
            {
                StopCoroutine(endSpreadRoutine);
            }
            endSpreadRoutine = LerpEndTile();
            StartCoroutine(endSpreadRoutine);
            SetArrowColor(sLevelRefactor.Directions.none);
        }
        else
        { 
            if (!arrowsSpread)
            { 
                arrowsSpread = true;
                SetArrowColor(curMoveDirection);
                if (spreadRoutine is object)
                {
                    StopCoroutine(spreadRoutine);
                }
                spreadRoutine = LerpToPosition(distToMove);
                StartCoroutine(spreadRoutine);
            }
        }
    }

    public void UnspreadArrows()
    {
        if (thisCoords == GameManger.instance.curLevelRef.endCoords)
        {
            arrowsSpread = false;
            if (endSpreadRoutine is object)
            {
                StopCoroutine(endSpreadRoutine);
            }
            endSpreadRoutine = LerpEndTile();
            StartCoroutine(endSpreadRoutine);
            SetArrowColor(sLevelRefactor.Directions.none);
        }
        else
        { 
            if (arrowsSpread)
            {
                arrowsSpread = false;
                SetArrowColor(sLevelRefactor.Directions.none);
                if (spreadRoutine is object)
                {
                    StopCoroutine(spreadRoutine);
                }
                spreadRoutine = LerpToPosition(0);
                StartCoroutine(spreadRoutine);
            }
        }
        
    }



    private void SetArrowColor(sLevelRefactor.Directions moveDir)
    {
        Color newCol;
        for (int i = 0; i < directionArrows.Count; i++)
        {
            if (thisCoords == GameManger.instance.curLevelRef.endCoords)
            {
                if (GameManger.instance.levelComplete)
                {
                    levelComplete = true;
                    if (ColorUtility.TryParseHtmlString('#' + canMoveHex, out newCol))
                    {
                        directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
                    }
                }
                else if (arrowsSpread)
                {
                    if (ColorUtility.TryParseHtmlString('#' + cantMoveHex, out newCol))
                    {
                        directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
                    }
                }
                else
                {
                    if (ColorUtility.TryParseHtmlString('#' + endTileHex, out newCol))
                    {
                        directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
                    }
                }
            }
            else
            { 
                if (!directionArrows[i].active)
                {
                    continue;
                }
                if (!arrowsSpread)
                {
                    if (thisCoords == GameManger.instance.curLevelRef.startCoords && ColorUtility.TryParseHtmlString('#' + canMoveHex, out newCol))
                    {
                        directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
                    }
                    else if (ColorUtility.TryParseHtmlString('#' + unspreadHex, out newCol))
                    {
                        directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
                    }
                }
                else
                {
                    if (moveDir == sLevelRefactor.Directions.startPush)
                    {
                        moveDir = GameManger.instance.playerRef.GetStartPushMoveDir();
                    }
                    if (directionArrows[i].direction == moveDir && !(thisCoords == GameManger.instance.curLevelRef.startCoords && !arrowsSpread)) // Checks wheter the arrow points towards the last movement direction of the player i.e points to the previous tile
                    {
                        if (ColorUtility.TryParseHtmlString('#' + cantMoveHex, out newCol))
                        {
                            directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
                        }
                    }
                    else
                    {
                        if (ColorUtility.TryParseHtmlString('#' + canMoveHex, out newCol))
                        { 
                            directionArrows[i].arrow.GetComponent<SpriteRenderer>().color = newCol;
                        }
                    }
                    
                }
            }

        }
    }

    private sLevelRefactor.Directions FlipDirection(sLevelRefactor.Directions dir)
    {
        switch (dir)    
        {
            case sLevelRefactor.Directions.left:
                dir = sLevelRefactor.Directions.right;
                break;
            case sLevelRefactor.Directions.right:
                dir = sLevelRefactor.Directions.left;
                break;
            case sLevelRefactor.Directions.forward:
                dir = sLevelRefactor.Directions.backward;
                break;
            case sLevelRefactor.Directions.backward:
                dir = sLevelRefactor.Directions.forward;
                break;
            default:
                break;
        }
        return dir;
    }

    private IEnumerator LerpToPosition(float dist)
    {
        List<DirectionArrow> toMoveArrows = new List<DirectionArrow>();
        List<Vector3> endPositions = new List<Vector3>();
        for (int i = 0; i < directionArrows.Count; i++)
        {
            if (directionArrows[i].active)
            {
                toMoveArrows.Add(directionArrows[i]);
                endPositions.Add(CalcEndPos(directionArrows[i].direction, directionArrows[i].startPos ,dist));
            }
        }
        float t = 0f;
        
        while (t <= 1f)
        {
            float mod = 1f;
            if (!arrowsSpread)
            {
                mod = unspreadSpeedModifier;
            }
            t += Time.deltaTime * spreadSpeed * mod;
            for (int i = 0; i < toMoveArrows.Count; i++)
            {
                toMoveArrows[i].arrow.transform.position = Vector3.Lerp(toMoveArrows[i].arrow.transform.position, endPositions[i], Mathf.Sin(t * Mathf.PI * 0.5f));
            }

            yield return null;
        }
        spreadRoutine = null;
    }

    private Vector3 CalcEndPos(sLevelRefactor.Directions dir, Vector3 startPos, float distMove)
    {
        switch (dir)
        {
            case sLevelRefactor.Directions.left:
                startPos -= transform.right * distMove;
                break;
            case sLevelRefactor.Directions.right:
                startPos += transform.right * distMove;
                break;
            case sLevelRefactor.Directions.forward:
                startPos += transform.up * distMove;
                break;
            case sLevelRefactor.Directions.backward:
                startPos -= transform.up * distMove;
                break;
            default:
                break;
        }
        return startPos;
    }

    private IEnumerator LerpEndTile() //Hard coded end tile behaviour to create a border around the player cube
    {
        float t = 0f;
        float mod = 1f;
        if (!arrowsSpread)
        {
            mod = unspreadSpeedModifier;
        }
        List<Vector3> newPos = new List<Vector3>();
        for (int i = 0; i < directionArrows.Count; i++)
        {
            int dirX = i == 0 || i == 2 ? -1 : 1;
            int dirY = i == 0 || i == 3 ? -1 : 1;
            newPos.Add(transform.position + new Vector3(0.13f * dirX, 0f, 0.13f * dirY));
        }
        while (t <= 1f)
        {
            t += Time.deltaTime * spreadSpeed * mod;
            for (int i = 0; i < directionArrows.Count; i++)
            {
                if (arrowsSpread)
                {
                    directionArrows[i].arrow.transform.position = Vector3.Lerp(directionArrows[i].arrow.transform.position, newPos[i], Mathf.Sin(t * Mathf.PI * 0.5f));
                    directionArrows[i].arrow.transform.rotation = Quaternion.Euler(90f, 0f, 45f);
                }
                else
                {
                    directionArrows[i].arrow.transform.position = Vector3.Lerp(directionArrows[i].arrow.transform.position, directionArrows[i].startPos, Mathf.Sin(t * Mathf.PI * 0.5f));
                    directionArrows[i].arrow.transform.rotation = Quaternion.Lerp(directionArrows[i].arrow.transform.rotation, Quaternion.Euler(90f, 0f, 0f), Mathf.Sin(t * Mathf.PI * 0.5f));
                }


            }
            yield return null;
        }
    }
}
