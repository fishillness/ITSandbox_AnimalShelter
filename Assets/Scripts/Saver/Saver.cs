using System.IO;
using System;
using UnityEngine;


[Serializable]
public class Saver<T>
{
    public T data;

        
    public static bool TryLoad(string filename, ref T data)
    {
        string path = FileHandler.Path(filename);
        //Debug.Log(path);
        if(File.Exists(path))
        {
            string encryptedDataString = File.ReadAllText(path);
            (string key, string ivSecret) = ConfigurationManager.LoadEncryptionConfig();
            string decryptedDataString = EncryptionManager.DecryptText(encryptedDataString, key, ivSecret);

            Saver<T> saver = JsonUtility.FromJson<Saver<T>>(decryptedDataString);
            data = saver.data;
            return true;
        }
        else
        {
            return false;
        }
            
    }
    public static void Save(string filename, T data)
    {
        Saver<T> wrapper = new Saver<T>();
        wrapper.data = data;
        string dataString = JsonUtility.ToJson(wrapper);

        (string key, string ivSecret) = ConfigurationManager.LoadEncryptionConfig();
        string encryptedDataString = EncryptionManager.EncryptText(dataString, key, ivSecret);

        File.WriteAllText(FileHandler.Path(filename), encryptedDataString);            
    }

        
}

public static class FileHandler
{
    public static string Path(string filename)
    {
        //Debug.Log(Application.persistentDataPath + "/"); C:/Users/aser9/AppData/LocalLow/DefaultCompany/ItSandboxNika/
        return Application.persistentDataPath + "/" + filename;
    }
    public static void Reset(string filename)
    {
        string path = Path(filename);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool HasFile(string filename)
    {
        return File.Exists(Path(filename));
    }
}
