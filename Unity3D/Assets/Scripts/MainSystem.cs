using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class MainSystem : ComponentSystem
{
    

    private List<string> imageFileNamesSnapshot;

    private RectTransform transform;

    private float interval;

    private string path = string.Format("{0}/StreamingAssets", Application.dataPath);
    private string configPath = string.Format("{0}/StreamingAssets/config.yaml", Application.dataPath);


    private void Scan()
    {
        var configReader = new YamlConfigReader(configPath);
        var localPath = configReader.ReadPath() ?? path;


        var directoryScanner = new DirectoryScanner(localPath);
        var explorerItems = directoryScanner.Scan();

        var imageFileNames = explorerItems.Select(p => p.ImageFileName).ToList();
        if (imageFileNamesSnapshot != null && imageFileNames.SequenceEqual(imageFileNamesSnapshot))
        {
            return;
        }
        imageFileNamesSnapshot = imageFileNames;
        Debug.Log("changed");

        var objects = transform.GetComponentsInChildren<Transform>().Where(p => p != transform);
        foreach (var obj in objects)
        {
            Object.Destroy(obj.gameObject);
        }

        foreach (var explorerItem in explorerItems)
        {
            var bytes = File.ReadAllBytes(explorerItem.ImageFileName);
            var texture = new Texture2D(2, 2);
            if(!texture.LoadImage(bytes))
            {
                Debug.LogErrorFormat("failed to load image file {0}", explorerItem.ImageFileName);
                continue;
            }
            var rect = new Rect(0F, 0F, texture.width, texture.height);
            var border = new Vector4(0F, 0F, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, Vector3.zero, 100F, 0, SpriteMeshType.FullRect, border);
            var panel = new GameObject("Cell").AddComponent<RectTransform>();
            panel.SetParent(transform);
            var image = new GameObject("Picture").AddComponent<Image>();
            image.transform.SetParent(panel);
            image.sprite = sprite;
            image.preserveAspect = true;

            var text = new GameObject("Name").AddComponent<Text>();
            text.transform.SetParent(panel);
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 22;
            text.color = Color.black;
            text.text = explorerItem.Name;
            text.alignment = TextAnchor.UpperCenter;
            var textRectTransform = text.GetComponent<RectTransform>();
            textRectTransform.anchorMin = new Vector2(0.1F, 0.1F);
            textRectTransform.anchorMax = new Vector2(0.9F, 0.9F);
            textRectTransform.anchoredPosition = Vector2.zero;
            textRectTransform.sizeDelta = Vector2.zero;
        }
    }

    protected override void OnStartRunning()
    {

        var canvas = Object.FindObjectOfType<Canvas>();
        transform = canvas.GetComponent<RectTransform>();


    }
    protected override void OnUpdate()
    {

        interval += Time.deltaTime;
        if(interval > 1F)
        {
            interval = 0F;
            Scan();
        }
    }
}
