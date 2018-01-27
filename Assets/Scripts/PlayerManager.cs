using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerController[] Players = new PlayerController[1];
    private CheckPointManager cpm;

    void Start()
    {
        cpm = FindObjectOfType(typeof(CheckPointManager)) as CheckPointManager;
    }

    void Update()
    {

    }

    public void SendPlayerToLastCheckPoint(int index = 0)
    {
        PlayerController pc = Players[index];
        Creature c = pc.Host;
        // If in Host
        if (c != null)
        {
            // Remove from Host
            c.EjectParasite();
        }

        CheckPoint cp = cpm.LastReached;

        // Move Parasite
        GameObject pGo = pc.PLaunch.ParasiteInstance;
        Transform pt = pGo.GetComponent<Transform>();
        Rigidbody2D pkb = pGo.GetComponent<Rigidbody2D>();
        pt.position = cp.transform.position;
        pkb.velocity = Vector2.zero;
    }

    public void KillPlayer(int index = 0)
    {
        PlayerController pc = Players[index];
        Creature c = pc.Host;
        // If in Host
        if (c != null)
        {
            // Remove from Host
            c.EjectParasite();
        }
        // Kill Parasite
        pc.Kill();
    }

    public void KillPlayerHost(int index = 0)
    {
        PlayerController pc = Players[index];
        Creature c = pc.Host;
        // If in Host
        if (c == null) return;

        // Remove from Host
        // Create Parasite
        c.EjectParasite();

        // Kill Host
        c.KillHost();
    }

    public void EjectPlayer(int index = 0)
    {
        PlayerController pc = Players[index];
        Creature c = pc.Host;
        // If in Host
        if (c != null)
        {
            // Remove from Host
            // Create Parasite
            c.EjectParasite();
        }
    }
}
