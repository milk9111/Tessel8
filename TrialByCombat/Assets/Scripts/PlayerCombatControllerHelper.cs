using DefaultNamespace;
using UnityEngine;

public class PlayerCombatControllerHelper : MonoBehaviour
{

    private PlayerCombatController _combatController;

    void Awake()
    {
        _combatController = GetComponentInParent<PlayerCombatController>();
    }

    public void AttackEnemy()
    {
        _combatController.AttackEnemy();
    }

    public void FinishAttack()
    {
        _combatController.FinishAttack();
    }
}
