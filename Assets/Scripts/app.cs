using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class app : MonoBehaviour
{
    public GameObject levelGenerator;
    // Update is called once per frame
    public List<GameObject> sr;

    void Start()
    {
        levelGenerator.GetComponent<LevelGenerator>().SetSpecialRoomsToPlace(sr);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && levelGenerator.GetComponent<LevelGenerator>().canGenerate)
        {
            levelGenerator.GetComponent<LevelGenerator>().InitializeRooms();
        }

        if (Input.GetKeyDown(KeyCode.Space) && levelGenerator.GetComponent<LevelGenerator>().canGenerate)
        {
            levelGenerator.GetComponent<LevelGenerator>().GenerateRooms();
        }
    }
}
