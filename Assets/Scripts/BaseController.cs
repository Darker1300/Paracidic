using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public abstract ControllerType CType { get; }

    public abstract Vector2 Movement { get; }
    public abstract bool TriggerJump { get; }
    public abstract bool IsTryingToJump { get; }
}

public enum ControllerType
{
    Invalid = -1,
    Player,
    Enemy
}