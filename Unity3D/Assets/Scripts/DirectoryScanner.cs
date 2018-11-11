using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class DirectoryScanner
{
    private readonly string[] extensions = "BMP,EXR,GIF,HDR,IFF,JPG,JPEG,PICT,PNG,PSD,TGA,TIFF".Split(',');
    private readonly string path;
    public DirectoryScanner(string path)
    {
        this.path = path;
    }

    public List<ExplorerItem> Scan()
    {
        var directories = Directory.GetDirectories(path);

        var explorerItems = new List<ExplorerItem>();

        foreach (var directory in directories)
        {
            var name = Path.GetFileName(directory);

            string imageFileName = null;
            try
            {
                imageFileName = Directory.GetFiles(directory).Where(p => extensions.Contains(Path.GetExtension(p).ToUpper().Replace(".", string.Empty))).FirstOrDefault();
            }
            catch (System.UnauthorizedAccessException ex)
            {
                Debug.Log(ex.Message);
                continue;
            }
            if (imageFileName == null)
            {
                Debug.Log(string.Format("no images in directory {0}", directory));
                continue;
            }
            var item = new ExplorerItem() { Name = name, ImageFileName = imageFileName };
            explorerItems.Add(item);

        }
        return explorerItems;
    }
}
