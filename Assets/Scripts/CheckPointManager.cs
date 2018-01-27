using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public List<CheckPoint> AllCheckpoints = new List<CheckPoint>();
    public CheckPoint LastReached = null;

    void Start()
    {
    }

    void Update()
    {

    }

    public void SetReached(CheckPoint ID)
    {
        LastReached = ID;
    }
}
