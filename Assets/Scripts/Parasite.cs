using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public class Parasite : MonoBehaviour
{
    private static float defaultCooldown = 3.0f;
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.tag == "Creature")
        {
            Creature hitCreature = go.GetComponent<Creature>();

            // Cooldown effect
            if (hitCreature.ParasiteCooldown > 0.0f) return;

            //Take control of this creature
            hitCreature.PlayerController = ((PlayerManager)FindObjectOfType(typeof(PlayerManager))).Players[0];
            hitCreature.PlayerController.Host = hitCreature;
            hitCreature.ParasiteCooldown = defaultCooldown;

            //Tell the camera to follow the new creature
            CameraFollow CF = FindObjectOfType<CameraFollow>();
            CF.target = hitCreature.transform;

            Destroy(gameObject);
        }
    }
}
