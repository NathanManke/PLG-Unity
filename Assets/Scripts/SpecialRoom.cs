using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : MonoBehaviour
{
    public GameObject RoomPrefab;
    // public int width, height;

    [TextArea] public string ConSpec;

    // To specify what parts can connect to other rooms
    // public RoomNode[,] RoomNodeMatrix;
    public GameObject roomNodePrefab;
    public RoomNode localNode;


    public void Awake()
    {
        localNode = Instantiate(roomNodePrefab).GetComponent<RoomNode>();
        localNode.SetIsSpecial(true);

        /* For now, assume it only connects in one direction */
        localNode.SetRuleUp(false);
        localNode.SetRuleDown(false);
        localNode.SetRuleLeft(false);
        localNode.SetRuleRight(false);
        localNode.SetColor(Color.green);
    }
}
