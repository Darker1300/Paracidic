using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    public string InputID_Horizontal = "Horizontal";
    public string InputID_Vertical = "Vertical";
    public string InputID_Jump = "Jump";
    public string InputID_Exit = "ExitCreature";

    public ParasiteLaunch PLaunch;
    public Creature Host = null;

    public void Awake()
    {
        if (!PLaunch) PLaunch = GetComponentInChildren<ParasiteLaunch>();
    }

    public override ControllerType CType
    {
        get { return ControllerType.Player; }
    }

    public override Vector2 Movement
    {
        get
        {
            if (IsTargetingCreature)
                return Vector2.zero;
            return new Vector2(Input.GetAxisRaw(InputID_Horizontal), Input.GetAxisRaw(InputID_Vertical));
        }
    }

    public override bool TriggerJump
    {
        get
        {
            if (IsTargetingCreature)
                return false;
            return Input.GetButtonDown(InputID_Jump);
        }
    }

    public override bool IsTryingToJump
    {
        get
        {
            if (IsTargetingCreature)
                return false;
            return Input.GetButton(InputID_Jump);
        }
    }

    public bool IsTargetingCreature
    {
        get
        {
            return Input.GetButton(InputID_Exit);
        }
    }

    public bool BeginTargetCreature
    {
        get
        {
            return Input.GetButtonDown(InputID_Exit);
        }
    }

    public bool FinishTargetCreature
    {
        get
        {
            return Input.GetButtonUp(InputID_Exit);
        }
    }

    public void Kill()
    {
        Debug.Log("Parasite Dead!");
    }
}
