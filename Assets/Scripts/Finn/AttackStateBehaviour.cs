using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    // StateMachineBehaviour tetiklendi�inde, animasyonun ba�lad��� state i�in �a�r�l�r
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Animator'�n ba�l� oldu�u GameObject'teki SwordComboController'� al
        var combo = animator.GetComponent<SwordComboController>();
        if (combo != null)
        {
            combo.OnAttackStateEnter();
        }
    }

    // Animasyon state�inden ��karken �a�r�l�r
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var combo = animator.GetComponent<SwordComboController>();
        if (combo != null)
        {
            combo.OnAttackStateExit();
        }
    }
}
