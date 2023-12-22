using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MenuBuilder : MonoBehaviour
{
    public void CreateIcon(string imageAddress, string name, GameObject itemBaseIcon)
    {
        GameObject item = Instantiate(itemBaseIcon, itemBaseIcon.transform);
        item.SetActive(true);
        item.transform.GetChild(0).gameObject.GetComponent<Text>().text = name;

        //this is to load a sprite from resources (Android)
        item.GetComponent<Image>().sprite = LoadSprite(imageAddress);
    }

    private Sprite LoadSprite(string path)
    {
        Debug.Log(path);
        if (string.IsNullOrEmpty(path)) return null;
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(900, 900, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.0f), 100.0f);
            return sprite;
        }
        return null;
    }
}
