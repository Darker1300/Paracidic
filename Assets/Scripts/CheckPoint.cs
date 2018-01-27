using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private CheckPointManager cpm;
    private SpriteRenderer rend;

    [Header("Config:")]
    public bool Checked = false;
    public bool StartingCheckPoint = false;

    void Awake()
    {
        cpm = FindObjectOfType(typeof(CheckPointManager)) as CheckPointManager;
        rend = GetComponent<SpriteRenderer>();
        cpm.AllCheckpoints.Add(this as CheckPoint);
        if (StartingCheckPoint) cpm.SetReached(this);
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (Checked) return;

        Checked = true;
        cpm.SetReached(this);
        OnCheckPointReached(collision);
    }

    void OnCheckPointReached(Collider2D collision)
    {
        // DO VISUAL
        Color c = rend.color;
        Vector3 hsv = new Vector3();
        Color.RGBToHSV(c, out hsv.x, out hsv.y, out hsv.z);

        // HSV Change
        hsv.z += 0.20f;

        Color o = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
        rend.color = o;
    }
}
