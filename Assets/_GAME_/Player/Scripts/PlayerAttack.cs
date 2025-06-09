using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackArea = null;
    [SerializeField] private float timeToAttack; // 0.4
    
    [SerializeField] private Animator playerAnimator;

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

        isAttack = true;
        attackArea.SetActive(true);
        updateAttackAnimation();


        yield return new WaitForSeconds(timeToAttack);

        isAttack = false;
        attackArea.SetActive(false);

    }

    private void updateAttackAnimation()
    {
        if(isAttack)
        {
            playerAnimator.CrossFade(playerAttack, 0, 0);
        }
    }
}
