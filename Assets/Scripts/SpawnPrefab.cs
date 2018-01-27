using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    [Header("References:")]
    public GameObject prefab = null;

    [Header("Config:")]
    public bool DestroySelfOnSpawn = true;

    void Awake()
    {
        if (!prefab) return;
        Instantiate<GameObject>(prefab, transform.position, transform.rotation,
            transform.parent != null ? transform.parent : transform.root);

        if (DestroySelfOnSpawn)
            Destroy(this.gameObject);
    }
}
