using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GroundData))]
public class PauseRoomPopulator : MonoBehaviour
{
    [SerializeField] private GameObject vendingMachinePrefab;
    [SerializeField] private int minMachines = 1;
    [SerializeField] private int maxMachines = 3;

    // Indices valides des spawnPoints
    private static readonly int[] AllowedIndices = { 0, 2, 3, 5, 6, 8 };

    private void Start()
    {
        GroundData data = GetComponent<GroundData>();
        if (data == null || vendingMachinePrefab == null) return;

        int machineCount = Random.Range(minMachines, maxMachines + 1);
        List<int> candidates = new List<int>(AllowedIndices);
        Shuffle(candidates);

        for (int i = 0; i < machineCount && i < candidates.Count; i++)
        {
            int index = candidates[i];
            Transform sp = data.GetSpawnPoint(index);
            if (sp != null)
            {
                GameObject vending = Instantiate(vendingMachinePrefab, sp.position, Quaternion.identity);
                vending.transform.SetParent(transform); // Hiérarchiquement lié au sol
            }
        }
    }

    // Utilitaire simple pour mélanger les indices
    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}