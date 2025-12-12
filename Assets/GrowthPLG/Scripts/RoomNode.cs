using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : MonoBehaviour
{
    // Whether the node is connected in any direction
    [SerializeField] private bool connectUp         = false;
    [SerializeField] private bool connectDown       = false;
    [SerializeField] private bool connectLeft       = false;
    [SerializeField] private bool connectRight      = false;

    // Rules describing which directions the node can connect towards
    [SerializeField] private bool ruleUp            = true;
    [SerializeField] private bool ruleDown          = true;
    [SerializeField] private bool ruleLeft          = true;
    [SerializeField] private bool ruleRight         = true;

    // Position on the map grid
    [SerializeField] private int[] gridPos          = {0, 0};

    // Whether this node has been connected into the rest of the map yet
    [SerializeField] private bool hasBeenFound      = false;

    // To store the game objects used to indicate connectivity in each direction
    private GameObject upInd, downInd, leftInd, rightInd, orb;

    // Whether this node is used in a special room
    [SerializeField] private bool isSpecial         = false;

    void Awake()
    {
        // Get references to child direction indicators 
        upInd       = transform.Find("Container/Up").gameObject;
        downInd     = transform.Find("Container/Down").gameObject;
        leftInd     = transform.Find("Container/Left").gameObject;
        rightInd    = transform.Find("Container/Right").gameObject;
        orb         = transform.Find("Container/Sphere").gameObject;

        // Set connectivity if values provided
        SetConnectUp(connectUp);
        SetConnectDown(connectDown);
        SetConnectLeft(connectRight);
        SetConnectRight(connectRight);
    }

    /*******************************
     Connectivity Functions
    *******************************/

    // Set/Get UDLR connectivity
    public void SetConnectUp(bool enable)
    {
        connectUp = enable;
    }
    public void SetConnectDown(bool enable)
    {
        connectDown = enable;
    }
    public void SetConnectLeft(bool enable)
    {
        connectLeft = enable;
    }
    public void SetConnectRight(bool enable)
    {
        connectRight = enable;
    }
    public bool[] GetConnections()
    {
        bool[] result = {connectUp, connectDown, connectLeft, connectRight};
        return result;
    }

    // Set/Get UDLR rules
    public void SetRuleUp(bool enable)
    {
        ruleUp = enable;
    }
    public void SetRuleDown(bool enable)
    {
        ruleDown = enable;
    }
    public void SetRuleLeft(bool enable)
    {
        ruleLeft = enable;
    }
    public void SetRuleRight(bool enable)
    {
        ruleRight = enable;
    }
    public bool[] GetRules()
    {
        bool[] result = {ruleUp, ruleDown, ruleLeft, ruleRight};
        return result;
    }

    // Rotate the room connectivity and rules 90 degrees
    public void RotateRight()
    {
        // Rotate current connections
        bool temp       = connectUp;
        connectUp       = connectLeft;
        connectLeft     = connectDown;
        connectDown     = connectRight;
        connectRight    = temp;

        // Rotate rules of connectivity
        temp            = ruleUp;
        ruleUp          = ruleLeft;
        ruleLeft        = ruleDown;
        ruleDown        = ruleRight;
        ruleRight       = temp;
    }

    /*******************************
     Placement Properties
    *******************************/

    // Set/Get whether this node has been found in generation
    public void SetHasBeenFound(bool value)
    {
        hasBeenFound = value;
    }
    public bool GetHasBeenFound()
    {
        return hasBeenFound;
    }

    // Set/Get grid positioning
    public void SetGridPosition(int X, int Z)
    {
        gridPos = new int[] {X, Z};
    }
    public int[] GetGridPosition()
    {
        return gridPos;
    }

    public void SetIsSpecial(bool val)
    {
        isSpecial = val;
    }
    public bool GetIsSpecial()
    {
        return isSpecial;
    }

    /*******************************
     Visualization
    *******************************/

    // Set connectivity indicators active/inactive depending on connectivity
    public void UpdateVisuals(bool val)
    {
        upInd.SetActive(connectUp && val);
        downInd.SetActive(connectDown && val);
        leftInd.SetActive(connectLeft && val);
        rightInd.SetActive(connectRight && val);
        orb.SetActive(val);
    }

    // Sets the colour of the sphere
    public void SetColor(Color c)
    {
        orb.GetComponent<Renderer>().material.color = c;
    }
}
