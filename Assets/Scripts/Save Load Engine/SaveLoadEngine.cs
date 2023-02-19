using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.Burst;
using UnityEngine;

// Can a stream be left open? Continous data streams?
[BurstCompile]
public class SaveLoadEngine : MonoBehaviour
{
    public Main main = null;

    //public SaveQueue saveQueue = null; ?
    //public LoadQueue loadQueue = null; ?

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory
        main = null;
    }

    [BurstCompile]
    public void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();

        Debug.Log("Data Path: " + Application.persistentDataPath);
    }

    //---------------------------------------------------------------

    [BurstCompile]
    public string CompressString(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }

    [BurstCompile]
    public string DecompressString(string compressedText)
    {
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        using (var memoryStream = new MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

            var buffer = new byte[dataLength];

            memoryStream.Position = 0;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }

    //---------------------------------------------------------------

    [BurstCompile]
    public string ToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    [BurstCompile]
    public T FromJson<T>(string obj)
    {
        return JsonUtility.FromJson<T>(obj);
    }

    //---------------------------------------------------------------

    [BurstCompile]
    public void SaveData(string data, string filepath)
    {
        string destination = Application.persistentDataPath + "/" + filepath + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(file, data);
        file.Close();
    }

    [BurstCompile]
    public string LoadData(string filepath)
    {
        string destination = Application.persistentDataPath + "/" + filepath + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string data = (string)binaryFormatter.Deserialize(file);
        file.Close();

        return data;
    }

    //---------------------------------------------------------------

    [BurstCompile]
    public FileStream CreateFile(string filepath)
    {
        string destination = Application.persistentDataPath + "/" + filepath + ".dat";

        if (!File.Exists(destination)) return File.Create(destination);
        else return null;
    }

    [BurstCompile]
    public void DeleteFile(string filepath)
    {
        string destination = Application.persistentDataPath + "/" + filepath + ".dat";

        if (File.Exists(destination)) File.Delete(destination);
        else
        {
            Debug.LogError("File not found");
        }
    }

    [BurstCompile]
    public void CreateFolder(string filepath)
    {
        string destination = Application.persistentDataPath + filepath;

        DirectoryInfo dictionaryInfo;
        if (!Directory.Exists(destination)) dictionaryInfo = Directory.CreateDirectory(destination);
        else
        {
            Debug.LogError("Folder already exists");
        }
    }

    [BurstCompile]
    public void DeleteFolder(string filepath)
    {
        string destination = Application.persistentDataPath + filepath;

        if (Directory.Exists(destination)) Directory.Delete(filepath);
    }
}
