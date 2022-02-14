using UnityEngine;

public class CharacterMovement : MonoBehaviour, ICharacterMovement
{
    public void HandleMovement()
    {
        Debug.Log("Handling movement!");
    }
}