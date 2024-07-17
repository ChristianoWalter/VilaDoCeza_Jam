using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class SaveManager
{
    // Método para salvamento de dados
    public static void SaveGame(SaveFiles saveFiles)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savegame.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, saveFiles);
        stream.Close();
    }

    // Método para carregamento de dados
    public static SaveFiles LoadFiles()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savegame.data";

        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveFiles returnFiles = bf.Deserialize(stream) as SaveFiles;
            stream.Close();
            return returnFiles;
        }
        else return null;
    }

}
