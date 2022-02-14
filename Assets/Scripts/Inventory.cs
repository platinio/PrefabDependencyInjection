using UnityEngine;

public class Inventory : MonoBehaviour, IInventory
{
    public void OpenInventory()
    {
        Debug.Log("Showing inventory");
    }
}