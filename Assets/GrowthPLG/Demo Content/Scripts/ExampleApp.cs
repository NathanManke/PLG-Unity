using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class app : MonoBehaviour
{
    public GameObject levelGenerator;   // Assign via inspector
    void Update()
    {
        // Do generation with SPACE
        if (Input.GetKeyDown(KeyCode.Space) && levelGenerator.GetComponent<LevelGenerator>())
        {
            levelGenerator.GetComponent<LevelGenerator>().DoGeneration();
        }

        // Toggle the visualization of RoomNodes with R
        if (Input.GetKeyDown(KeyCode.R) && levelGenerator.GetComponent<LevelGenerator>())
        {
            levelGenerator.GetComponent<LevelGenerator>().ToggleVisuals();
        }

        // Print number of real halls
        if (Input.GetKeyDown(KeyCode.P) && levelGenerator.GetComponent<LevelGenerator>())
        {
            Debug.Log(levelGenerator.GetComponent<LevelGenerator>().GetRealHalls().Count);
        }

    }
}