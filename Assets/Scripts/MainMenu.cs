using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject musicObject;

    void Start()
    {
        musicObject.GetComponent<AudioSource>().time += 1.5f;

        Screen.fullScreen = false;
        Cursor.visible = true;
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect((Screen.width / 2)-75, 180, 150, 80), "Play"))
        {
            Application.LoadLevel("play");
        }

        /*
        if (GUI.Button(new Rect((Screen.width / 2)-75, 280, 150, 80), "High Scores"))
        {
            Application.LoadLevel("highScores");
            DontDestroyOnLoad(musicObject);
        }*/

        if (GUI.Button(new Rect((Screen.width / 2)-75, 380, 150, 80), "Quit"))
        {
            Application.Quit();
        }
    }
}

