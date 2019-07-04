using DefaultNamespace;
using EnemyControllers;
using UnityEngine;

public class DeathBorder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player":
                other.GetComponent<PlayerCombatController>().FinishDead();
                break;
            case "Enemy":
                other.GetComponent<EnemyController>().MarkAsDead();
                break;
            default:
                Debug.Log("Inside DeathBorder default case");
                break;
        }
    }
}
