using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class app : MonoBehaviour
{
    public GameObject levelGenerator;

    void Start()
    {
    }

    void Update()
    {
        
        // Do generation with SPACE
        if (Input.GetKeyDown(KeyCode.Space) && levelGenerator.GetComponent<LevelGenerator>())
        {
            levelGenerator.GetComponent<LevelGenerator>().DoGeneration(true);
        }

        // Clear the generation objects after the fact
        if (Input.GetKeyDown(KeyCode.R) && levelGenerator.GetComponent<LevelGenerator>())
        {
            levelGenerator.GetComponent<LevelGenerator>().ClearGenerationObjects();
        }

    }
}