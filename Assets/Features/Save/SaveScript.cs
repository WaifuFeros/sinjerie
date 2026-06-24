using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveScript : MonoBehaviour
{
    public static bool HasLoadedSave { get; private set; }

    public PlayerSave _playerSave;
    public EncyclopediaPanel _encyclopedia;

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            SaveInfo();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveInfo();
        }
    }

    private void OnApplicationQuit()
    {
        SaveInfo();
    }

    public void SaveInfo()
    {
        _playerSave.maxBanana = MetaProgressionManager.Instance.TotalBananaCount;
        _playerSave.currentBanana = MetaProgressionManager.Instance.CurrentBananaCount;
        _playerSave.statUpgrades = MetaProgressionManager.Instance.GetStatUpgradeSave();

        string dossier = Path.Combine(Application.persistentDataPath, "Save");

        if (!Directory.Exists(dossier))
        {
            Directory.CreateDirectory(dossier);
        }

        string path = Path.Combine(dossier, "PlayerPerf.json");
        string path2 = Path.Combine(dossier, "Encyclopedia.json");

        string Json = JsonUtility.ToJson(_playerSave);
        string Json2 = JsonUtility.ToJson(_encyclopedia);

        File.WriteAllText(path, Json);
        File.WriteAllText(path2, Json2);

        Debug.Log("Sauvegarde réussie dans : " + dossier);
    }

    public void LoadInfo()
    {
        string dossier = Path.Combine(Application.persistentDataPath, "Save");
        string path = Path.Combine(dossier, "PlayerPerf.json");
        string path2 = Path.Combine(dossier, "Encyclopedia.json");

        if (File.Exists(path))
        {
            string jsonRecupere = File.ReadAllText(path);
            string jsonRecupere2 = File.ReadAllText(path2);

            JsonUtility.FromJsonOverwrite(jsonRecupere, _playerSave);
            JsonUtility.FromJsonOverwrite(jsonRecupere2, _encyclopedia);

            Debug.Log("Données de PlayerPerf et de Encyclopedia chargées !");

            MetaProgressionManager.Instance.SetTotalBanana(_playerSave.maxBanana);
            MetaProgressionManager.Instance.CurrentBananaCount = _playerSave.currentBanana;
            MetaProgressionManager.Instance.LoadUpgradeLevels(_playerSave.statUpgrades);

            HasLoadedSave = true;
        }
        else
        {
            Debug.LogWarning("Le fichier PlayerPerf.json ou le dossier encyclopedia n'existe pas encore.");
            return;
        }
    }
}