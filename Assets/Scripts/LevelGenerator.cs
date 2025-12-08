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

    void Start()
    {
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
        for (int row = 0; row < levelSize; row++)
        { 
            for (int col = 0; col < levelSize; col++)
            {
                Vector3 roomPos = transform.position;
                roomPos += new Vector3(roomScale * row, 0, roomScale * col);
                RoomNode curRoom = Instantiate(roomNodePrefab, transform).GetComponent<RoomNode>();
                curRoom.SetGridPosition(row, col, roomScale);

                roomMatrix[row, col] = curRoom;
            }
        }
    }

    public void GenerateRoomsMethod1()
    {

    }

    public void PickAndPlace()
    {

    }
    /* Idea:
    
    - The level will be generated into a matrix of mxn size, occupied by RoomNodes (or their gameobjects).
    - The information contained in the RoomNodes that occupy the cells of this structure will be used
    to generate the real level. The information they need to contain is the directions in which the room is connected.
    - The positions to place each real room is determined based on the cell position and the scale of each room.
    The scale is assumed to be uniform for each room. The position is on a [column][row] basis.


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
