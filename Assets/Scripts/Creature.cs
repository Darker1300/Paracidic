using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureMovementMode
{
    None = -1,
    Standard,
    Rotational,
}

public enum CreatureJumpMode
{
    None = -1,
    Standard,
    Rotational,
}

public enum SurfaceAttachedTo
{
    None = -1,
    Up,
    Down,
    Left,
    Right,
}

public enum RayDirection
{
    None = -1,

    Down,
    x1,
    DownLeft,
    x2,
    Left,
    x3,
    UpLeft,
    x4,
    Up,
    x5,
    UpRight,
    x6,
    Right,
    x7,
    DownRight,
    x8,

    MaxDirections
}

public class Creature : MonoBehaviour
{
    [Header("References:")]
    public PlayerController PlayerController;
    public EnemyController HostController;
    public Animator Anim;
    public Rigidbody2D Body2D;
    public Collider2D ShapeCollider;
    public SpriteRenderer SRenderer;

    [Header("Core Creature Data:")]
    [Range(0, 10)]
    public float JumpPower = 5;
    [Range(0.05f, 1)]
    public float JumpStickRayLength = 0.2f;
    [Range(0, 40)]
    public float MovementTopSpeed = 2;
    [Range(0, 25)]
    public float MovementForce = 5;
    [Range(0, 5)]
    public float AirMovementForce = 2;
    
    [Header("Gravity:")]
    [Range(0, 3)]
    public float FastFallGravity = 2f;
    [Range(0, 3)]
    public float HighJumpGravity = 0.75f;
    [Range(0, 3)]
    public float RestingGravity = 1;

    [Header("Extra Creature Data:")]
    [Range(0, 1)]
    public float WallGravity = 0.25f;
    [Range(0, 10)]
    public float WallAttraction = 0;
    [Range(0, 25)]
    public float MaxRotationalSpeed = 10;
    [Range(0, 25)]
    public float MaxRotationalForce = 10;
    public CreatureMovementMode MovementMode;
    public CreatureJumpMode JumpMode;

    [Header("Parasite Data:")]
    // Cooldown/Death
    public float ParasiteCooldown = 0.0f;
    [Range(0, 5)]
    public float ParasiteAttactRadius = 0.0f;
    [Range(0, 25)]
    public float ParasiteAttactForce = 0.0f;
    public LinkedList<Collider2D> IgnoredColliders = new LinkedList<Collider2D>();


    public BaseController CurrentController
    {
        get
        {
            if (PlayerController != null)
                return PlayerController;
            return HostController;
        }
    }

    public List<RaycastHit2D> LastTerrainHits { get; private set; }
    //List<RaycastHit2D> LastTerrainHits = new List<RaycastHit2D>((int)RayDirection.MaxDirections);

    private bool HasTraction;

    void Awake()
    {
        HostController = gameObject.AddComponent<EnemyController>();

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
            PlayerController = pc;

        if (!Anim) Anim = GetComponent<Animator>();
        if (!Body2D) Body2D = GetComponent<Rigidbody2D>();
        if (!ShapeCollider) ShapeCollider = GetComponent<Collider2D>();
        if (!SRenderer) SRenderer = GetComponent<SpriteRenderer>();

        LastTerrainHits = new List<RaycastHit2D>((int)RayDirection.MaxDirections);
    }

    bool IsOnWall;
    Vector2 AverageSurfaceNormal = new Vector2(0, 0);

    void Update()
    {
        LastTerrainHits.Clear();
        for (int i = 0; i < (int)RayDirection.MaxDirections; i++)
        {
            Vector2 Direction = DegreeToVector2((22.5f * i) - 90);

            Vector2 rayStart = Body2D.position;

            RaycastHit2D hitInfo;
            ShapeCollider.Raycast(new Ray2D(rayStart + (Direction * 10), -Direction), out hitInfo, 10f);
            DrawStarDebug(hitInfo.point);

            LastTerrainHits.Add(Physics2D.Raycast(hitInfo.point, Direction, JumpStickRayLength, LayerMask.GetMask("Terrain", "DynamicObject")));
            Debug.DrawLine(ToVector3(hitInfo.point), ToVector3(hitInfo.point) + ToVector3(Direction * JumpStickRayLength), Color.yellow);
        }

        HasTraction = LastTerrainHits[0].collider != null;

        // Player Parasite Cooldown
        if (ParasiteCooldown > 0.0f)
        {
            ParasiteCooldown -= Time.deltaTime;

            // End
            if (ParasiteCooldown < 0.0f)
            {
                ParasiteCooldown = 0.0f;

                foreach (Collider2D c in IgnoredColliders)
                    if (c != null)
                        Physics2D.IgnoreCollision(ShapeCollider, c, false);
                IgnoredColliders.Clear();
            }
        }

        // Player Parasite Targeting Logic
        if (PlayerController != null)
        {
            if (PlayerController.BeginTargetCreature)
                if (!PlayerController.PLaunch.Visible)
                    PlayerController.PLaunch.ShowUI(transform);

            if (PlayerController.FinishTargetCreature)
            {
                HostController.AICooldown = UnityEngine.Random.Range(2, 3f);

                PlayerController.PLaunch.HideUI();

                EjectParasite();
            }
        }

        //Jump Logic
        switch (JumpMode)
        {
            case CreatureJumpMode.Standard:
                if (CurrentController.TriggerJump && LastTerrainHits[0].collider != null)
                {
                    if (Body2D.velocity.y < 0.1f)
                    {
                        if (Body2D.velocity.y < 0)
                            Body2D.velocity = new Vector2(Body2D.velocity.x, 0);
                        Body2D.AddForce(LastTerrainHits[0].normal * 100 * JumpPower * Body2D.mass);
                    }
                }
                break;
            case CreatureJumpMode.Rotational:
                //Provide attraction force
                IsOnWall = false;

                int ASNCount = 0;
                AverageSurfaceNormal = Vector2.zero;
                foreach (var item in LastTerrainHits)
                {
                    if (item.collider != null)
                    {
                        AverageSurfaceNormal += item.normal;
                        ASNCount++;
                    }
                }

                if (ASNCount > 0)
                {
                    //We are touching a surface, run glue logic and jump logic
                    AverageSurfaceNormal /= ASNCount;
                    AverageSurfaceNormal.Normalize();

                    //Glue logic
                    foreach (var item in LastTerrainHits)
                    {
                        if (item.collider != null && item.normal.y <= 0.1f)
                        {
                            IsOnWall = true;
                            Body2D.AddForce(-item.normal * Body2D.mass * WallAttraction * 10);
                            Debug.DrawLine(ToVector3(item.point), ToVector3(item.point + (-item.normal * 1)), Color.cyan);
                        }
                    }

                    //Jump Logic
                    if (CurrentController.TriggerJump)
                    {
                        if (Body2D.velocity.y < 0.1f)
                        {
                            if (Body2D.velocity.y < 0)
                                Body2D.velocity = new Vector2(Body2D.velocity.x, 0);
                            Body2D.AddForce(AverageSurfaceNormal * 100 * JumpPower * Body2D.mass);
                        }
                    }
                }

                break;
        }
    }

    private void DrawStarDebug(Vector2 point)
    {
        for (int i = 0; i < (int)RayDirection.MaxDirections; i++)
        {
            Vector2 Direction = DegreeToVector2((45f * i) - 90);
            Debug.DrawLine(ToVector3(point), ToVector3(point + (Direction * 0.05f)));
        }
    }

    void FixedUpdate()
    {
        SuckInThePlayer();

        switch (MovementMode)
        {
            case CreatureMovementMode.Standard:
                if (HasTraction)
                    Body2D.AddForce(new Vector2(CurrentController.Movement.x, 0f) * Time.fixedDeltaTime * MovementForce * 300 * Body2D.mass);
                else
                    Body2D.AddForce(new Vector2(CurrentController.Movement.x, 0f) * Time.fixedDeltaTime * AirMovementForce * 800 * Body2D.mass);
                break;
            case CreatureMovementMode.Rotational:
                if (Mathf.Abs(Body2D.angularVelocity) < MaxRotationalSpeed * 100 && AverageSurfaceNormal != Vector2.zero)
                {
                    SurfaceAttachedTo SAT = SurfaceAttachedTo.None;
                    if (AverageSurfaceNormal.y > 0 && AverageSurfaceNormal.y > Mathf.Abs(AverageSurfaceNormal.x))
                        SAT = SurfaceAttachedTo.Down;
                    if (AverageSurfaceNormal.y < 0 && AverageSurfaceNormal.y < -Mathf.Abs(AverageSurfaceNormal.x))
                        SAT = SurfaceAttachedTo.Up;
                    if (AverageSurfaceNormal.x > 0 && AverageSurfaceNormal.x > Mathf.Abs(AverageSurfaceNormal.y))
                        SAT = SurfaceAttachedTo.Left;
                    if (AverageSurfaceNormal.x < 0 && AverageSurfaceNormal.x < -Mathf.Abs(AverageSurfaceNormal.y))
                        SAT = SurfaceAttachedTo.Right;

                    //Debug.Log("SAT : " + SAT);                    

                    switch (SAT)
                    {
                        case SurfaceAttachedTo.Up:
                            Body2D.AddForce(new Vector2(CurrentController.Movement.x, 0f) * Time.fixedDeltaTime * 50 * Body2D.mass * MaxRotationalForce);
                            break;
                        case SurfaceAttachedTo.Down:
                            Body2D.AddForce(new Vector2(CurrentController.Movement.x, 0f) * Time.fixedDeltaTime * 50 * Body2D.mass * MaxRotationalForce);
                            break;
                        case SurfaceAttachedTo.Left:
                            Body2D.AddForce(new Vector2(0f, CurrentController.Movement.y) * Time.fixedDeltaTime * 50 * Body2D.mass * MaxRotationalForce);
                            break;
                        case SurfaceAttachedTo.Right:
                            Body2D.AddForce(new Vector2(0f, CurrentController.Movement.y) * Time.fixedDeltaTime * 50 * Body2D.mass * MaxRotationalForce);
                            break;
                    }

                    Body2D.AddForce(new Vector2(CurrentController.Movement.x, CurrentController.Movement.y) * Time.fixedDeltaTime * 200 * Body2D.mass);
                    //if (HasTraction)
                    //    Body2D.AddTorque(-CurrentController.Movement.x * Time.fixedDeltaTime * MovementSpeed * 100 * Body2D.mass);
                    //else
                    //    Body2D.AddTorque(-CurrentController.Movement.x * Time.fixedDeltaTime * MovementSpeed * 10 * Body2D.mass);
                }
                break;
        }

        if (Body2D.velocity.magnitude > MovementTopSpeed)
        {
            Body2D.velocity = Body2D.velocity.normalized;
            Body2D.velocity *= MovementTopSpeed;
        }

        if (IsOnWall)
            Body2D.gravityScale = WallGravity;
        else if (Body2D.velocity.y < 0)
            Body2D.gravityScale = FastFallGravity;
        else if (CurrentController.IsTryingToJump)
            Body2D.gravityScale = HighJumpGravity;
        else
            Body2D.gravityScale = RestingGravity;
    }

    private void SuckInThePlayer()
    {
        if (ParasiteAttactRadius == 0)
            return;

        Collider2D hit = Physics2D.OverlapCircle(Body2D.position, ParasiteAttactRadius, LayerMask.NameToLayer("Parasite"));

        //get the offset between the objects
        Vector3 offset = hit.transform.position - transform.position;
        //we're doing 2d physics, so don't want to try and apply z forces!
        offset.z = 0;

        //get the squared distance between the objects
        float magsqr = offset.sqrMagnitude;

        //check distance is more than 0 (to avoid division by 0) and then apply a gravitational force to the object
        //note the force is applied as an acceleration, as acceleration created by gravity is independent of the mass of the object
        if (magsqr > 0.0001f)
            Body2D.AddForce(ParasiteAttactForce * 100 * offset.normalized / magsqr, ForceMode2D.Force);
            //hit.GetComponent<Rigidbody2D>().AddForce(ParasiteAttactForce * -100 * offset.normalized / magsqr, ForceMode2D.Force);
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static float Distance(float value1, float value2)
    {
        return Math.Abs(value1 - value2);
    }

    private Vector3 ToVector3(Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y, 0);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Knockback(collision);
    }

    void Knockback(Collision2D collision)
    {
        Knockbackable k = collision.gameObject.GetComponent<Knockbackable>();
        if (k)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                k.RB2D.AddForceAtPosition((-collision.contacts[i].normal) * (k.KnockbackForce * 1f / collision.contacts.Length), collision.contacts[i].point, ForceMode2D.Impulse);
            }
        }
    }

    public bool EjectParasite()
    {
        if (PlayerController == null) return false;

        PlayerController.PLaunch.CreateParasite(transform.position);

        // Get reliable ParasiteLaunch reference
        ParasiteLaunch pl = ((PlayerManager)FindObjectOfType(typeof(PlayerManager))).Players[0].PLaunch;
        GameObject newInstance = pl.CreateParasite(transform.position);

        // Set Physics Cooldown
        ParasiteCooldown = 3.0f;
        var cols = newInstance.GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            Physics2D.IgnoreCollision(ShapeCollider, c);
            IgnoredColliders.AddLast(c);
        }

        // Clear from host
        PlayerController.Host = null;
        PlayerController = null;

        return true;
    }

    public void KillHost()
    {

    }
}

public static class Physics2DExtension
{
    public static bool Raycast(this Collider2D collider, Ray2D ray, out RaycastHit2D hitInfo, float maxDistance)
    {
        var oriLayer = collider.gameObject.layer;
        const int tempLayer = 31;
        collider.gameObject.layer = tempLayer;
        hitInfo = Physics2D.Raycast(ray.origin, ray.direction, maxDistance, 1 << tempLayer);
        collider.gameObject.layer = oriLayer;
        if (hitInfo.collider && hitInfo.collider != collider)
        {
            Debug.LogError("Collider2D.Raycast() need a unique temp layer to work! Make sure Layer #" + tempLayer + " is unused!");
            return false;
        }
        return hitInfo.collider != null;
    }
}