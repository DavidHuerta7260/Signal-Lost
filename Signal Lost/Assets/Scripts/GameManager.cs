using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public int totalTerminals = 5;

    public static Vector3 playerPosition;
    public static bool hasSavedPosition = false;
    public static bool puzzleSolved = false;

    // Tracks which terminal was last used
    public string lastTerminalID = "";

    private Dictionary<string, bool> completedTerminals = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteTerminal(string terminalID)
    {
        if (!completedTerminals.ContainsKey(terminalID))
        {
            completedTerminals[terminalID] = true;
            puzzleSolved = true;
            Debug.Log("Terminal completed: " + terminalID);
        }
    }

    public bool IsTerminalComplete(string terminalID)
    {
        return completedTerminals.ContainsKey(terminalID) && completedTerminals[terminalID];
    }

    public int GetCompletedCount()
    {
        return completedTerminals.Count;
    }

    public bool AllTerminalsComplete()
    {
        return completedTerminals.Count >= totalTerminals;
    }

    public void SavePlayerPosition(Vector3 position)
    {
        playerPosition = position;
        hasSavedPosition = true;
    }
}