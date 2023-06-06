using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Body
{
    public string text;
    public string imgUrl;
    public string videoUrl;
    public string imageBase64;
}
public class Controller : MonoBehaviour
{
    public Image imagePreview;
    public TMP_Text text;
    string base64Image;
    public TMP_InputField messageField;
    public TMP_InputField imageUrlField;
    public TMP_InputField videoUrlField;
    Texture2D currentTexture;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void ShareData()
    {
        string hintUrl = "wss://nostr.zebedee.cloud";
        Body bodyObject = new Body();
        bodyObject.text = "What sounds does an ostrich make?";

        if (messageField.text.Length != 0)
        {
            bodyObject.text = messageField.text;
        }

        bodyObject.imgUrl = "https://pbs.twimg.com/profile_images/1604195803748306944/LxHDoJ7P_400x400.jpg";
        if (imageUrlField.text.Length > 0)
        {
            bodyObject.imgUrl = imageUrlField.text;
        }
        bodyObject.videoUrl = "https://i.imgur.com/WC7LW4t.mp4";
        if (videoUrlField.text.Length > 0)
        {
            bodyObject.videoUrl = videoUrlField.text;
        }
        if (base64Image != null)
        {
            bodyObject.imageBase64 = base64Image;
        }

        string body = JsonUtility.ToJson(bodyObject);
        string deepLink = "nostr-share://?msg=" + System.Net.WebUtility.UrlEncode(body) + "&hint=" + System.Net.WebUtility.UrlEncode(hintUrl);
        text.text = deepLink;
        Application.OpenURL(deepLink);
    }

    public Texture2D ResizeTexture(Texture2D originalTexture, int newWidth, int newHeight)
    {
        // Create a new texture with the desired dimensions
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight);

        // Scale the original texture data onto the new texture
        Color[] resizedColors = new Color[newWidth * newHeight];
        float ratioX = (float)newWidth / originalTexture.width;
        float ratioY = (float)newHeight / originalTexture.height;
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int index = (int)(x / ratioX) + (int)(y / ratioY) * originalTexture.width;
                resizedColors[x + y * newWidth] = originalTexture.GetPixelBilinear(x / (float)newWidth, y / (float)newHeight);
            }
        }

        // Set the pixels of the resized texture
        resizedTexture.SetPixels(resizedColors);
        resizedTexture.Apply();

        return resizedTexture;
    }

    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100.0f);
    }


    public void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                currentTexture = NativeGallery.LoadImageAtPath(path, -1, false);
                if (currentTexture == null)
                {
                    text.text = "Couldn't load texture from " + path;
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                int newWidth = 400;
                int newHeight = 400;
                currentTexture = ResizeTexture(currentTexture, newWidth, newHeight);

                byte[] imageBytes = currentTexture.EncodeToPNG();
                base64Image = Convert.ToBase64String(imageBytes);



                imagePreview.sprite = TextureToSprite(currentTexture);

            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }
}
