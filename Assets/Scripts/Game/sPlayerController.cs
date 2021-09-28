using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class sPlayerController : MonoBehaviour
{
    [SerializeField] private List<sLevelRefactor.Directions> movesMade = new List<sLevelRefactor.Directions>(); //used for undo function
    [HideInInspector] public bool undoMove;
    [SerializeField] private Queue<sLevelRefactor.Directions> movementQueue = new Queue<sLevelRefactor.Directions>();
    private bool cancelQueue; //true when CancelMovement is pressed, will clear queue once movwement is over
    [SerializeField] private sLevelRefactor curLevel; 
    [SerializeField] private sLevelRefactor.Coordinates curGridLoc = new sLevelRefactor.Coordinates();
    [SerializeField] private sLevelRefactor.Coordinates prevGridLoc = new sLevelRefactor.Coordinates();

    private IEnumerator movementRoutine;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float pushSpeedMod;
    [SerializeField] private float undoSpeedMod;
    [SerializeField] private int nRollsPerTile = 4; //how many times the cube needs to rotate to travel to the center of the next tile
    private float startYLevel;

    [HideInInspector] public bool isPushing; // Set to true when landing on a pushtile
    private sLevelRefactor.Directions pushDir; //direction the player is currently being pushed
    private bool undoPush;
    private bool prevLocPushSet; //the previous location after undoing a push move was being overriden in calc move, this helps fix that
    void Start()
    {
        curLevel = GameManger.instance.curLevelRef;
        startYLevel = transform.position.y;
        curGridLoc = curLevel.startCoords; 
        UIManager.instance.UpdateMovesMade(0);

    }
    void Update()
    {
        if (!undoMove && !isPushing)//Stop a move being queued while tryuing to undo a move
        { 
            if (Input.GetButtonDown("Forward"))
            {
                movementQueue.Enqueue(sLevelRefactor.Directions.forward);
            }
            else if (Input.GetButtonDown("Left"))
            {
                movementQueue.Enqueue(sLevelRefactor.Directions.left);
            }
            else if (Input.GetButtonDown("Right"))
            {
                movementQueue.Enqueue(sLevelRefactor.Directions.right);
            }
            else if (Input.GetButtonDown("Backward"))
            {
                movementQueue.Enqueue(sLevelRefactor.Directions.backward);
            }
        }
        if (movementQueue.Count != 0 && movementRoutine is null && !undoMove)
        {
            if (cancelQueue)
            {
                movementQueue.Clear();
                cancelQueue = false;
            }
            else
            {
                sLevelRefactor.Coordinates nextLoc;
                GameObject tile = curLevel.GetNextTile(movementQueue.Peek(), curGridLoc, out nextLoc);
                sTile tileRef = null;
                if (tile != null)
                {
                    tileRef = tile.GetComponent<sTile>();
                    if (tileRef.tileType == tileTypes.pushTile && !isPushing) //Start oush move | Checks if isPushing so cube can be pushed over other push tiles 
                    {
                        isPushing = true;
                        pushDir = tileRef.pushDirection;
                        //add a startpush to moves made when push begins and an endpush when its over. Then when undoing if it is startpush/endpush we know to keep going untill we hit the other one then go once more and done
                        movesMade.Add(sLevelRefactor.Directions.startPush);
                    }
                }
                if (tile != null && tileRef.tileType != tileTypes.empty && (curLevel.GetTile(nextLoc) != curLevel.GetTile(prevGridLoc) || isPushing))
                {//trigger a move
                    CalcMovement(nextLoc);
                }
                else //removes the last attempted move as there is no valid movement in specified direction. if a push move was active it adds end push to adi with undoing
                {
                    movementQueue.Dequeue();
                    if (isPushing)
                    {
                        movesMade.Add(sLevelRefactor.Directions.endPush);
                        isPushing = false;
                    }

                }
            }
        }
        else if (movementQueue.Count == 0)
        { //if player lands on a push tile they should be pushed
            if (curLevel.GetTile(curGridLoc).GetComponent<sTile>().tileType == tileTypes.pushTile)
            {
                isPushing = true;
                pushDir = curLevel.GetTile(curGridLoc).GetComponent<sTile>().pushDirection;
                movesMade.Add(sLevelRefactor.Directions.startPush);
                movementQueue.Clear();
                movementQueue.Enqueue(pushDir);
            }
        }

        if (movementQueue.Count != 0 && Input.GetButtonDown("CancelMovement")) //allows player to cancel movement queue. The queue will eb cleared next frame
        {
            cancelQueue = true;
        }

        if (movesMade.Count != 0 && Input.GetKeyDown(KeyCode.R) && !undoMove && movementQueue.Count == 0) //only undo if not moveing to avoid complications
        {
            undoMove = true;
            UndoMove();
        }
        
    }

    public void UndoMove()
    {
        if (movesMade.Count != 0 && undoMove && movementRoutine is null && movementQueue.Count == 0)
        {
            movementQueue.Clear();
            if (movesMade[movesMade.Count - 1] == sLevelRefactor.Directions.endPush)
            {
                //Start of Undo push
                movesMade.RemoveAt(movesMade.Count - 1); //removes the end push move at start of undo
                undoPush = true;
            }
            if (movesMade[movesMade.Count - 1] == sLevelRefactor.Directions.startPush)
            {
                //end of undo push
                movesMade.RemoveAt(movesMade.Count - 1); //removes the start push move at end of undo
                undoPush = false;
                undoMove = false;
                //Stop prevGridLoc being set in CalcMovement if its already happened here
                prevLocPushSet = true;
                //this was being reset in CalcMovement so has to be set here instead
                prevGridLoc = sLevelRefactor.GetNewCoordinates(curGridLoc, movesMade[movesMade.Count - 1]); 
            }
            else
            { //player is moved backwards once
                movementQueue.Enqueue(movesMade[movesMade.Count - 1]);
                movesMade.RemoveAt(movesMade.Count - 1);
                sLevelRefactor.Coordinates nextLoc;
                curLevel.GetNextTile(movementQueue.Peek(), curGridLoc, out nextLoc);
                CalcMovement(nextLoc);
            }
        }
        else
        {
            undoMove = false;
        }
    }

    private void AddUndoPushMove() 
    {
        if (movesMade[movesMade.Count - 1] == sLevelRefactor.Directions.endPush)
        {
            undoPush = true;
            movesMade.RemoveAt(movesMade.Count - 1);

        }
        if (undoPush)
        {
            if (movesMade[movesMade.Count - 1] == sLevelRefactor.Directions.startPush)
            {
                movesMade.RemoveAt(movesMade.Count - 1);
            }
            else
            {
                movementQueue.Enqueue(movesMade[movesMade.Count - 1]);
                movesMade.RemoveAt(movesMade.Count - 1);
                AddUndoPushMove();
            }
        }
    }

    private void CalcMovement(sLevelRefactor.Coordinates newLoc) //Calculates direction of movement + the edge and axis to rotate around before initating the movement routine
    {
        curLevel.GetTile(curGridLoc).GetComponent<sTile>().TileLeft();
        if (!prevLocPushSet) //ensures previous grid location isnt overriden when a push move is undone
        { 
            prevGridLoc = curGridLoc;
        }
        prevLocPushSet = false; //ensure this is reset
        curGridLoc = newLoc;
        if (undoMove && movesMade.Count != 0)
        {
            prevGridLoc = sLevelRefactor.GetNewCoordinates(curGridLoc, movesMade[movesMade.Count - 1]);
        }
        else if (undoMove && movesMade.Count == 0)
        {
            prevGridLoc = new sLevelRefactor.Coordinates(); //no prev location when at start as no moves made
        }
        Vector3 axis = Vector3.zero;
        int forwards = 0;
        int sideways = 0;
        sLevelRefactor.Directions dir = movementQueue.Peek();
        if (dir == sLevelRefactor.Directions.left) //grid is made of 1x1 unit tiles vector coordinates are used
        {
            sideways = 1;
            axis = Vector3.forward;
        }
        else if (dir == sLevelRefactor.Directions.right)
        {
            sideways = -1;
            axis = Vector3.forward;
        }
        else if (dir == sLevelRefactor.Directions.forward)
        {
            forwards = -1;
            axis = Vector3.right;
        }
        else if (dir == sLevelRefactor.Directions.backward)
        {
            forwards = 1;
            axis = Vector3.right;
        }
        //find the bottom edge of the cube in the direction of movement for rotation
        Vector3 edge = new Vector3((transform.position.x - ((transform.localScale.x / 2) * (float)sideways)),
            (transform.position.y - (transform.localScale.y / 2)),
            transform.position.z - ((transform.localScale.z / 2) * (float)forwards));
        movementRoutine = Movement(edge, forwards, sideways, axis);
        //RotateAroundPoint(transform, edge, transform.right, 90);
        if (!undoMove) //when a move is made add the flipped version to moves made.
        { //Only flipped now so that it doesnt need to be done later when undoing moves
            movesMade.Add(sLevelRefactor.FlipDirection(movementQueue.Peek()));
        }
        StartCoroutine(movementRoutine);
    }
    /// <summary>
    /// Rotate object around a specified point along a specified axis
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="aroundPoint"></param>
    /// <param name="aroundAxis"></param>
    /// <param name="angle"></param>
    private void RotateAroundPoint(Transform obj, Vector3 aroundPoint, Vector3 aroundAxis, float angle)
    {
        Vector3 dir = obj.position - aroundPoint;
        Quaternion rot = Quaternion.AngleAxis(angle, aroundAxis);
        obj.position = aroundPoint + rot * dir;
        obj.rotation = rot * obj.rotation;
    }

    //rotate cube to next tileq
    private IEnumerator Movement(Vector3 edge, float fDir, float sDir, Vector3 axis)
    {
        float angle = 0f;
        float increment = 0f;
        bool isUndoing = undoMove;
        float rotSpeed = GameManger.instance.playerSpeed;
        if (isPushing)
        {
            rotSpeed *= pushSpeedMod;
        }
        else if (isUndoing)
        {
            rotSpeed *= undoSpeedMod;
        }
        for (int i = 0; i < nRollsPerTile; i++) //repeat roll howver many rolls it takes to reach center of the next tile
        {
            while (angle < 90f) //90 degree rootation results in cube 'rolling' once onto the next side
            {// player is rotated on one if its edge once
                increment = Time.deltaTime * rotSpeed;
                increment = sDir != 0 ? increment * sDir : increment;
                increment = fDir != 0 ? increment * -fDir : increment;
                angle += Time.deltaTime * rotSpeed;
                RotateAroundPoint(transform, edge, axis, increment);
                
                yield return null;
            }
            //The new edge is found after 90d rotation is complete 
            edge = new Vector3((transform.position.x - ((transform.localScale.x / 2) * (float)sDir)), (transform.position.y - (transform.localScale.y / 2)), transform.position.z - ((transform.localScale.z / 2) * (float)fDir));
            angle = 0f;
        }
        //position and rotation are directly set to avoid any slight rotating errors.
        transform.rotation = Quaternion.identity;
        Vector3 tempPos = transform.position;
        tempPos.y = startYLevel;
        transform.position = new Vector3(curGridLoc.x, startYLevel, curGridLoc.y);

        if (curGridLoc == curLevel.endCoords && curLevel.IsAllSwitchOn()) //check if level completed
        {
            GameManger.instance.LevelComplete(TotalMoves());
        }
        //Let tiles know when theyve been landed on and the direction the player came from
        if (movesMade.Count != 0)
        {
            curLevel.GetTile(curGridLoc).GetComponent<sTile>().LandedOn(movesMade[movesMade.Count - 1]);
        }
        else
        {
            curLevel.GetTile(curGridLoc).GetComponent<sTile>().LandedOn(sLevelRefactor.Directions.none);
        }
        
        if (!isPushing)//doesnt remove the cur movement when the player has landed on a push tile to make them continue in one direction
        {
            movementQueue.Dequeue();
        }
        else if (isPushing)
        {//ensure player has to move in push direction untill push is over
            movementQueue.Clear();
            movementQueue.Enqueue(pushDir);
        }
        if (!GameManger.instance.isLevelEditing)
        { 
            UIManager.instance.UpdateMovesMade(TotalMoves());
        }
        movementRoutine = null;
        if (isUndoing && undoPush)//If push direction then needs to keep undoing
        {
            UndoMove();
        }
        else
        {
            undoMove = false;
        }
        
    }

    private int TotalMoves()
    {
        int moves = 0;
        bool countMoves = true;
        foreach (var item in movesMade)
        {
            if (countMoves)
            {
                moves++;
            }
            if (item == sLevelRefactor.Directions.startPush || item == sLevelRefactor.Directions.endPush)
            {
                countMoves = !countMoves;
            }
        }
        return moves;
    }

    public void UpdateSpeed(float newSpeed)
    {
        rotateSpeed = newSpeed;
    }

    public sLevelRefactor.Directions GetStartPushMoveDir()
    {
        return movesMade[movesMade.Count - 2];
    }
}
