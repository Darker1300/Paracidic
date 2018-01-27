using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    private SpriteRenderer rend;

    [Header("Config:")]
    public bool Checked = false;
    [Header("Debug:")]
    public bool Activate = false;

    public UnityEvent OnActivate = new UnityEvent();

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Activate && !Checked)
        {
            Checked = true;
            Activate = false;
            OnSwitch();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (Checked) return;

        Checked = true;
        OnSwitch();
    }

    void OnSwitch()
    {
        // DO VISUAL
        Color c = rend.color;
        Vector3 hsv = new Vector3();
        Color.RGBToHSV(c, out hsv.x, out hsv.y, out hsv.z);

        // HSV Change
        hsv.x += 0.20f;

        Color o = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
        rend.color = o;
        // -----

        OnActivate.Invoke();
    }
}
