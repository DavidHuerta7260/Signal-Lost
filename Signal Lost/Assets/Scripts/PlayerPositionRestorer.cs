using UnityEngine;

public class PlayerPositionRestorer : MonoBehaviour
{
    void Start()
    {
        if (GameManager.hasSavedPosition)
        {
            transform.position = GameManager.playerPosition;
            Debug.Log("Player restored to: " + GameManager.playerPosition);

            // Trigger room feedback for completed terminal
            if (GameManager.Instance != null)
            {
                string lastID = GameManager.Instance.lastTerminalID;
                RoomFeedback[] allRooms = FindObjectsOfType<RoomFeedback>();
                foreach (RoomFeedback room in allRooms)
                {
                    if (room.requiredTerminalID == lastID)
                        room.ActivateFeedback();
                }
            }
        }
    }
}
