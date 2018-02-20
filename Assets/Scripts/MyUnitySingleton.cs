using UnityEngine;
using System.Collections;

public class MyUnitySingleton : MonoBehaviour
{
    private float musicTime; // Current time into music track
    public GameObject musicObject;

    private static MyUnitySingleton instance = null;
    public static MyUnitySingleton Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        // Don't reinitalise the object if it exists
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        musicTime = musicObject.GetComponent<AudioSource>().time;
    }

    void OnLevelWasLoaded(int level)
    {
        musicObject.GetComponent<AudioSource>().time = musicTime;

        if (level == 2)
            print("Woohoo");
    }
}
