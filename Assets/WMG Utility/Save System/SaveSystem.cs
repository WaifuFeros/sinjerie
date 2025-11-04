using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace WMG.Save
{
    public static class SaveSystem
    {
        public static void Save(string path, string fileName, string extention, object saveData)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileStream stream = new FileStream(path + fileName + extention, FileMode.Create);

            binaryFormatter.Serialize(stream, saveData);
            stream.Close();
        }

        public static object Load(string path)
        {
            if (!File.Exists(path))
                return null;

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                var save = binaryFormatter.Deserialize(file);
                file.Close();
                return save;
            }
            catch
            {
                Debug.LogError($"Failed To load file at {path}");
                file.Close();
                return null;
            }
        }

        public static T Load<T>(string path) where T : SaveFile
        {
            if (!File.Exists(path))
                return null;

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                var save = binaryFormatter.Deserialize(file);
                file.Close();
                return save as T;
            }
            catch
            {
                Debug.LogError($"Failed To load file at {path}");
                file.Close();
                return null;
            }
        }

        public static string[] GetAllFiles(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Directory.GetFiles(path);
        }

        public static string[] GetAllFilesSortedByDate(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string[] fileNames = Directory.GetFiles(path);
            DateTime[] creationTimes = new DateTime[fileNames.Length];
            for (int i = 0; i < fileNames.Length; i++)
                creationTimes[i] = new FileInfo(fileNames[i]).CreationTime;
            Array.Sort(creationTimes, fileNames);
            Array.Reverse(fileNames);

            return fileNames;
        }

        public static void DeleteFile(string path)
        {
            if (!File.Exists(path))
                return;

            File.Delete(path);
        }
    }

}