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
    private int IndexZ              = 1;

    // UDLR indices
    private int IndexUp             = 0;
    private int IndexDown           = 1;
    private int IndexLeft           = 2;
    private int IndexRight          = 3;

    private bool visualize          = false;

    /*******************************
     Internal Use
    *******************************/

    // Prefab of RoomNodes that are used for generation
    public GameObject roomNodePrefab;

    // Used for generation
    private List<RoomNode> allRoomNodes;            // Contains every roomNode created
    private List<RoomNode> expandables;             // Contains all expandable roomNodes
    private List<RoomNode> unconnecteds;            // Contains all unconnected roomNodes
    private RoomNode startRoom;                     // The room from which generation starts
    private List<RoomNode> availableSpecials;       // Available spots to place a special room

    private List<GameObject> realHalls;             // Contains all the instantiated hallways
    private List<GameObject> realSpecials;          // Contains all the instantiated special rooms

    private Transform nodeContainer;                // Transform to parent all roomNodes
    private Transform roomContainer;                // Transform to parent all placed rooms

    /*******************************
     System Parameters
    *******************************/

    // Pefabs of rooms of different connectivity
    [Header("Hallway Prefabs")]
    public GameObject   Single;
    public GameObject   DoubleI; // 2 connections in a straight line
    public GameObject   DoubleL; // 2 connections at a right angle
    public GameObject   Triple;
    public GameObject   Quad;

    [Header("Generation Parameters")]
    public int recentPoolSize = 5;
    public int desiredIterations = 10;

    public float roomScale  = 1;


    // Prefabs of the special rooms that need to be placed in the level
    [Header("Special Rooms")]
    public bool considerSpecialRooms;
    public List<GameObject> specialRoomsToPlace;
    public int iterationsBetweenSpecialPlacement = 10;

    
    // Initialize the all-containing lists
    void Awake()
    {
        allRoomNodes    = new List<RoomNode>();
        realHalls       = new List<GameObject>();
        realSpecials    = new List<GameObject>();

        nodeContainer = transform.Find("Nodes");
        roomContainer = transform.Find("Rooms");
    }

    // The easily callable method that generates the level
    public void DoGeneration()
    {
        Initialize();
        GenerateRooms();
        PickAndPlace();
        UpdateAllVisuals(visualize);
    }

    /*******************************
     Initialization Helpers
    *******************************/

    // Empty lists used in generation and destroy roomNode instances
    public void ClearGenerationObjects()
    {
        for (int i = allRoomNodes.Count - 1; i >= 0; i--)
        {
            Destroy(allRoomNodes[i].gameObject);
            allRoomNodes.RemoveAt(i);
        }
        expandables  = new List<RoomNode>();
        unconnecteds = new List<RoomNode>();
        availableSpecials = new List<RoomNode>();
    }

    // Empty lists containing instantiated level objects and destroy their instances
    public void ClearInstantiatedRooms()
    {
        for (int i = realSpecials.Count - 1; i >= 0; i--)
        {
            Destroy(realSpecials[i]);
            realSpecials.RemoveAt(i);
        }
        for (int i = realHalls.Count - 1; i >= 0; i--)
        {
            Destroy(realHalls[i]);
            realHalls.RemoveAt(i);
        }
    }

    // Initialize the program for generation
    public void Initialize()
    {
        // Reset everything
        ClearGenerationObjects();
        ClearInstantiatedRooms();

        // The first room
        startRoom = Instantiate(roomNodePrefab, nodeContainer).GetComponent<RoomNode>();
        startRoom.SetGridPosition(0, 0);
        startRoom.SetHasBeenFound(true);
        startRoom.SetColor(Color.yellow);


        expandables.Add(startRoom);
        allRoomNodes.Add(startRoom);
    }

    /*******************************
     Generation Helpers
    *******************************/

    public void GenerateRooms()
    {
        int numSpecial      = specialRoomsToPlace.Count;

        // Determine the total number of iterations that must be done
        int totalIters      = Mathf.Max(desiredIterations, numSpecial * iterationsBetweenSpecialPlacement);

        // Do generation
        for (int itersComplete = 1; itersComplete <= totalIters; itersComplete++)
        {
            List<RoomNode> newRooms = GenerateHallways(1);
            if (considerSpecialRooms && itersComplete % iterationsBetweenSpecialPlacement == 0) 
            {
                Debug.Log("Generating special room");
                GenerateSpecialRoom();
            }
        }
    }

    // Perform numIterations hallway generations
    public List<RoomNode> GenerateHallways(int numIterations)
    {
        List<RoomNode> generatedRooms = new List<RoomNode>();
        for (int i = 0; i < numIterations; i++)
        {
            List<RoomNode> mostRecents = GetMostRecents(recentPoolSize, expandables);
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

    // Get the last numRecent entries in the list
    public List<RoomNode> GetMostRecents(int numRecent, List<RoomNode> toGetFrom)
    {
        List<RoomNode> result = new List<RoomNode>();

        if (toGetFrom.Count <= numRecent) return toGetFrom;

        for (int i = toGetFrom.Count - numRecent; i < toGetFrom.Count; i++)
        {
            result.Add(toGetFrom[i]);
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

    // Pick a direction to expand this room into
    public int[] PickDirToExpandInto(RoomNode roomFrom)
    {
        bool special                = roomFrom.GetIsSpecial();
        List<int[]> finalCons       = new List<int[]>();
        bool[] roomFromRules        = roomFrom.GetRules();
        bool[] roomFromCons         = roomFrom.GetConnections();
        
        // Determine which directions we can connect into
        RoomNode curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp, special);
        if (roomFromRules[IndexUp] && !roomFromCons[IndexUp] && curInto && curInto.GetRules()[IndexDown] && !curInto.GetIsSpecial()) 
        { 
            if (!special || curInto.GetHasBeenFound()) finalCons.Add(gridUp);
        }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown, special);
        if (roomFromRules[IndexDown] && !roomFromCons[IndexDown] && curInto && curInto.GetRules()[IndexUp] && !curInto.GetIsSpecial()) 
        { 
            if (!special || curInto.GetHasBeenFound()) finalCons.Add(gridDown);
        }
        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft, special);
        if (roomFromRules[IndexLeft] && !roomFromCons[IndexLeft] && curInto && curInto.GetRules()[IndexRight] && !curInto.GetIsSpecial()) 
        { 
            if (!special || curInto.GetHasBeenFound()) finalCons.Add(gridLeft);
        }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight, special);
        if (roomFromRules[IndexRight] && !roomFromCons[IndexRight] && curInto && curInto.GetRules()[IndexLeft] && !curInto.GetIsSpecial()) 
        { 
            if (!special || curInto.GetHasBeenFound()) finalCons.Add(gridRight);
        }
       
        // We assume finalCons is non-empty, since roomFrom must be an expandable room
        return finalCons[Random.Range(0, finalCons.Count)];
    }

    // Connect a room with the one in the direction specified
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
            unconnecteds.Remove(roomInto);
            rval = true;
        }
        if (!CheckCanExpand(roomFrom)) expandables.Remove(roomFrom);
        if (!CheckCanExpand(roomInto)) expandables.Remove(roomInto);

        // Return whether this room is newly found
        return rval;
    }

    // Determine if a room can be expanded at all
    public bool CheckCanExpand(RoomNode roomFrom)
    {
        bool rval = false;
        bool[] roomFromRules        = roomFrom.GetRules();
        bool[] roomFromCons         = roomFrom.GetConnections();

        // Check rules, current connectivity, and properties of the adjacent rooms
        // This also initializes adjacent roomNodes if appropriate
        RoomNode curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridUp, roomFrom.GetIsSpecial());
        if (roomFromRules[IndexUp] && !roomFromCons[IndexUp] && curInto && curInto.GetRules()[IndexDown] && !curInto.GetIsSpecial())
        { rval = true; }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridDown, roomFrom.GetIsSpecial());
        if (roomFromRules[IndexDown] && !roomFromCons[IndexDown] && curInto && curInto.GetRules()[IndexUp] && !curInto.GetIsSpecial()) 
        { rval = true; }

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridLeft, roomFrom.GetIsSpecial());
        if (roomFromRules[IndexLeft] && !roomFromCons[IndexLeft] && curInto && curInto.GetRules()[IndexRight] && !curInto.GetIsSpecial()) 
        { rval = true;}

        curInto = GetRoomAtGridPos(roomFrom.GetGridPosition(), gridRight, roomFrom.GetIsSpecial());
        if (roomFromRules[IndexRight] && !roomFromCons[IndexRight] && curInto && curInto.GetRules()[IndexLeft] && !curInto.GetIsSpecial()) 
        { rval = true; }

        return rval;
    }

    // Find the room with a given grid position. If none found, initialize a room at that position if appropriate
    public RoomNode GetRoomAtGridPos(int[] gridPos, int[] offset = null, bool preventNew = false)
    {
        // Add offset and check for existence
        offset ??= defaultOffset;
        int[] resultPos = {gridPos[IndexX] + offset[IndexX], gridPos[IndexZ] + offset[IndexZ]};

        foreach (RoomNode room in allRoomNodes)
        {
            int[] curPos = room.GetGridPosition();
            if (curPos[0] == resultPos[0] && curPos[1] == resultPos[1]) return room;
        }

        // None found, so initialize a room at that position if desired
        if (preventNew) return null;

        RoomNode newRoom = Instantiate(roomNodePrefab, nodeContainer).GetComponent<RoomNode>();
        newRoom.SetGridPosition(resultPos[IndexX], resultPos[IndexZ]);
        newRoom.transform.localPosition = GetCoords(newRoom, roomScale);
        allRoomNodes.Add(newRoom);
        unconnecteds.Add(newRoom);

        return newRoom;
    }

    /*******************************
     Special Room Helpers
    *******************************/

    // Find
    public void GenerateSpecialRoom()
    {
        // Get most recent unconnecteds
        List<RoomNode> mostRecents = GetMostRecents(recentPoolSize, unconnecteds);

        // Pick randomly from most recent unconnecteds and make it special
        RoomNode special = mostRecents[Random.Range(0, mostRecents.Count)];
        special.SetColor(Color.cyan); 
        special.SetIsSpecial(true);
        special.SetHasBeenFound(true);
        unconnecteds.Remove(special);
        availableSpecials.Add(special);

        // Determine a direction to connect this special room in
        int[] dir = PickDirToExpandInto(special);
        ConnectRooms(special, dir);

        // Maintain expandability for nodes around special room
        foreach (int[] curDir in new int[][] { gridUp, gridDown, gridLeft, gridRight})
        {
            RoomNode curRoom = GetRoomAtGridPos(special.GetGridPosition(), curDir, true);
            // Only check existing, connected nodes
            if (curRoom && curRoom.GetHasBeenFound() && !CheckCanExpand(curRoom)) expandables.Remove(curRoom);
        }
    }

    // Return the squared distance between two rooms by grid position
    public float GetRoomDistanceSqrd(RoomNode r1, RoomNode r2)
    {
        int[] pos1 = r1.GetGridPosition();
        int[] pos2 = r2.GetGridPosition();

        float distX = pos1[IndexX] - pos2[IndexX];
        float distY = pos1[IndexZ] - pos2[IndexZ];

        return distX * distX + distY * distY;
    }

    /*******************************
     Room Placement
    *******************************/

    // Place all hallways and special rooms
    public void PickAndPlace()
    {
        // Do special rooms first
        for (int i = specialRoomsToPlace.Count - 1; i >= 0; i--)
        {
            // Pick a special room to place this at
            if (availableSpecials.Count == 0)
            {
                Debug.Log("No special room spots available!"); break;
            }

            // Get room prefab and random special room spot to place it at
            GameObject specialPrefab    = specialRoomsToPlace[i];
            RoomNode specialRoom        = availableSpecials[Random.Range(0, availableSpecials.Count)];
            PlaceSpecial(specialRoom, specialPrefab);
            availableSpecials.Remove(specialRoom);
        }

        // Make remaining available special rooms non-special
        for (int i = availableSpecials.Count - 1; i >= 0; i--)
        {
            availableSpecials[i].SetIsSpecial(false);
            availableSpecials.RemoveAt(i);
        }
        
        // Generate hallways
        for (int i = allRoomNodes.Count - 1; i >= 0; i--)
        {
            RoomNode room = allRoomNodes[i];
            if (room.GetIsSpecial()) continue;
            PlaceHallway(room);
        }
    }

    public void PlaceSpecial(RoomNode specialRoom, GameObject specialPrefab)
    {
        float rotation  = 0f;
        bool[] cons     = specialRoom.GetConnections();

        if      (cons[IndexUp])     rotation = -90f;
        else if (cons[IndexDown])   rotation = 90f;
        else if (cons[IndexLeft])   rotation = 180f;

        GameObject realSpecial = Instantiate(specialPrefab, roomContainer);
        realSpecial.transform.localPosition = GetCoords(specialRoom, roomScale);
        realSpecial.transform.localRotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
        realSpecials.Add(realSpecial);
    }

    // Determine the given hallway's type, rotation and place appropriately
    public void PlaceHallway(RoomNode room)
    {
        GameObject type;
        float rotation  = 0f;
        int udlr        = 0b0000;
        int cons        = 0;

        // Determine how many connections, represent UDLR connections in binary
        foreach (bool val in room.GetConnections())
        {
            int b = val? 1 : 0;
            udlr <<= 1;
            udlr |= b;
            cons += b;
        }
        if (cons == 0) return;  // Should not be unconnected

        // Singly connected
        else if (cons == 1) 
        {
            type = Single;
            if      (udlr == 0b0001)    rotation = 0f;     
            else if (udlr == 0b0100)    rotation = 90f;    
            else if (udlr == 0b0010)    rotation = 180f;
            else                        rotation = -90f;
        }
        // Triply connected
        else if (cons == 3) 
        {
            type = Triple;
            if      (udlr == 0b1101)    rotation = 0f;
            else if (udlr == 0b0111)    rotation = 90f;
            else if (udlr == 0b1110)    rotation = 180f;
            else                        rotation = -90f;
        }
        // Fully connected; rotation irrelevant
        else if (cons == 4) type = Quad;

        // Doubly connected, so determine if I or L shaped
        else
        {
            if      (udlr == 0b0011)    { type = DoubleI; rotation = 0f;    }
            else if (udlr == 0b1100)    { type = DoubleI; rotation = 90f;   }

            else if (udlr == 0b0101)    { type = DoubleL; rotation = 0f;    }
            else if (udlr == 0b0110)    { type = DoubleL; rotation = 90f;   }
            else if (udlr == 0b1010)    { type = DoubleL; rotation = 180f;  }
            else                        { type = DoubleL; rotation = -90f;  }
        }

        // Instantiate the real room
        GameObject realRoom = Instantiate(type, roomContainer, false);
        realRoom.transform.localPosition = GetCoords(room, roomScale);
        realRoom.transform.localRotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
        realHalls.Add(realRoom);
    }

    // Get realword positioning of a room
    public Vector3 GetCoords(RoomNode room, float scale)
    {
        int[] gridPos = room.GetGridPosition();
        return new Vector3(gridPos[IndexX] * scale, 0f, gridPos[IndexZ] * scale);
    }

    public void UpdateAllVisuals(bool val)
    {
        foreach (RoomNode room in allRoomNodes)
        {
            room.UpdateVisuals(val);
        }
    }

    public void ToggleVisuals()
    {
        visualize = !visualize;
        UpdateAllVisuals(visualize);
    }
}
