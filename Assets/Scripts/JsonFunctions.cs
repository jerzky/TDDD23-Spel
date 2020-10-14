﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public static class Json
{
    [Serializable]
    public struct SerializeableContainer<T>
    {
        public T container;
    }

    public struct mytest
    {
        int i;
        int k;
    }


    public static T JsonToContainer<T>(string path)
    {
        var pathc = Path.Combine(Application.streamingAssetsPath, path);
        var fileContent = File.ReadAllText(pathc);
        Debug.Log(fileContent);
        return JsonConvert.DeserializeObject<T>(fileContent);
    }

    public static void SaveToJson<T>(T container, string path)
    {
        var setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        var json = JsonConvert.SerializeObject(container, setting);
        var pathc = Path.Combine(Application.streamingAssetsPath, path);
        File.WriteAllText(pathc, json);
    }
}
