using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    /*******************************
     Constants
    *******************************/

    // Up/Down/Left/Right coordinate offsets
    private static readonly int[] gridUp    = {0, 1};
    private static readonly int[] gridDown  = {0, -1};
    private static readonly int[] gridLeft  = {-1, 0};
    private static readonly int[] gridRight =  {1, 0};
    private static readonly int[] defaultOffset = {0, 0};

    // Index of X and Y coordinates in a gridPos
    private int IndexX              = 0;
    private int IndexY              = 1;

    // UDLR indices
    private int IndexUp             = 0;
    private int IndexDown           = 1;
    private int IndexLeft           = 2;
    private int IndexRight          = 3;

    /*******************************
     Fields
    *******************************/

    // Prefab of RoomNodes that are used for generation
    public GameObject roomNodePrefab;

    // Grid size, cell size
    public int levelSize    = 10;
    public float roomScale  = 1;
    public int iterations   = 10;
    public int maxExpandables = 5;

    // Pefabs of rooms of different connectivity
    [Header("Room Prefabs")]
    public GameObject   Single;
    public GameObject   DoubleI; // 2 connections in a straight line
    public GameObject   DoubleL; // 2 connections at a right angle
    public GameObject   Triple;
    public GameObject   Quad;

    public bool canGenerate = true;

    // To contain the list of special rooms
    public List<GameObject> specialRoomsToPlace;
    public List<SpecialRoom> specialRooms;

    // Used for generation
    private List<RoomNode> roomMatrix;
    private List<RoomNode> expandables;
    private List<RoomNode> unconnnecteds;
    private RoomNode startRoom;

    void Awake()
    {
        roomMatrix = new List<RoomNode>();
    }

    public void SetSpecialRoomsToPlace(List<GameObject> SRIn)
    {
        specialRoomsToPlace = SRIn;
    }

    public void DoGeneration()
    {
        canGenerate = false;
        InitializeRooms();
        GenerateRooms();
        PickAndPlace();
        UpdateAllVisuals();
        canGenerate = true;
    }

    public void InitializeRooms()
    {
        // Reset everything
        for (int i = roomMatrix.Count - 1; i >= 0; i--)
        {
            Destroy(roomMatrix[i].gameObject);
            roomMatrix.RemoveAt(i);
        }

        // Initialize level
        expandables  = new List<RoomNode>();
        unconnnecteds = new List<RoomNode>();

        startRoom = Instantiate(roomNodePrefab, transform).GetComponent<RoomNode>();
        startRoom.SetHasBeenFound(true);
        startRoom.SetColor(Color.red);
        startRoom.SetGridPosition(0, 0, roomScale);

        expandables.Add(startRoom);
        roomMatrix.Add(startRoom);


        // Init randomness
        Random.InitState(1);
    }

    public void GenerateRooms()
    {
        // Main loop
        List<RoomNode> newRooms = GenerateHallways(10);
        //GenerateSpecialRoom();
        UpdateAllVisuals();
    }

    /*******************************
     Generation Helpers
    *******************************/
    
    public List<RoomNode> GenerateHallways(int numIterations)
    {
        List<RoomNode> generatedRooms = new List<RoomNode>();
        for (int i = 0; i < numIterations; i++)
        {
            List<RoomNode> mostRecents = GetMostRecents(maxExpandables);
            // Pick a room to expand from
            RoomNode roomFrom = PickRoomToExpandFrom(mostRecents);

            // Pick a direction to expand into
            int[] direction = PickDirToExpandInto(roomFrom);
            RoomNode roomInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), direction);

            // Connect the rooms
            if (ConnectRooms(roomFrom, direction)) generatedRooms.Add(roomInto);
        }
        return generatedRooms;
    }

    // Get the most recently placed expandable rooms
    public List<RoomNode> GetMostRecents(int numRecent)
    {
        List<RoomNode> result = new List<RoomNode>();

        if (expandables.Count <= maxExpandables) return expandables;

        for (int i = expandables.Count - maxExpandables; i < expandables.Count; i++)
        {
            result.Add(expandables[i]);
        }
        return result;
    }

    // Pick a room to expand from, weighted by recency
    public RoomNode PickRoomToExpandFrom(List<RoomNode> recents)
    {
        int sumOfWeight = 0;
        int i;

        // Sum all weights
        for (i = 0; i < recents.Count; i++)
        { sumOfWeight += 1 << i; }

        // Pick a number on the range
        int val = Random.Range(0, sumOfWeight);

        // Determine which index corresponds to the range this random number landed on
        for (i = 0; i < recents.Count; i++)
        { 
            val -= 1 << i;
            if (val < 0) return recents[i];
        }
        return recents[i];
    }

    public int[] PickDirToExpandInto(RoomNode roomFrom)
    {
        List<int[]> finalCons       = new List<int[]>();
        bool[] roomFromRules        = roomFrom.GetRules();
        bool[] roomFromCons         = roomFrom.GetConnections();
        
        // Determine which directions we can connect into
        
        RoomNode curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp);
        if (roomFromRules[IndexUp] && !roomFromCons[IndexUp] && curInto && curInto.GetRules()[IndexDown] && !curInto.GetIsSpecial()) 
        { finalCons.Add(gridUp); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown);
        if (roomFromRules[IndexDown] && !roomFromCons[IndexDown] && curInto && curInto.GetRules()[IndexUp] && !curInto.GetIsSpecial()) 
        { finalCons.Add(gridDown); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft);
        if (roomFromRules[IndexLeft] && !roomFromCons[IndexLeft] && curInto && curInto.GetRules()[IndexRight] && !curInto.GetIsSpecial()) 
        { finalCons.Add(gridLeft); }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight);
        if (roomFromRules[IndexRight] && !roomFromCons[IndexRight] && curInto && curInto.GetRules()[IndexLeft] && !curInto.GetIsSpecial()) 
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
       
        
        // We assume finalCons is non-empty, since roomFrom must be an expandable room
        return finalCons[Random.Range(0, finalCons.Count)];
    }

    public bool ConnectRooms(RoomNode roomFrom, int[] direction)
    {
        bool rval = false;
        // Set connectivity appropriately
        RoomNode roomInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), direction);
        if (direction == gridUp)
        { roomFrom.SetConnectUp(true); roomInto.SetConnectDown(true); }
        else if (direction == gridDown)
        { roomFrom.SetConnectDown(true); roomInto.SetConnectUp(true); }
        else if (direction == gridLeft)
        { roomFrom.SetConnectLeft(true); roomInto.SetConnectRight(true); }
        else if (direction == gridRight)
        { roomFrom.SetConnectRight(true); roomInto.SetConnectLeft(true); }

        // Maintain expandables
        if (!roomInto.GetHasBeenFound()) 
        {
            roomInto.SetHasBeenFound(true);
            expandables.Add(roomInto);
            unconnnecteds.Remove(roomInto);
            rval = true;
        }
        if (!CheckCanExpand(roomFrom)) expandables.Remove(roomFrom);
        if (!CheckCanExpand(roomInto)) expandables.Remove(roomInto);

        // Return whether this room is newly found
        return rval;
    }

    public bool CheckCanExpand(RoomNode roomFrom)
    {
        bool[] roomFromRules        = roomFrom.GetRules();
        bool[] roomFromCons         = roomFrom.GetConnections();

        // Determine if there is any room we can connect into
        RoomNode curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp);
        if (roomFromRules[IndexUp] && !roomFromCons[IndexUp] && curInto && curInto.GetRules()[IndexDown] && !curInto.GetIsSpecial())
        { return true; }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown);
        if (roomFromRules[IndexDown] && !roomFromCons[IndexDown] && curInto && curInto.GetRules()[IndexUp] && !curInto.GetIsSpecial()) 
        { return true; }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft);
        if (roomFromRules[IndexLeft] && !roomFromCons[IndexLeft] && curInto && curInto.GetRules()[IndexRight] && !curInto.GetIsSpecial()) 
        { return true;}

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight);
        if (roomFromRules[IndexRight] && !roomFromCons[IndexRight] && curInto && curInto.GetRules()[IndexLeft] && !curInto.GetIsSpecial()) 
        { return true; }

        return false;
    }

    public RoomNode GetRoomAtGridPos(int[] gridPos, int[] offset = null)
    {
        // Add offset and check for existence
        offset ??= defaultOffset;
        int[] resultPos = {gridPos[IndexX] + offset[IndexX], gridPos[IndexY] + offset[IndexY]};

        foreach (RoomNode room in roomMatrix)
        {
            int[] curPos = room.GetGridPosition();
            if (curPos[0] == resultPos[0] && curPos[1] == resultPos[1]) return room;
        }

        // No room found, so create a new one
        RoomNode newRoom = Instantiate(roomNodePrefab, transform).GetComponent<RoomNode>();
        newRoom.SetGridPosition(resultPos[IndexX], resultPos[IndexY], roomScale);
        roomMatrix.Add(newRoom);
        unconnnecteds.Add(newRoom);
        Debug.Log($"New room at {resultPos[IndexX]} {resultPos[1]}");
        return newRoom;
    }

    /*******************************
     Special Room Helpers
    *******************************/

    public void GenerateSpecialRoom()
    {
        return;
    }
    public RoomNode PlaceSpecialRoom(RoomNode roomFrom)
    {
        return null;
    }

    public RoomNode GetFurthestHallway(List<RoomNode> toCheck, RoomNode origin)
    {
        return null;
    }

    public float GetRoomDistanceSqrd(RoomNode r1, RoomNode r2)
    {
        int[] pos1 = r1.GetGridPosition();
        int[] pos2 = r2.GetGridPosition();

        float distX = pos1[IndexX] - pos2[IndexX];
        float distY = pos1[IndexY] - pos2[IndexY];

        return distX * distX + distY * distY;
    }

    public void PickAndPlace()
    {
        /*
        TO DO... place hallways and special rooms (instantiate prefabs at correct locations)
       */
    }

    public void UpdateAllVisuals()
    {
        foreach (RoomNode room in roomMatrix)
        {
            room.UpdateVisuals();
        }
    }
}
