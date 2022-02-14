using UnityEngine;

public class AttackHandler : MonoBehaviour, IAttackHandler
{
    public void Attack()
    {
        Debug.Log("Attack");
    }
}