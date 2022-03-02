using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public FlockManager flockManager;
    float speed;
    bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(flockManager.transform.position, flockManager.flyLimits*2);

        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;

        if (!b.Contains(transform.position))
        {
            turning = true;
            // Debug.Log("hit edge of box" + direction);
            direction = flockManager.transform.position - transform.position;
            // Debug.Log("after rotating from box" + direction);
        } 

        else if (Physics.Raycast(transform.position, this.transform.forward * 1f, out hit))
        {
            // Debug.Log("inside the raycast");
            turning = true;
            Debug.Log("raycast detected something" + this.transform.forward + hit.normal);
            // Debug.DrawRay(this.transform.position, this.transform.forward * 1f, Color.red);

            // Debug.Log(direction + "1");
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
            Debug.Log("supposedly new direction" + direction);
            // Debug.Log(direction + "2");
            // Debug.Log("draw rays");
        }
        else 
            turning = false;

        if(turning) 
        {
            Debug.Log(direction);
            Debug.Log(transform.rotation + "before");
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * flockManager.rotationSpeed);
            Debug.Log(transform.rotation + "after");
        }
        else
        {
            if(Random.Range(0,100) < 10)
            {
                speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
            }

            if(Random.Range(0,100) < 20) 
            {
                ApplyRules();
            }
        }
        transform.Translate(0, 0, Time.deltaTime * speed);  // move the object forward along its z axis 1 unit/second.
    }

    void ApplyRules()
    {
        GameObject[] flock;
        flock = flockManager.allBirds;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        float nAngle;
        int groupSize = 0;

        foreach (GameObject bird in flock)
        {
            if (bird != this.gameObject)
            {
                nDistance = Vector3.Distance(bird.transform.position, this.transform.position);
                nAngle = Vector3.Angle(this.transform.forward,bird.transform.position- this.transform.position);
                if(nDistance <= flockManager.neighbourDistance && nAngle<flockManager.neighbourAngle)
                {
                    vCentre += bird.transform.position;
                    groupSize++;

                    if(nDistance < 1.0f) 
                    {
                        vAvoid = vAvoid + (this.transform.position - bird.transform.position);
                    }

                    Flocking anotherFlock = bird.GetComponent<Flocking>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }
        if(groupSize >0)
        {
            vCentre = vCentre/groupSize + (flockManager.goalPos - this.transform.position);
            speed = gSpeed/groupSize;

            Vector3 direction = (vCentre + vAvoid) - transform.position;
            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}

