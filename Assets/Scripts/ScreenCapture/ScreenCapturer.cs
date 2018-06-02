using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCapturer : MonoBehaviour {

    public static ScreenCapturer instance = null;
    private Camera cam;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        cam = GetComponent<Camera>();
    }

    public void ScreenCap()
    {
        StartCoroutine(ScreenCapture());
    }


    private IEnumerator ScreenCapture()
    {

        yield return new WaitForEndOfFrame();

        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // Set the supplied RenderTexture as the active one
        RenderTexture.active = cam.targetTexture;


        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, true);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        // Restorie previously active render texture
        RenderTexture.active = currentActiveRT;


        Sprite screenCap = Sprite.Create(tex, new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), new Vector2(0,0),100);
        ScreenShotKeeper.instance.addScreenShot(screenCap);
    }

}
