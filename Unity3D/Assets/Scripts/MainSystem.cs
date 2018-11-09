using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class MainSystem : ComponentSystem
{
    private const string extensions = "BMP,EXR,GIF,HDR,IFF,JPG,JPEG,PICT,PNG,PSD,TGA,TIFF";

    protected override void OnStartRunning()
    {
        var extensionsSplitted = extensions.Split(',');

        var canvas = Object.FindObjectOfType<Canvas>();
        var transform = canvas.GetComponent<RectTransform>();

        var path = string.Format("{0}/StreamingAssets", Application.dataPath);
        var directories = Directory.GetDirectories(path);
        foreach(var directory in directories)
        {
            var name = Path.GetFileName(directory);


            var imageFile = Directory.GetFiles(directory).Where(p => extensionsSplitted.Contains(Path.GetExtension(p).ToUpper().Replace(".", string.Empty))).FirstOrDefault();
            if(imageFile == null)
            {
                Debug.Log(Path.GetExtension(Directory.GetFiles(directory).FirstOrDefault()));
                continue;
            }
            var bytes = File.ReadAllBytes(imageFile);
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
    protected override void OnUpdate()
    {
        
        Enabled = false;
    }
}
