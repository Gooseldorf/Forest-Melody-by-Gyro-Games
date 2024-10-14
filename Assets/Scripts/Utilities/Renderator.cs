using UnityEngine;
//using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class Renderator : MonoBehaviour
{
    [SerializeField]
    string screenShotPath;
    [SerializeField]
    int resolutionMultiplier;

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Backspace)) //Keyboard.current[Key.Backspace].wasReleasedThisFrame
        {
            if (!string.IsNullOrEmpty(screenShotPath))
            {
                ScreenCapture.CaptureScreenshot(screenShotPath + "/Render"  // path to folder with renders and firts part of the name (Render)
                                                               + System.DateTime.Now.ToString("_yyyy-MM-dd_") // giving date to name
                                                               + System.DateTime.Now.ToString("hh-mm-ss_") // giving current time to name
                                                               + ".png", resolutionMultiplier); //screenshot with resolution multipler
                Debug.Log("Screenshot was taken to " + screenShotPath);
            }
            else
            {
                Debug.LogError("ScreenShot path is empty");
            }

        }
    }
#endif
}
