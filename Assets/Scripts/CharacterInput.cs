using UnityEngine;

public class CharacterInput : MonoBehaviour, ICharacterInput
{
    public void HandleInput()
    {
        Debug.Log("Handling Input!");
    }
}