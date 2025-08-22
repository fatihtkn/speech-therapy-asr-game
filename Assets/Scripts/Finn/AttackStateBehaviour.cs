using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    // StateMachineBehaviour tetiklendiðinde, animasyonun baþladýðý state için çaðrýlýr
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Animator'ýn baðlý olduðu GameObject'teki SwordComboController'ý al
        var combo = animator.GetComponent<SwordComboController>();
        if (combo != null)
        {
            combo.OnAttackStateEnter();
        }
    }

    // Animasyon state’inden çýkarken çaðrýlýr
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var combo = animator.GetComponent<SwordComboController>();
        if (combo != null)
        {
            combo.OnAttackStateExit();
        }
    }
}
