using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splatter : MonoBehaviour
{
//    private MeshRenderer meshRenderer;
    public ParticleSystem particleSystem;
    private bool burst;
//    private Material material;
    private const float Lifetime = 30f;
    private float endTime;

    public float appearTime;

    private void Awake()
    {
        endTime = Time.time + Lifetime;
        // Rely on other script to set appear time.
        if (appearTime == 0)
            appearTime = endTime;
        var e = particleSystem.emission;
        e.rateOverTime = 0;
        burst = true;
    }

    private void Start()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        //material = GetComponent<Renderer>().material;

        //material.SetFloat("_index", Random.RandomRange(0, 4));

    }

    void Update()
    {
        if (Time.time > endTime)
        {
            Destroy(this.gameObject);
        }
        else if (Time.time > appearTime)
        {
            particleSystem.gameObject.SetActive(true);
            if (burst)
            {
                particleSystem.Emit(1);
                burst = false;
            }
        }
    }
}
