using Pathfinding;
using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    [SerializeField] private AIPath aiPath;
    [SerializeField] private AIDestinationSetter destinationSetter;
    
    private Transform _cachedTarget;
    Vector3 lastPos;

    private bool LOS;
    private void Start()
    {
        LOS = true;
        aiPath.enabled = false;
        destinationSetter.enabled = false;
    }

    private void FixedUpdate()
    {
        DisableChasing();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _cachedTarget = collision.transform;
            destinationSetter.target = _cachedTarget;

            LOS = false;
            aiPath.enabled = true;
            destinationSetter.enabled = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            lastPos = collision.transform.position;
            _cachedTarget.position = lastPos;

            LOS = true;
            destinationSetter.target = _cachedTarget;

            destinationSetter.enabled= false;
            
        }
    }

    private void DisableChasing()
    {
        float distance = Vector3.Distance(transform.position, lastPos);

        if (LOS == true && distance < 0.5f)
        {
            aiPath.enabled = false;
        }

    }

}
