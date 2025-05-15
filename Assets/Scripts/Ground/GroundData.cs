using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class GroundData : MonoBehaviour
{
    public enum GroundType { Safe, Level1, Level2 }

    [Header("Type de ce ground")]
    public GroundType Type = GroundType.Safe;

    [Header("Réglages de population (pour Level1 et Level2)")]
    public int minObstacles = 0;
    public int maxObstacles = 2;
    public int minMoney = 0;
    public int maxMoney = 2;

    [Header("Repères générés automatiquement (ne rien toucher)")]
    [SerializeField] private Transform[] spawnPoints = new Transform[9];

    private const int COLS = 3;
    private const int ROWS = 3;

    private void OnValidate()
    {
        if (spawnPoints == null || spawnPoints.Length != COLS * ROWS)
            spawnPoints = new Transform[COLS * ROWS];

        var box = GetComponentInChildren<Collider>() as BoxCollider;
        if (box == null)
        {
            Debug.LogWarning("[GroundData] Aucun BoxCollider trouvé sur le Ground.");
            return;
        }

        Vector3 localSize = Vector3.Scale(box.size, box.transform.localScale);
        float laneWidth = localSize.x / COLS;
        float rowDepth = localSize.z / ROWS;

        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                int idx = r * COLS + c;

                if (spawnPoints[idx] == null)
                {
                    GameObject go = new GameObject($"SpawnPoint_{idx}");
                    go.transform.SetParent(transform, false);
                    spawnPoints[idx] = go.transform;

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        Undo.RegisterCreatedObjectUndo(go, "Create SpawnPoint");
#endif
                }

                float x = (c - 1) * laneWidth;
                float z = localSize.z * 0.5f - rowDepth * (r + 0.5f);
                spawnPoints[idx].localPosition = new Vector3(x, 0f, z);
            }
        }
    }

    public Transform GetSpawnPoint(int index) => (index >= 0 && index < spawnPoints.Length) ? spawnPoints[index] : null;
    public int SpawnPointCount => spawnPoints.Length;
}
