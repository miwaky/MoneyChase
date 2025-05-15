using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GroundData))]
public class GroundPopulator : MonoBehaviour
{
    private GroundData data;
    private bool alreadyPopulated = false;

    private void Awake()
    {
        data = GetComponent<GroundData>();
    }

    private void Start()
    {
        if (alreadyPopulated) return;
        alreadyPopulated = true;

        if (data == null) return;
        if (data.Type == GroundData.GroundType.Safe) return;

        LevelManager lm = LevelManager.Instance;
        if (lm == null)
        {
            Debug.LogError("[GroundPopulator] LevelManager.Instance est null !");
            return;
        }

        int obstaclesToSpawn = Random.Range(data.minObstacles, data.maxObstacles + 1);
        int moneyToSpawn = Random.Range(data.minMoney, data.maxMoney + 1);
        int totalNeeded = obstaclesToSpawn + moneyToSpawn;

        if (data.SpawnPointCount < totalNeeded)
        {
            Debug.LogWarning($"[GroundPopulator] Pas assez de spawn points (dispo: {data.SpawnPointCount}, requis: {totalNeeded}) sur {gameObject.name}");
            return;
        }

        List<int> indices = new List<int>();
        for (int i = 0; i < data.SpawnPointCount; i++) indices.Add(i);
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        int idx = 0;

        // Obstacles
        for (int i = 0; i < obstaclesToSpawn; i++, idx++)
        {
            Transform sp = data.GetSpawnPoint(indices[idx]);
            if (sp == null) continue;

            GameObject prefab = lm.GetObstaclePrefab();
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                go.transform.position = sp.position + Vector3.up * 1f;
                lm.ApplyDynamicScale(go);
            }
        }

        // Money
        for (int i = 0; i < moneyToSpawn; i++, idx++)
        {
            Transform sp = data.GetSpawnPoint(indices[idx]);
            if (sp == null) continue;

            GameObject prefab = lm.GetMoneyPrefab();
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                go.transform.position = sp.position + Vector3.up * 2f;
                lm.ApplyDynamicScale(go);
            }
        }

        Debug.Log($"[GroundPopulator]  {obstaclesToSpawn} obstacle(s) et {moneyToSpawn} argent(s) plac�s sur {gameObject.name}");
    }
}
