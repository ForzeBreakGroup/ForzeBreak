using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalyticManager : MonoBehaviour
{
    sealed class AnalyticEntry
    {
        private List<string> entry;

        public AnalyticEntry()
        {
            entry = new List<string>();
        }

        public void Insert(string value)
        {
            entry.Add(value);
        }

        public override string ToString()
        {
            return string.Join(",", entry.ToArray());
        }
    }

    private static AnalyticManager analyticManager;
    public static AnalyticManager instance
    {
        get
        {
            if (!analyticManager)
            {
                analyticManager = FindObjectOfType<AnalyticManager>();
                if (!analyticManager)
                {
                    Debug.LogError("AnalyticManager must be attached to a gameobject in scene");
                }
            }

            return analyticManager;
        }
    }

    [SerializeField]
    private string baseLogFileName = "Build";

    private Dictionary<string, AnalyticEntry> entryLogs;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static void Init()
    {
        if (instance.entryLogs == null)
        {
            instance.entryLogs = new Dictionary<string, AnalyticEntry>();
        }
        else
        {
            instance.entryLogs.Clear();
        }

        // Log GamePlay information
        Insert("ArenaName", SceneManager.GetActiveScene().name);
        Insert("GameTime", DateTime.Now.ToString());
        Insert("NumberOfPlayers", PhotonNetwork.playerList.Length);
    }

    public static void Write()
    {
        // Write the structure to text file
        string filename = string.Join("-", new string[] { instance.baseLogFileName, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), PhotonNetwork.gameVersion });
        string path = Application.dataPath +"/" + filename;
        int versionNum = 0;

        // Allocate a filename that does not exists
        while (File.Exists(path + versionNum.ToString()))
        {
            ++versionNum;
        }

        // Format the output string
        string output = instance.ToJson();

        // Write to file
        using (FileStream fs = File.Create(path + "-" + versionNum + ".txt"))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(output);
            fs.Write(info, 0, info.Length);
            fs.Close();
        }
    }

    public static void Clear()
    {
        instance.entryLogs.Clear();
    }

    public static void Insert(string key, Vector3 pos)
    {
        Insert(key, pos.ToString());
    }

    public static void Insert(string key, bool condition)
    {
        Insert(key, (condition) ? "true" : "false");
    }

    public static void Insert(string key, int count)
    {
        Insert(key, count.ToString());
    }

    public static void Insert(string key, string value)
    {
        if (!instance.entryLogs.ContainsKey(key))
        {
            AnalyticEntry entry = new AnalyticEntry();
            instance.entryLogs.Add(key, entry);
        }

        instance.entryLogs[key].Insert(value);
        Debug.Log("Logging Information: " + key + " value: " + value);
    }

    private string ToJson()
    {
        string output = "";
        foreach (KeyValuePair<string, AnalyticEntry> entry in instance.entryLogs)
        {
            string entryToString = "\"" + entry.Key + "\": ";
            output += entryToString + entry.Value.ToString() + ";";
        }

        return output;
    }
}
