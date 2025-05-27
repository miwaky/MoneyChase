using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GroundData : MonoBehaviour
{
    public enum GroundType { Safe, Level1, Level2 }
    public enum EnigmeZonePosition { None, Before, Center, After }

    [Header("Type de ce ground")]
    public GroundType Type = GroundType.Safe;

    [Header("Zone d'enigme")]
    public EnigmeZonePosition EnigmePosition = EnigmeZonePosition.None;

    [Header("Reglages de population (pour Level1 et Level2)")]
    public int minObstacles = 0;
    public int maxObstacles = 2;
    public int minMoney = 0;
    public int maxMoney = 2;

    [Header("Reperes generes automatiquement (ne rien toucher)")]
    [SerializeField] private Transform[] spawnPoints = new Transform[9];

    private const float groundWidthEditor = 20f;
    private const float groundHeightEditor = 30f;
    
    private const int COLS = 3;
    private const int ROWS = 3;

    private void OnValidate()
    {
        if (spawnPoints == null || spawnPoints.Length != COLS * ROWS)
            spawnPoints = new Transform[COLS * ROWS];


        float laneWidth = groundWidthEditor / 3f;
        float rowDepth = groundHeightEditor / 3f;

        float totalWidth = laneWidth * COLS;
        float totalDepth = rowDepth * ROWS;

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

                float x = (c - 1) * laneWidth; // -1, 0, 1 => -3, 0, +3
                float z = totalDepth * 0.5f - rowDepth * (r + 0.5f); // 4.5, 1.5, -1.5
                spawnPoints[idx].localPosition = new Vector3(x, 0f, z);
            }
        }
    }

    public Transform GetSpawnPoint(int index) => (index >= 0 && index < spawnPoints.Length) ? spawnPoints[index] : null;
    public int SpawnPointCount => spawnPoints.Length;

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.yellow;

        foreach (var sp in spawnPoints)
        {
            if (sp != null)
            {
                Gizmos.DrawWireSphere(sp.position, 0.3f);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
            {
                Handles.Label(spawnPoints[i].position + Vector3.up * 0.2f, $"SP{i}", style);
            }
        }
    }
#endif
}