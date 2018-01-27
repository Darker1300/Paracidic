using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("Config:")]
    public Breaker.DamageType DMGType = Breaker.DamageType.Horizontal;

    public bool IsBroken = false;

    void Awake()
    {
    }

    void Update()
    {
    }

    public void Break()
    {
        if (IsBroken) return;

        IsBroken = true;
    }
}
