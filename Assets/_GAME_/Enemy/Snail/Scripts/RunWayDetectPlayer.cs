using UnityEngine;

public class SnailPlayerDetect : MonoBehaviour
{
    [SerializeField] private StickySnailController snailController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            snailController.StartFleeing();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            snailController.StopFleeing();
        }
    }
}