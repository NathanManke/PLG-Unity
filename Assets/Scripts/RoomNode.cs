using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : MonoBehaviour
{
    /* Whether the node is connected in any direction */
    [SerializeField] private bool connectUp         = false;
    [SerializeField] private bool connectDown       = false;
    [SerializeField] private bool connectLeft       = false;
    [SerializeField] private bool connectRight      = false;

    /* Rules of which directions the node can connect in */
    [SerializeField] private bool ruleUp            = false;
    [SerializeField] private bool ruleDown          = false;
    [SerializeField] private bool ruleLeft          = false;
    [SerializeField] private bool ruleRight         = false;

    /* Position on the map grid */
    [SerializeField] private int[] gridPos          = {0, 0};

    /* Whether this node has been connected into the rest of the map yet */
    [SerializeField] private bool hasBeenFound      = false;

    /* To store the game objects used to indicate connectivity in each direction */
    private GameObject upInd, downInd, leftInd, rightInd, orb;


    void Awake()
    {
        /* Get references to child direction indicators */
        upInd       = transform.Find("Container/Up").gameObject;
        downInd     = transform.Find("Container/Down").gameObject;
        leftInd     = transform.Find("Container/Left").gameObject;
        rightInd    = transform.Find("Container/Right").gameObject;
        orb         = transform.Find("Container/Sphere").gameObject;

        Debug.Log(orb == null);

        /* Set connectivity if values provided */
        SetConnectUp(connectUp);
        SetConnectDown(connectDown);
        SetConnectLeft(connectRight);
        SetConnectRight(connectRight);
    }

    /*******************************
        Connectivity Functions
    *******************************/

    /* Set UDLR connectivity */
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

    /* Set UDLR rules */
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
    public void RotateRight()
    {
        /* Rotate current connections */
        bool temp       = connectUp;
        connectUp       = connectLeft;
        connectLeft     = connectDown;
        connectDown     = connectRight;
        connectRight    = temp;

        /* Rotate rules of connectivity */
        temp            = ruleUp;
        ruleUp          = ruleLeft;
        ruleLeft        = ruleDown;
        ruleDown        = ruleRight;
        ruleRight       = temp;
    }

    /* Get/set whether this node has been found in generation */
    public void SetHasBeenFound(bool value)
    {
        hasBeenFound = value;
    }
    public bool GetHasBeenFound()
    {
        return hasBeenFound;
    }

    /* Grid positioning */
    public void SetGridPosition(int X, int Z, float roomScale)
    {
        gridPos = new int[] {X, Z};
        transform.position = new Vector3(X * roomScale, 0, Z * roomScale);
        transform.Find("Container").localScale = Vector3.one * roomScale;
    }
    public int[] GetGridPosition()
    {
        return gridPos;
    }

    /*******************************
        Visualization
    *******************************/
    public void UpdateVisuals()
    {
        upInd.SetActive(connectUp);
        downInd.SetActive(connectDown);
        leftInd.SetActive(connectLeft);
        rightInd.SetActive(connectRight);
    }

    public void SetColor(Color c)
    {
        if (orb == null) { Debug.Log("wtf"); return; }
        orb.GetComponent<Renderer>().material.color = c;
    }
}
