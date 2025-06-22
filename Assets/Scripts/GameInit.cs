using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private Company companyData;

    private void Awake()
    {
        companyData.LoadFromFile();
        Debug.Log("[CompanyLoader] Chargement forc� de la sauvegarde au d�marrage.");
    }
    private void Start()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            SetFrameSettings();
        }
#else
    SetFrameSettings();
#endif
    }

    private void SetFrameSettings()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }



   
}

