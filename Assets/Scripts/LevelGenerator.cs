using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    /*******************************
        Constants
    *******************************/

    /* Up/Down/Left/Right coordinate offsets */
    private static readonly int[] gridUp    = {0, 1};
    private static readonly int[] gridDown  = {0, -1};
    private static readonly int[] gridLeft  = {-1, 0};
    private static readonly int[] gridRight =  {1, 0};
    private static readonly int[] defaultOffset = {0, 0};

    /* Index of X and Y coordinates in a gridPos */
    private int IndexX              = 0;
    private int IndexY              = 1;

    /* UDLR indices */
    private int IndexUp             = 0;
    private int IndexDown           = 1;
    private int IndexLeft           = 2;
    private int IndexRight          = 3;

    /*******************************
        Fields
    *******************************/

    /* Prefab of RoomNodes that are used for generation */
    public GameObject roomNodePrefab;

    /* Grid size, cell size */
    public int levelSize    = 10;
    public float roomScale  = 1;

    /* Pefabs of rooms of different connectivity */
    [Header("Room Prefabs")]
    public GameObject   Single;
    public GameObject   DoubleI; // 2 connections in a straight line
    public GameObject   DoubleL; // 2 connections at a right angle
    public GameObject   Triple;
    public GameObject   Quad;

    /* To contain the list of special rooms */
    public GameObject[] SpecialRooms;

    /* Used for generation */
    private RoomNode[,] roomMatrix;


    void Awake()
    {
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
                /* Create an instance of the RoomNode */
                RoomNode curRoom = Instantiate(roomNodePrefab, transform).GetComponent<RoomNode>();

                /* Place the room in the world */
                curRoom.SetGridPosition(X, Z, roomScale);

                /* Check if on border, update rules accordingly */
                if (X == 0) curRoom.SetRuleLeft(false);
                else if (X == levelSize - 1) curRoom.SetRuleRight(false);
                if (Z == 0) curRoom.SetRuleDown(false);
                else if (Z == levelSize - 1) curRoom.SetRuleUp(false);

                /* Add the room to the matrix */
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

        /* Initialize list of rooms that can be grown from */
        List<RoomNode> expandables = new List<RoomNode>();

        /* Pick a room to start from */
        RoomNode startRoom = roomMatrix[levelSize/2, levelSize/2];
        startRoom.SetHasBeenFound(true);
        startRoom.SetColor(Color.red);
        expandables.Add(startRoom);

        /* Main loop */
        int numIter = 15;

        for (int i = 0; i < numIter; i++)
        {
            /* Pick a room to expand from */
            RoomNode roomFrom = PickRoomToExpandFrom(expandables);

            /* Pick a direction to expand into */
            RoomNode roomInto = PickRoomToExpandInto(roomFrom);

            /* Connect the rooms */


            /* Maintain expandables list */

        }
    }

    /*******************************
        Generation Helpers
    *******************************/
    
    /* Pick a room to expand from, weighted by recency */
    public RoomNode PickRoomToExpandFrom(List<RoomNode> expandables)
    {
        int sumOfWeight = 0;
        int i;

        /* Sum all weights */
        for (i = 0; i < expandables.Count; i++)
        { sumOfWeight += i * (int)Mathf.Pow(2, i); }

        /* Pick a number on the range */
        int val = Random.Range(0, sumOfWeight);

        /* Determine which index corresponds to the range this random number landed on */
        i = -1;
        while (val - (int)Mathf.Pow(2, ++i) >= 0) {}

        return expandables[i];
    }

    public RoomNode PickRoomToExpandInto(RoomNode roomFrom)
    {
        List<RoomNode> finalCons    = new List<RoomNode>();
        bool[] roomFromRules        = roomFrom.GetRules();
        bool[] roomFromCons         = roomFrom.GetConnections();
        RoomNode curInto;
        
        /* Determine which directions we can connect into */
        /* We are checking if the from room CAN connect, ISNT connected, and that the into room exists and CAN connect*/
        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp);
        if (roomFromRules[IndexUp] && !roomFromCons[IndexUp] && curInto && curInto.GetRules()[IndexDown]) 
        { finalCons.Add(curInto); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown);
        if (roomFromRules[IndexDown] && !roomFromCons[IndexDown] && curInto && curInto.GetRules()[IndexUp]) 
        { finalCons.Add(curInto); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft);
        if (roomFromRules[IndexLeft] && !roomFromCons[IndexLeft] && curInto && curInto.GetRules()[IndexRight]) 
        { finalCons.Add(curInto); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight);
        if (roomFromRules[IndexRight] && !roomFromCons[IndexRight] && curInto && curInto.GetRules()[IndexLeft]) 
        { finalCons.Add(curInto); }
        
        /* We assume finalCons is non-empty, since roomFrom must be an expandable room */
        return finalCons[Random.Range(0, finalCons.Count)];
    }

    public RoomNode GetRoomAtGridPos(int[] gridPos, int[] offset = null)
    {
        /* Add offset and check if within bounds */
        offset ??= defaultOffset;
        int[] resultPos = {gridPos[IndexX] + offset[IndexX], gridPos[IndexY] + offset[IndexY]};

        if (resultPos[IndexX] < 0 || resultPos[IndexX] > levelSize - 1) return null;
        if (resultPos[IndexY] < 0 || resultPos[IndexY] > levelSize - 1) return null;

        return roomMatrix[resultPos[IndexX], resultPos[IndexY]];
    }

    public void PickAndPlace()
    {
        /*
        TO DO... place hallways and special rooms (instantiate prefabs at correct locations)
        */
    }
}
