using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using System.IO;
using System.Linq;
using System;

public class YamlConfigReader
{
    private readonly string configPath;
    private readonly object locker = new object();

    public YamlConfigReader(string path)
    {
        configPath = path;
    }

    public string ReadPath()
    {
        string path = null;

        if (!File.Exists(configPath))
        {
            Debug.Log(string.Format("Missing config: {0}", configPath));
            return null;
        }
        var yaml = new YamlStream();
        lock (locker)
        {
            using (var input = new StreamReader(configPath))
            {
                yaml.Load(input);
            }
        }
        YamlMappingNode mappingNode = null;
        try
        {
            mappingNode = (YamlMappingNode)yaml.Documents[0].RootNode;
        }
        catch (System.ArgumentOutOfRangeException ex)
        {
            Debug.Log(ex.Message);
            return null;
        }
        var localPathNode = mappingNode.Children.FirstOrDefault(p => p.Key.ToString() == "localPath");
        if (localPathNode.Key.ToString() != "localPath")
        {
            Debug.Log("Config has no localPath node");
            return null;
        }


        path = Environment.ExpandEnvironmentVariables(localPathNode.Value.ToString()).Replace('/', '\\');
        if (!Directory.Exists(path))
        {
            Debug.Log(string.Format("Missing directory {0}", path));
            return null;
        }
        Debug.Log(path);
        return path;
    }
}
