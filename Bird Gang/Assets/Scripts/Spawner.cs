using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.IO;


public class Spawner : MonoBehaviour
{
    public int NumberOfPeopleTotal;
    public int NumberOfGoodPeople;
    public int NumberOfBadPeople;
    public int NumberOfMiniBoss;
    public Renderer spawnRenderer;

    private int NumberGoodPeopleSpawned;
    private int NumberBadPeopleSpawned;
    private int NumberMiniBossSpawned;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    private Vector3 centerPosition;

    private List<GameObject> miniBosses = new List<GameObject>();

    PhotonView PV;
    Transform child;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        NumberOfPeopleTotal = NumberGoodPeopleSpawned + NumberBadPeopleSpawned;

        minPosition = spawnRenderer.bounds.min;
        maxPosition = spawnRenderer.bounds.max;
        centerPosition = spawnRenderer.bounds.center;
    }

    // Update is called once per frame
    void Update()
    {
        NumberOfPeopleTotal = NumberGoodPeopleSpawned + NumberBadPeopleSpawned;
    }

    private void SpawnGoodPerson()
    {
        Vector3 position = centerPosition;// Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","Good Person Cube"),position,Quaternion.identity);
        NumberGoodPeopleSpawned++;
    }

    private void SpawnBadPerson()
    {
        Vector3 position = centerPosition;// Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bad Person Cube"), position, Quaternion.identity);
        NumberBadPeopleSpawned++;
    }
    private void SpawnMiniBoss()
    {
        Vector3 position = centerPosition;// + new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));
        GameObject newMiniBoss = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MiniBoss Cube"), position, Quaternion.identity);
        miniBosses.Add(newMiniBoss);
        NumberMiniBossSpawned++;
    }

    public void fillMaxGoodPeople(int numOfPeople)
    {
        NumberOfGoodPeople = numOfPeople;

        while (NumberGoodPeopleSpawned < NumberOfGoodPeople)
        {
            SpawnGoodPerson();
        }


    }

    public void fillMaxBadPeople(int numOfPeople)
    {
        NumberOfBadPeople = numOfPeople;

        while (NumberBadPeopleSpawned < NumberOfBadPeople)
        {
            SpawnBadPerson();
        }
    }

    public void fillMaxMiniBoss(int numOfPeople)
    {
        NumberOfMiniBoss = numOfPeople;

        while (NumberMiniBossSpawned < NumberOfMiniBoss)
        {
            SpawnMiniBoss();
        }
    }

    public void destroyMiniBosses()
    {
        foreach (GameObject mb in miniBosses)
        {
            if (mb)
                PhotonNetwork.Destroy(mb);
        }
        miniBosses.Clear();
    }

    public int GetNumberOfMiniBoss()
    {
        return NumberOfMiniBoss;
    }
    public int GetNumberOfGoodPeople()
    {
        return NumberOfGoodPeople;
    }
    public int GetNumberOfBadPeople()
    {
        return NumberOfBadPeople;
    }
    public void DecrementGoodPeople()
    {
        NumberGoodPeopleSpawned--;
    }
    public void DecrementBadPeople()
    {
        NumberBadPeopleSpawned--;
    }
}
