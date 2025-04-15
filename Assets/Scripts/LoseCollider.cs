using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseCollider : MonoBehaviour
{
    public BoxCollider boxTriggerCollider;
    void OnTriggerEnter(Collider COllider)
    {
        if (gameObject.CompareTag("Player (Detectable)"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(4);
        }
    }
}
