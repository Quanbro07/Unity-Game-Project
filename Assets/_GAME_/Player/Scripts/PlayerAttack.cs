using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject attackArea = null;

    private bool isAttack = false;
    [SerializeField] private float timeToAttack = 1f;

    private void Awake()
    {
        attackArea = transform.Find("AttackArea").gameObject;
    }

    // Update is called once per frame
    void Update()
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
        attackArea.SetActive(isAttack);
        yield return new WaitForSeconds(timeToAttack);
        
        isAttack = false;
        attackArea.SetActive(isAttack);

    }
}
