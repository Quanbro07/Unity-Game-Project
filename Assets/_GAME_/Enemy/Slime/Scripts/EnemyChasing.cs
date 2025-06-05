using UnityEngine;

public class EnemyChasing : MonoBehaviour
{
    [SerializeField] private SlimeController slimeController;

    bool LOS = true;
    Transform playerTransform;

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.CompareTag("Player"))
        {
            Debug.Log("PLAYER");
            playerTransform = collision.transform;
            LOS = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LOS = true;
            slimeController.GatherInput(Vector2.zero); // Stop Moving
        }
    }

    private void FixedUpdate()
    {
        if (LOS == false && playerTransform != null)
        {
            Vector2 direction = (Vector2)(playerTransform.position - slimeController.transform.position);
            slimeController.GatherInput(direction);
        }
    }

}
