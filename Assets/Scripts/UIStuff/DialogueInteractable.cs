using UnityEngine;

public class DialougeInteractable : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string dialogueText;
    [SerializeField] private float interactionRange = 3f;

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            DialogueManager.Instance.CloseDialogue();
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager.Instance.ShowDialogue(dialogueText);
        }
    }

    // Optional: Show visual indicator when player is in range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
