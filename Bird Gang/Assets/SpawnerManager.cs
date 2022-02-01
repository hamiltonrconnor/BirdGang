using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    
    public List<PeopleSpawner> spawners;
    // Start is called before the first frame update
    void Start()
    {
        
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
