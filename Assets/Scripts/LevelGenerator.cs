using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject roomNodePrefab;
    public int levelSize = 10;
    public float roomScale = 1;

    private RoomNode[,] roomMatrix;
    
    [Header("Room Prefabs")]
    // Pefabs of rooms of different connectivity
    public GameObject   Single;
    public GameObject   DoubleI; // 2 connections in a straight line
    public GameObject   DoubleL; // 2 connections at a right angle
    public GameObject   Triple;
    public GameObject   Quad;

    public GameObject[] SpecialRooms;

    void Awake()
    {
        Debug.Log(roomNodePrefab);
        // Initialize the room matrix
        roomMatrix = new RoomNode[levelSize, levelSize];
        DoGeneration();
    }

    void DoGeneration()
    {
        InitializeRooms();
        GenerateRoomsMethod1();
        PickAndPlace();
    }

    public void InitializeRooms()
    {
        for (int X = 0; X < levelSize; X++)
        { 
            for (int Z = 0; Z < levelSize; Z++)
            {
                RoomNode curRoom = Instantiate(roomNodePrefab, transform).GetComponent<RoomNode>();
                curRoom.SetGridPosition(X, Z, roomScale);

                /* Check if on border */
                if (X == 0) curRoom.SetConnectLeft(false);
                else if (X == levelSize - 1) curRoom.SetConnectRight(false);
                if (Z == 0) curRoom.SetConnectDown(false);
                else if (Z == levelSize - 1) curRoom.SetConnectUp(false);
                roomMatrix[X,Z] = curRoom;
            }
        }
    }

    public void GenerateRoomsMethod1()
    {
        /*
        Initialize list of expandable rooms
        initialize starting room

        loop..
            pick room from expandable rooms
            pick a direction from connectable rooms
            expand!
            update connectivity for rooms involved
            add to expandable rooms if appropriate
            remove from expandable rooms if appropriate!!
        */

        // Initialize expandable rooms, pick starting room
        List<RoomNode> expandables = new List<RoomNode>();
        RoomNode startRoom = roomMatrix[levelSize/2, levelSize/2];
        startRoom.SetColor(Color.red);
        expandables.Add(startRoom);
        

    }

    public void PickAndPlace()
    {

    }
    /* Idea:
    
    - The level will be generated into a matrix of mxn size, occupied by RoomNodes (or their gameobjects).
    - The information contained in the RoomNodes that occupy the cells of this structure will be used
    to generate the real level. The information they need to contain is the directions in which the room is connected.
    - The positions to place each real room is determined based on the cell position and the scale of each room.
    The scale is assumed to be uniform for each room. The position is on a [column][X] basis.


    For the sake of debugging, this class will include a method called UpdateVisuals() that will iterate
    over the level datastructure, move the transform of RoomNode placeholders into place (and scale appropraitely),
    and update their visuals to match.

    The user must be able to specify prefab to be used at each type of level. So the pick-and-place algorithm must
    look these up according to the connectivity of each room.



    Main loop:

    Initialize starting room

    find room that can be expanded from
    do expansion
    update connectivities of rooms accordingly
    repeat until enough rooms added / termination condition reached. 

    then, do pick and place.
    But for now, just iterate over each non-null cell and transform it to the correct location

    maybe to make it easy just instantiate every cell and update connectivites as required. They can later be purged if not needed. :))))))))

    */
}
