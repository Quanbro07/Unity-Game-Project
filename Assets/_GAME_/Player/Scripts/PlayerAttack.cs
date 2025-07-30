using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackArea = null;
    [SerializeField] private float timeToAttack = 0.4f;

    [SerializeField] private Animator playerAnimator;
    private int playerAttack_1 = Animator.StringToHash("PlayerAttack_1");
    private int playerAttack_2 = Animator.StringToHash("PlayerAttack_2");
    private int playerAttack_3 = Animator.StringToHash("PlayerAttack_3");

    private float maxComboTime = 1f;
    private int comboStep = 0;
    private float comboTimer = 0f;

    private bool isAttack = false;
    private bool isComboTiming = false;

    private Coroutine attackCoroutine;

    public bool IsAttack => isAttack;
    public int currentCombo => comboStep;

    private void Update()
    {
        HandleComboTimer();

        if (Input.GetMouseButtonDown(0) && !isAttack)
        {
            attackCoroutine = StartCoroutine(Attack());
        }
    }

    private void HandleComboTimer()
    {
        if (isComboTiming)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    private IEnumerator Attack()
    {
        isAttack = true;

        comboStep++;
        if (comboStep > 3) comboStep = 1;

        UpdateAttackAnimation(comboStep);

        comboTimer = maxComboTime;
        isComboTiming = true;

        attackArea.SetActive(true);

        yield return new WaitForSeconds(timeToAttack);

        isAttack = false;
        attackArea.SetActive(false);
    }

    private void UpdateAttackAnimation(int step)
    {
        switch (step)
        {
            case 1:
                playerAnimator.CrossFade(playerAttack_1, 0.1f, 0);
                break;
            case 2:
                playerAnimator.CrossFade(playerAttack_2, 0.1f, 0);
                break;
            case 3:
                playerAnimator.CrossFade(playerAttack_3, 0.1f, 0);
                break;
            default:
                Debug.LogWarning("Invalid combo step: " + step);
                break;
        }
    }

    private void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
        isComboTiming = false;
    }

    public void DisableAttackTemporarily(float duration)
    {
        StartCoroutine(DisableAttackCoroutine(duration));
    }

    private IEnumerator DisableAttackCoroutine(float duration)
    {
        isAttack = true;
        yield return new WaitForSeconds(duration);
        isAttack = false;
    }

    public void ForceCancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttack = false;
        attackArea.SetActive(false);
        ResetCombo();
    }
}
