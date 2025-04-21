using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifespan : MonoBehaviour
{
    public float lifetime = 2f;
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
