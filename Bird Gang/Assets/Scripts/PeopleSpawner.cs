using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpawner : MonoBehaviour
{
    public int NumberOfPeopleTotal;
    public int NumberOfGoodPeople;
    public int NumberOfBadPeople;
    public GameObject badPersonPrefab;
    public GameObject goodPersonPrefab;
  
    private int NumberGoodPeopleSpawned;
    private int NumberBadPeopleSpawned;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    private Vector3 centerPosition;
    public Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        NumberOfPeopleTotal = 50;
        
        minPosition = renderer.bounds.min;
        maxPosition = renderer.bounds.max;
        centerPosition = renderer.bounds.max;
    }

    // Update is called once per frame
    void Update()
    {   
        if (NumberGoodPeopleSpawned < NumberOfGoodPeople)
        {
            SpawnGoodPerson();
        }
        if (NumberBadPeopleSpawned < NumberOfBadPeople)
        {
            SpawnBadPerson();
        }
    }

    private void SpawnGoodPerson()
    {
        GameObject newGoodPerson = Instantiate(goodPersonPrefab);
        newGoodPerson.transform.position = centerPosition
            + new Vector3(Random.Range(minPosition.x, maxPosition.x), 1, Random.Range(minPosition.z, maxPosition.z));
        
        NumberGoodPeopleSpawned++;
    } 

    private void SpawnBadPerson()
    {
        GameObject newBadPerson = Instantiate(badPersonPrefab);
        newBadPerson.transform.position = centerPosition
            + new Vector3(Random.Range(minPosition.x, maxPosition.x), 1, Random.Range(minPosition.z, maxPosition.z));
        NumberBadPeopleSpawned++;
    }
    public void SetNumberOfGoodPeople(int numOfPeople)
    {
        NumberOfGoodPeople = numOfPeople;
    }
    public void SetNumberOfBadPeople(int numOfPeople)
    {
        NumberOfBadPeople = numOfPeople;
    }
    public int GetNumberOfGoodPeople(int numOfPeople)
    {
        return NumberOfGoodPeople;
    }
    public int GetNumberOfBadPeople(int numOfPeople)
    {
        return NumberOfBadPeople;
    }
}
