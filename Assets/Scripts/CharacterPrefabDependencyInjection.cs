using Platinio.DependencyInjection;
using UnityEngine;

public class CharacterPrefabDependencyInjection : PrefabDependencyInjection
{
    [Header("Dependencies")]
    [SerializeField] [DependencyInterface(typeof(IAttackHandler))] private AttackHandler m_attackHandler = null;
    [SerializeField] [DependencyInterface(typeof(ICharacterInput))] private CharacterInput m_characterInput = null;
    [SerializeField] [DependencyInterface(typeof(ICharacterMovement))] private CharacterMovement m_characterMovement = null;
    [SerializeField] [DependencyInterface(typeof(IInventory))] private Inventory m_inventory = null;
    [SerializeField] [Dependency] private Animator m_animator = null;
}
