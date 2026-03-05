using System.IO;
using System;
using UnityEngine;

public static class ConfigurationManager
{

    private static string SecretsConfigFile = Application.dataPath + "/Resources/appsettings.secrets.json";

    public static (string key, string iv) LoadEncryptionConfig()
    {
        TextAsset json = Resources.Load<TextAsset>("appsettings.secrets"); 
        if (json == null) 
            throw new FileNotFoundException("appsettings.secrets.json not found in Resources"); 

        EncryptionSettings settings = JsonUtility.FromJson<EncryptionSettings>(json.text); 
        return (settings.Key, settings.IV);
    }

    public static void CreateSecretsConfigTemplate()
    {
        EncryptionSettings template = new EncryptionSettings
        {
            Key = "your_secrets_key",
            IV = "your_initialization_vector"
        };
        
        string json = JsonUtility.ToJson(template, true);
        string path = Path.Combine(Application.streamingAssetsPath, SecretsConfigFile);
        string directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, json);

        Console.WriteLine($"Template file created: {SecretsConfigFile}");
        Console.WriteLine("Replace the values ​​with your secret data!");
    }

    private static EncryptionSettings LoadFromFile(string filename)
    {
        try
        {
            string json = File.ReadAllText(filename);
            EncryptionSettings settings = JsonUtility.FromJson<EncryptionSettings>(json);

            if (settings == null)
            {
                Debug.LogError($"File {filename} is empty or contains invalid JSON");
                return null;
            }

            if (string.IsNullOrEmpty(settings.Key) ||
                string.IsNullOrEmpty(settings.IV))
            {
                Debug.LogError($"The file {filename} is missing encryption keys");
                return null;
            }

            return settings;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error accessing file {filename}: {ex.Message}");
            return null;
        }
    }
}
