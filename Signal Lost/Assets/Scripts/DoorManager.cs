using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Door[] allDoors;

    void OnEnable()
    {
        RefreshAllDoors();
    }

    public void RefreshAllDoors()
    {
        foreach (Door door in allDoors)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.IsTerminalComplete(door.requiredTerminalID))
            {
                door.gameObject.SetActive(false);
            }
        }
    }
}