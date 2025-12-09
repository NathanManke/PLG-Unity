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
    public int iterations   = 10;
    public int maxExpandables = 5;

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
        GenerateRooms();
        PickAndPlace();
        UpdateAllVisuals();
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

    public void GenerateRooms()
    {
        /* Initialize list of rooms that can be grown from */
        List<RoomNode> expandables = new List<RoomNode>();

        /* Pick a room to start from */
        RoomNode startRoom = roomMatrix[levelSize/2, levelSize/2];
        startRoom.SetHasBeenFound(true);
        startRoom.SetColor(Color.red);
        expandables.Add(startRoom);

        /* Main loop */
        for (int i = 0; i < iterations; i++)
        {
            /* Pick a room to expand from */
            RoomNode roomFrom = PickRoomToExpandFrom(expandables);

            /* Pick a direction to expand into */
            int[] direction = PickDirToExpandInto(roomFrom);

            /* Connect the rooms */
            ConnectRooms(roomFrom, direction, expandables);

            /* Maintain expandables list */
            RoomNode roomInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), direction);
            if (!CheckCanExpand(roomFrom)) expandables.Remove(roomFrom);
            if (!CheckCanExpand(roomInto)) expandables.Remove(roomInto);

            /* Limit the number of rooms we can expand from */

            while (expandables.Count > maxExpandables)
            {
                expandables.RemoveAt(0);
            }
        }
    }

    /*******************************
     Hallway Generation Helpers
    *******************************/
    
    /* Pick a room to expand from, weighted by recency */
    public RoomNode PickRoomToExpandFrom(List<RoomNode> expandables)
    {
        int sumOfWeight = 0;
        int i;

        /* Sum all weights */
        for (i = 0; i < expandables.Count; i++)
        { sumOfWeight += 1 << i; }

        /* Pick a number on the range */
        int val = Random.Range(0, sumOfWeight);

        /* Determine which index corresponds to the range this random number landed on */
        for (i = 0; i < expandables.Count; i++)
        { 
            val -= 1 << i;
            if (val < 0) return expandables[i];
        }
        Debug.Log($"{expandables.Count}, {i}");
        return expandables[i];
    }

    public int[] PickDirToExpandInto(RoomNode roomFrom)
    {
        List<int[]> finalCons       = new List<int[]>();
        bool[] roomFromRules        = roomFrom.GetRules();
        bool[] roomFromCons         = roomFrom.GetConnections();
        
        /* Determine which directions we can connect into */
        
        RoomNode curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp);
        if (roomFromRules[IndexUp] && !roomFromCons[IndexUp] && curInto && curInto.GetRules()[IndexDown]) 
        { finalCons.Add(gridUp); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown);
        if (roomFromRules[IndexDown] && !roomFromCons[IndexDown] && curInto && curInto.GetRules()[IndexUp]) 
        { finalCons.Add(gridDown); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft);
        if (roomFromRules[IndexLeft] && !roomFromCons[IndexLeft] && curInto && curInto.GetRules()[IndexRight]) 
        { finalCons.Add(gridLeft); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight);
        if (roomFromRules[IndexRight] && !roomFromCons[IndexRight] && curInto && curInto.GetRules()[IndexLeft]) 
        { finalCons.Add(gridRight); }
        

        /*
        RoomNode curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp);
        if (roomFromRules[IndexUp] && curInto && curInto.GetRules()[IndexDown]) 
        { finalCons.Add(gridUp); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown);
        if (roomFromRules[IndexDown] && curInto && curInto.GetRules()[IndexUp]) 
        { finalCons.Add(gridDown); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft);
        if (roomFromRules[IndexLeft] && curInto && curInto.GetRules()[IndexRight]) 
        { finalCons.Add(gridLeft); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight);
        if (roomFromRules[IndexRight] && curInto && curInto.GetRules()[IndexLeft]) 
        { finalCons.Add(gridRight); }
        */
        
        /* We assume finalCons is non-empty, since roomFrom must be an expandable room */
        return finalCons[Random.Range(0, finalCons.Count)];
    }

    public void ConnectRooms(RoomNode roomFrom, int[] direction, List<RoomNode> expandables)
    {
        /* Set connectivity appropriately */
        RoomNode roomInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), direction);
        if (direction == gridUp)
        { roomFrom.SetConnectUp(true); roomInto.SetConnectDown(true); }
        else if (direction == gridDown)
        { roomFrom.SetConnectDown(true); roomInto.SetConnectUp(true); }
        else if (direction == gridLeft)
        { roomFrom.SetConnectLeft(true); roomInto.SetConnectRight(true); }
        else if (direction == gridRight)
        { roomFrom.SetConnectRight(true); roomInto.SetConnectLeft(true); }

        /* Add to the expandables list if its a newly connected room */
        if (roomInto.GetHasBeenFound()) return;
        roomInto.SetHasBeenFound(true);
        expandables.Add(roomInto);
    }

    public bool CheckCanExpand(RoomNode roomFrom)
    {
        bool[] roomFromRules        = roomFrom.GetRules();
        bool[] roomFromCons         = roomFrom.GetConnections();

        /* Determine if there is any room we can connect into */
        RoomNode curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp);
        if (roomFromRules[IndexUp] && !roomFromCons[IndexUp] && curInto && curInto.GetRules()[IndexDown])
        { return true; }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown);
        if (roomFromRules[IndexDown] && !roomFromCons[IndexDown] && curInto && curInto.GetRules()[IndexUp]) 
        { return true; }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft);
        if (roomFromRules[IndexLeft] && !roomFromCons[IndexLeft] && curInto && curInto.GetRules()[IndexRight]) 
        { return true;}

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight);
        if (roomFromRules[IndexRight] && !roomFromCons[IndexRight] && curInto && curInto.GetRules()[IndexLeft]) 
        { return true; }

        return false;
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

    public void UpdateAllVisuals()
    {
        for (int X = 0; X < levelSize; X++)
        { for (int Y = 0; Y < levelSize; Y++)
            { roomMatrix[X, Y].UpdateVisuals(); }
        }
    }
}
