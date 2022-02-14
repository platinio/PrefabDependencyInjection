using Platinio.DependencyInjection;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Dependencies
    [SerializeField] [MonoTypeRestriction(typeof(IAttackHandler))] [InjectableInterfaceDependency(typeof(IAttackHandler))]
    private MonoInterface<IAttackHandler> m_attackHandler;

    [SerializeField] [MonoTypeRestriction(typeof(ICharacterInput))] [InjectableInterfaceDependency(typeof(ICharacterInput))]
    private MonoInterface<ICharacterInput> m_characterInput;
    
    [SerializeField] [MonoTypeRestriction(typeof(ICharacterMovement))] [InjectableInterfaceDependency(typeof(ICharacterMovement))]
    private MonoInterface<ICharacterMovement> m_characterMovement;
    
    [SerializeField] [MonoTypeRestriction(typeof(IInventory))] [InjectableInterfaceDependency(typeof(IInventory))] 
    private MonoInterface<IInventory> m_inventory;

    [SerializeField] [InjectableDependency] private Animator m_animator;
    #endregion
}
