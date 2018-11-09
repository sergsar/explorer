using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class MainSystem : ComponentSystem
{
    private readonly string[] extensions = "BMP,EXR,GIF,HDR,IFF,JPG,JPEG,PICT,PNG,PSD,TGA,TIFF".Split(',');

    private List<string> imageFileNamesSnapshot;

    private RectTransform transform;

    private float interval;

    private string path = string.Format("{0}/StreamingAssets", Application.dataPath);

    private void Scan()
    {



        var directories = Directory.GetDirectories(path);

        var imageFileNames = new List<string>();
        foreach (var directory in directories)
        {
            var name = Path.GetFileName(directory);


            var imageFile = Directory.GetFiles(directory).Where(p => extensions.Contains(Path.GetExtension(p).ToUpper().Replace(".", string.Empty))).FirstOrDefault();
            if (imageFile == null)
            {
                Debug.Log(Path.GetExtension(Directory.GetFiles(directory).FirstOrDefault()));
                continue;
            }
            imageFileNames.Add(imageFile);
        }

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

        foreach (var imageFileName in imageFileNames)
        {
            var bytes = File.ReadAllBytes(imageFileName);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            var rect = new Rect(0F, 0F, texture.width, texture.height);
            var border = new Vector4(0F, 0F, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, Vector3.zero, 100F, 0, SpriteMeshType.FullRect, border);
            var panel = new GameObject("Cell").AddComponent<RectTransform>();
            panel.SetParent(transform);
            var image = new GameObject("Picture").AddComponent<Image>();
            image.transform.SetParent(panel);
            image.sprite = sprite;
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
