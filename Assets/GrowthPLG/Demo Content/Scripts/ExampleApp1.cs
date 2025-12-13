using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleApp1 : MonoBehaviour
{
    public GameObject levelGenerator;   // Assign via inspector
    private LevelGenerator lg;

    void Start()
    {
        // Generate on start
        lg = levelGenerator.GetComponent<LevelGenerator>();
        lg.DoGeneration();
    }

    void Update()
    {
        // Do another generation with SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lg.DoGeneration();
        }

        // Toggle the visualization of RoomNodes with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            lg.ToggleVisuals();
        }

        // Print number of real halls with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(lg.GetRealHalls().Count);
        }

    }
}