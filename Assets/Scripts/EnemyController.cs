using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    public float AICooldown = 0;

    public override ControllerType CType
    {
        get { return ControllerType.Enemy; }
    }

    public override Vector2 Movement
    {
        get
        {
            float AICooldownClamped = 1 - AICooldown;
            if (AICooldownClamped < 0)
                return Vector2.zero;
            if (AICooldownClamped > 1)
                AICooldownClamped = 1;
            return Vector2.ClampMagnitude(new Vector2(Mathf.Sin(Time.time), 0), 1) * AICooldownClamped;
        }
    }

    public override bool TriggerJump
    {
        get
        {
            if (AICooldown > 0)
                return false;
            return Mathf.Sin(Time.time * 0.5f) > 0.97f;
        }
    }
    public override bool IsTryingToJump
    {
        get
        {
            if (AICooldown > 0)
                return false;
            return Mathf.Sin(Time.time * 0.5f) > 0.97f;
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (AICooldown > 0)
            AICooldown -= Time.deltaTime;
    }
}
