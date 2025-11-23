using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : MonoBehaviour
{
    [SerializeField] private bool connectUp     = false;
    [SerializeField] private bool connectDown   = false;
    [SerializeField] private bool connectLeft   = false;
    [SerializeField] private bool connectRight  = false;

    private GameObject up, down, left, right;

    // Start is called before the first frame update
    void Start()
    {
        // Get references to child direction indicators
        up      = transform.Find("Up").gameObject;
        down    = transform.Find("Down").gameObject;
        left    = transform.Find("Left").gameObject;
        right   = transform.Find("Right").gameObject;
        // Set connection based on indicators
        SetConnectUp(connectUp);
        SetConnectDown(connectDown);
        SetConnectLeft(connectRight);
        SetConnectRight(connectRight);
    }

    /*******************************
     Public visualization functions
    *******************************/
    public bool[] GetEnabledRooms()
    {
        bool[] result = {connectUp, connectDown, connectLeft, connectRight};
        return result;
    }
    public void UpdateVisuals()
    {
        // Set up/down/left/right accordingly
    }

    /*******************************
     Set or unset UDLR connections
    *******************************/
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

}
