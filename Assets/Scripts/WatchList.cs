using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DS;
using Newtonsoft.Json;
using UnityEngine;

public class WatchList : MonoBehaviour
{

    private string filePath;
    private DataStructure.WatchListData _watchList;

    private void Start()
    {
        filePath =  Path.Combine(Application.persistentDataPath, "watchlist.json");
        if (!File.Exists(filePath))
        {
            // Create new WatchListData with default values
            var watchListData = new DataStructure.WatchListData();
            watchListData.movieIdList = new List<string>();
            // Serialize default WatchListData to JSON
            string json = JsonConvert.SerializeObject(watchListData);
            // Write JSON data to file
            File.WriteAllText(filePath, json);
        }
        Debug.Log(Application.persistentDataPath);
    }

    public void SaveWatchListData(string movieId)
    {
        string jsonData = File.ReadAllText(filePath);
        _watchList = JsonConvert.DeserializeObject<DataStructure.WatchListData>(jsonData);
        Debug.Log("movies count"+ _watchList.movieIdList.Count);
        if (!_watchList.movieIdList.Contains(movieId))
        {
            _watchList.movieIdList.Add(movieId);
            string upDatedJsonData = JsonConvert.SerializeObject(_watchList);
            File.WriteAllText(filePath, upDatedJsonData);
        }
       
        
    }

    public DataStructure.WatchListData LoadWatchListData()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            
            return JsonConvert.DeserializeObject<DataStructure.WatchListData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Watchlist data file does not exist.");
            return new DataStructure.WatchListData();
        }
    }
}
