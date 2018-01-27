using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetCenterOfMass : MonoBehaviour
{
    [Header("References:")]
    public Rigidbody2D RB2D;

    [Header("Config:")]
    public Vector2 WorldCenter;
    public bool Reset = false;

    [Header("Debug:")]
    public bool DrawCenter = true;
    private Vector2 OldCenter = new Vector2();

    void Start()
    {
        if (!RB2D) RB2D = GetComponent<Rigidbody2D>();
        OldCenter = RB2D.centerOfMass;
        WorldCenter = RB2D.centerOfMass;
    }

    private void OnValidate()
    {
        if (!this.enabled) return;
        if (!Application.isEditor) return;
        if (!RB2D) RB2D = GetComponent<Rigidbody2D>();

        if (Reset) { WorldCenter = OldCenter; Reset = false; }

        Vector2 c = RB2D.centerOfMass;
        RB2D.centerOfMass = WorldCenter;
    }

    private void OnDrawGizmosSelected()
    {
        if (!RB2D) return;

        if (!DrawCenter) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(RB2D.worldCenterOfMass.x, RB2D.worldCenterOfMass.y, transform.position.z), 0.025f);
    }
}
