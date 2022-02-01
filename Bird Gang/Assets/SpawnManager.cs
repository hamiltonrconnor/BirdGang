using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnManager : MonoBehaviour
{
     PeopleSpawner[] spawners;
    // Start is called before the first frame update
    void Start()
    {
        spawners= GetComponentsInChildren<PeopleSpawner>();
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var spawner in spawners)
        {
            spawner.SetNumberOfGoodPeople(15);
            spawner.SetNumberOfBadPeople(15);
        }
    }
}
