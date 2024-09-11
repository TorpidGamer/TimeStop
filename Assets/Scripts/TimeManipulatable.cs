using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TimeManipulatable : MonoBehaviour
{
    Rigidbody rb;
    bool rewinding, stopped, stopAfterRewind;
    public int maxPITs = 600; //time in frames to store
    List<PointInTime> pits;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pits = new List<PointInTime>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!rewinding && !stopped)
        {
            pits.Add(new PointInTime(rb.position, rb.velocity, rb.angularVelocity, rb.rotation));
            stopAfterRewind = false;
            if (pits.Count > maxPITs)
            {
                pits.RemoveAt(0);
            }
        }
        else if (rewinding)
        {
            int pitsNum = pits.Count-1;
            if (pitsNum+1 > 1)
            {
                rb.position = pits[pitsNum].position;
                rb.rotation = pits[pitsNum].rotation;
                pits.RemoveAt(pitsNum);
            }
            else Stop();
        }
    }

    private void OnEnable()
    {
        Player.onStop += Stop;
        Player.onResume += Resume;
        Player.onRewind += Rewind;
        Player.onStopAfterRewind += onStopAfterRewind;
    }

    void Stop()
    {
        if (rewinding) return;
        rb.isKinematic = true;
        stopped = true;
    }

    void Resume()
    {
        stopped = false;
        if (rewinding && stopAfterRewind) { rewinding = false; Stop(); }
        else
        {
            rewinding = false;
            rb.isKinematic = false;
            rb.velocity = pits[pits.Count - 1].velocity;
            rb.angularVelocity = pits[pits.Count - 1].angularV;
        }
    }

    void Rewind()
    {
        if (stopped) stopAfterRewind = true;
        rb.isKinematic = true;
        rewinding = true;
    }

    void onStopAfterRewind()
    {
        stopAfterRewind = true;
    }
}
