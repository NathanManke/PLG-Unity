using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : MonoBehaviour
{
    public GameObject RoomPrefab;
    public float rotation;
    public int width, height;

    [TextArea] public string ConSpec;

    // To specify what parts can connect to other rooms
    public RoomNode[,] RoomNodeMatrix;


    public void Start()
    {
        // Initialize connectivity matrix based on conspec
    }
}
