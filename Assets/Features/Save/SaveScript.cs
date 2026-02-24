using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveScript : MonoBehaviour
{
    public PlayerPerf _playerPerf;
    public Encyclopedia _encyclopedia;

    private void Start()
    {
         _playerPerf = new PlayerPerf();
        _encyclopedia = new Encyclopedia();
    }
    public void SaveInfo()
    {
        string dossier = Path.Combine(Application.persistentDataPath, "Save");

        if (!Directory.Exists(dossier))
        {
            Directory.CreateDirectory(dossier);
        }

        string path = Path.Combine(dossier, "PlayerPerf.json");
        string path2 = Path.Combine(dossier, "Encyclopedia.json");

        string Json = JsonUtility.ToJson(_playerPerf);
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

            JsonUtility.FromJsonOverwrite(jsonRecupere, _playerPerf);
            JsonUtility.FromJsonOverwrite(jsonRecupere2, _encyclopedia);

            Debug.Log("Données de PlayerPerf et de Encyclopedia chargées !");
        }
        else
        {
            Debug.LogWarning("Le fichier PlayerPerf.json ou le dossier encyclopedia n'existe pas encore.");
        }
    }
}