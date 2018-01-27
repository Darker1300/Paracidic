using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteLaunch : MonoBehaviour
{
    [Header("References:")]
    public Camera Cam;
    public Transform UI_Cursor;
    public GameObject ParasitePrefab;

    [Header("Config:")]
    public string InputID_Horizontal = "Horizontal";
    public string InputID_Vertical = "Vertical";
    public float UI_Radius = 1f;
    public float LaunchForce = 5f;

    [Header("Info:")]
    public GameObject ParasiteInstance = null;
    public Transform DrawTransform = null;
    public Vector2 Direction = new Vector2();
    public bool Visible = false;

    void Start()
    {
        if (!Cam) Cam = Camera.main;
        if (!UI_Cursor) UI_Cursor = GetComponentInChildren<SpriteRenderer>().transform;
        if (UI_Cursor) UI_Cursor.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!Visible) return;
        if (!DrawTransform) return;

        Direction = new Vector2(Input.GetAxisRaw(InputID_Horizontal), Input.GetAxisRaw(InputID_Vertical));
        UI_Cursor.right = Direction;
        UI_Cursor.position = (Vector2)(DrawTransform.position + UI_Cursor.right) * UI_Radius;
    }

    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
            return 360f - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1f);
        else
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
    }

    public void ShowUI(Transform Target)
    {
        //UI_Cursor.SetParent(Target, false);
        DrawTransform = Target;
        UI_Cursor.gameObject.SetActive(true);
        Visible = true;
    }

    public void HideUI()
    {
        DrawTransform = null;
        //UI_Cursor.SetParent(transform, false);
        UI_Cursor.gameObject.SetActive(false);
        Visible = false;
    }

    public GameObject CreateParasite(Vector3 position)
    {
        if (ParasitePrefab == null) { Debug.LogError("Prefab Missing."); return null; }

        // Destroy Dupes
        if (ParasiteInstance != null)
            Destroy(ParasiteInstance);

        // Old rot
        // Quaternion q = Quaternion.LookRotation(new Vector3(0f, 0f, Angle(Direction)));

        // Create
        ParasiteInstance = Instantiate(ParasitePrefab, position, Quaternion.identity);

        // Assign fresh reference to PlayerController
        Creature c = ParasiteInstance.GetComponent<Creature>();
        c.PlayerController = ((PlayerManager)FindObjectOfType(typeof(PlayerManager))).Players[0];

        // Force
        Rigidbody2D rb = ParasiteInstance.GetComponent<Rigidbody2D>();
        rb.AddForce(Direction * LaunchForce, ForceMode2D.Impulse);

        // Update Camera Follow
        CameraFollow cf = Cam.GetComponent<CameraFollow>();
        if (cf) cf.target = ParasiteInstance.transform;

        return ParasiteInstance;
    }
}
