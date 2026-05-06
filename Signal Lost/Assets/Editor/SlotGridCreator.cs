using UnityEngine;
using UnityEditor;

public class SlotGridCreator : EditorWindow
{
    int columns = 5;
    int rows = 5;
    float spacingX = 1.07f;
    float spacingY = 1.07f;
    Vector2 startPosition = Vector2.zero;
    GameObject slotPrefab;

    [MenuItem("Tools/Slot Grid Creator")]
    public static void ShowWindow()
    {
        GetWindow<SlotGridCreator>("Slot Grid Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Slot Grid Settings", EditorStyles.boldLabel);

        columns = EditorGUILayout.IntField("Columns", columns);
        rows = EditorGUILayout.IntField("Rows", rows);
        spacingX = EditorGUILayout.FloatField("Spacing X", spacingX);
        spacingY = EditorGUILayout.FloatField("Spacing Y", spacingY);
        startPosition = EditorGUILayout.Vector2Field("Start Position", startPosition);
        slotPrefab = (GameObject)EditorGUILayout.ObjectField("Slot Prefab", slotPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Create Grid"))
        {
            CreateGrid();
        }
    }

    void CreateGrid()
    {
        // Create parent
        GameObject parent = new GameObject("PuzzleGrid");
        parent.transform.position = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(parent, "Create Slot Grid");

        int count = 1;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject slot;

                if (slotPrefab != null)
                    slot = (GameObject)PrefabUtility.InstantiatePrefab(slotPrefab);
                else
                {
                    slot = new GameObject("Slot " + count);
                    slot.tag = "Slot";
                    slot.layer = LayerMask.NameToLayer("Slot");

                    // Add required components
                    slot.AddComponent<PuzzlePiece>();
                    slot.AddComponent<PipePiece>();
                    BoxCollider2D col2d = slot.AddComponent<BoxCollider2D>();
                    col2d.isTrigger = true;
                    col2d.size = new Vector2(1f, 1f);
                }

                slot.name = "Slot " + count;
                slot.transform.SetParent(parent.transform);

                float x = startPosition.x + col * spacingX;
                float y = startPosition.y - row * spacingY;
                slot.transform.position = new Vector3(x, y, 0);

                count++;
            }
        }

        Debug.Log("Created " + (rows * columns) + " slots");
    }
}