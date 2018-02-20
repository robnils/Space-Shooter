using UnityEngine;
using System.Collections;

public class HighScores : MonoBehaviour 
{
    void Start()
    { 
       
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect((Screen.width / 2), 180, 150, 80), "Go back"))
        {
            Application.LoadLevel("menu");
        }
    }
}
