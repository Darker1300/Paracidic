using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : MonoBehaviour
{
    public enum DamageType
    {
        Horizontal,
        Vertical
    }

    private readonly RayDirection[] HorizontalRays = new RayDirection[] { RayDirection.Left, RayDirection.Right };

    private readonly RayDirection[] VerticalRays = new RayDirection[] { RayDirection.Up };

    public Creature creature;

    public DamageType DMGType = DamageType.Horizontal;

    void Start()
    {
        if (!creature) creature = GetComponent<Creature>();
    }

    void Update()
    {
        List<RaycastHit2D> lastTerrainHits = creature.LastTerrainHits;//[(int)RayDirection.Up];

        foreach (RayDirection rd in HorizontalRays)
        {
            RaycastHit2D rh = lastTerrainHits[(int)rd];
            if (rh.collider == null) continue;

            // Evaluate in direction
            //
        }
    }
}
