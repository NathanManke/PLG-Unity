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
        if (Input.GetKeyDown(KeyCode.R) && levelGenerator.GetComponent<LevelGenerator>())
        {
            levelGenerator.GetComponent<LevelGenerator>().InitializeRooms();
        }

        if (Input.GetKeyDown(KeyCode.Space) && levelGenerator.GetComponent<LevelGenerator>())
        {
            levelGenerator.GetComponent<LevelGenerator>().GenerateRooms();
        }

        if (Input.GetKeyDown(KeyCode.P) && levelGenerator.GetComponent<LevelGenerator>())
        {
            levelGenerator.GetComponent<LevelGenerator>().PickAndPlace();
        }
    }
}
