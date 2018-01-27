using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Knockbackable : MonoBehaviour
{
    [Header("References:")]
    public Rigidbody2D RB2D;
    [Header("Config:")]
    public float KnockbackForce = 5f;

    void Start()
    {
        if (!RB2D) RB2D = GetComponent<Rigidbody2D>();
    }
}
