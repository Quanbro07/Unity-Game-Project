using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackArea = null;
    [SerializeField] private float timeToAttack; // 0.4
    
    [SerializeField] private Animator animator;
    private int playerAttack = Animator.StringToHash("PLayAttack_1");

    public bool IsAttack => isAttack;
    private bool isAttack;


    // Update is called once per frame
    private void Awake()
    {
        isAttack = false;
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        HandleAttack();
    }
    void HandleAttack()
    {
        if (Input.GetMouseButton(0) && isAttack == false) 
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        Debug.Log($"Attack started - Frame: {Time.frameCount}, Time: {Time.time}");

        isAttack = true;
        attackArea.SetActive(true);
        updateAttackAnimation();

        Debug.Log($"Starting wait for {timeToAttack} seconds at time: {Time.time}");

        // Break the wait into smaller chunks to see where it stops
        float waitTime = 0f;
        while (waitTime < timeToAttack)
        {
            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
            Debug.Log($"Waited {waitTime} seconds so far...");
        }

        Debug.Log($"Wait finished - Frame: {Time.frameCount}, Time: {Time.time}");
        isAttack = false;
        attackArea.SetActive(false);
        Debug.Log($"Attack ended - Frame: {Time.frameCount}, Time: {Time.time}");

    }

    private void updateAttackAnimation()
    {
        if(isAttack)
        {
            Debug.Log($"Setting attack animation: {playerAttack}");
            animator.CrossFade(playerAttack, 0, 0);
        }
    }
}
