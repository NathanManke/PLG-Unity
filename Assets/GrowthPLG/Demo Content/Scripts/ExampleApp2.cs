using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleApp2 : MonoBehaviour
{
    public GameObject levelGenerator1;   // Assign via inspector
    public GameObject levelGenerator2;   // Assign via inspector
    public GameObject levelGenerator3;   // Assign via inspector
    private LevelGenerator lg1;
    private LevelGenerator lg2;
    private LevelGenerator lg3;

    void Start()
    {
        // Generate on start
        lg1 = levelGenerator1.GetComponent<LevelGenerator>();
        lg1.DoGeneration();
        lg2 = levelGenerator2.GetComponent<LevelGenerator>();
        lg2.DoGeneration();
        lg3 = levelGenerator3.GetComponent<LevelGenerator>();
        lg3.DoGeneration();
    }

    void Update()
    {
        // Do another generation with SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lg1.DoGeneration();
            lg2.DoGeneration();
            lg3.DoGeneration();
        }

        // Toggle the visualization of RoomNodes with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            lg1.ToggleVisuals();
            lg2.ToggleVisuals();
            lg3.ToggleVisuals();
        }

        // Print number of real halls with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(lg1.GetRealHalls().Count);
            Debug.Log(lg2.GetRealHalls().Count);
            Debug.Log(lg3.GetRealHalls().Count);
        }

    }
}