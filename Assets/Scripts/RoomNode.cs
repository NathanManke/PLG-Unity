using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : MonoBehaviour
{
    [SerializeField] private bool connectUp     = false;
    [SerializeField] private bool connectDown   = false;
    [SerializeField] private bool connectLeft   = false;
    [SerializeField] private bool connectRight  = false;

    [SerializeField] private int[] gridPos;
    private GameObject up, down, left, right;

    // Start is called before the first frame update
    void Start()
    {
        // Get references to child direction indicators
        up      = transform.Find("Container/Up").gameObject;
        down    = transform.Find("Container/Down").gameObject;
        left    = transform.Find("Container/Left").gameObject;
        right   = transform.Find("Container/Right").gameObject;
        // Set connection based on indicators
        SetConnectUp(connectUp);
        SetConnectDown(connectDown);
        SetConnectLeft(connectRight);
        SetConnectRight(connectRight);
        gridPos = new int[] {0, 0};
    }

    /*******************************
     Coonectivity Functions
    *******************************/

    /* UDLR */
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

    /* Grid positioning */
    public void SetGridPosition(int row, int col, float roomScale)
    {
        gridPos = new int[] {row, col};
        transform.position = new Vector3(row * roomScale, 0, col * roomScale);
        transform.localScale = Vector3.one * roomScale;
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
        up.SetActive(connectUp);
        down.SetActive(connectDown);
        left.SetActive(connectLeft);
        right.SetActive(connectRight);
    }

}
