using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatforms : MonoBehaviour
{
    [SerializeField] private bool isHorizontal = true;  // True for horizontal, False for vertical movement
    [SerializeField] private float speed = 5f;         // Movement speed
    [SerializeField] private float distance = 10f;     // Distance to move in either direction

    private Vector3 startPosition;
    private float movementFactor;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the movement using a sine wave for smooth back-and-forth motion
        float time = Time.time;
        movementFactor = Mathf.Sin(time * speed);

        Vector3 offset = isHorizontal ?
            new Vector3(movementFactor * distance, 0f, 0f) :
            new Vector3(0f, movementFactor * distance, 0f);

        transform.position = startPosition + offset;
    }
}
