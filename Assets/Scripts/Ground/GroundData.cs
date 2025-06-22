using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Informations sur un segment de sol (type, position énigme, points de spawn, etc.)
/// </summary>
[ExecuteAlways]
public class GroundData : MonoBehaviour
{
    #region Enums

    public enum GroundType { Safe, Level1, Level2, Level3, Level4, Level5 }

    public enum EnigmeZonePosition { None, Before, Center, After }

    #endregion

    #region Public Parameters

    [Header("Type de ce ground")]
    public GroundType Type = GroundType.Safe;

    [Header("Zone d'enigme")]
    public EnigmeZonePosition EnigmePosition = EnigmeZonePosition.None;

    [Header("Réglages de population (pour Level1 à Level5)")]
    public int minObstacles = 0;
    public int maxObstacles = 2;
    public int minMoney = 0;
    public int maxMoney = 2;

    [Header("Repères générés automatiquement (ne rien toucher)")]
    [SerializeField] private Transform[] spawnPoints = new Transform[9];

    #endregion

    #region Constantes d'édition

    private const float GroundWidthEditor = 20f;
    private const float GroundHeightEditor = 30f;
    private const int COLS = 3;
    private const int ROWS = 3;

    #endregion

    #region Génération des points de spawn (éditeur)

    private void OnValidate()
    {
        if (spawnPoints == null || spawnPoints.Length != COLS * ROWS)
            spawnPoints = new Transform[COLS * ROWS];

        float laneWidth = GroundWidthEditor / 3f;
        float rowDepth = GroundHeightEditor / 3f;

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

                float x = (c - 1) * laneWidth;
                float z = totalDepth * 0.5f - rowDepth * (r + 0.5f);
                spawnPoints[idx].localPosition = new Vector3(x, 0f, z);
            }
        }
    }

    #endregion

    #region Public API

    public Transform GetSpawnPoint(int index) =>
        (index >= 0 && index < spawnPoints.Length) ? spawnPoints[index] : null;

    public int SpawnPointCount => spawnPoints.Length;

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.yellow;
        foreach (var sp in spawnPoints)
        {
            if (sp != null)
                Gizmos.DrawWireSphere(sp.position, 0.3f);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        GUIStyle style = new GUIStyle
        {
            normal = { textColor = Color.white },
            fontSize = 12
        };

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
                Handles.Label(spawnPoints[i].position + Vector3.up * 0.2f, $"SP{i}", style);
        }
    }
#endif

    #endregion
}
