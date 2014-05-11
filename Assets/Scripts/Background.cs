using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour
{
    public float scrollSpeed; 
    public float tileSizeZ;

    private Vector3 startPosition;

    void Start()
    {
        //transform.localScale.x =
        //transform.localScale = new Vector3(10.0f * Screen.height / Screen.width, 20.0f * Screen.height / Screen.width, 0);
        //transform.localScale = new Vector3(Screen.width, Screen.height);
     
        startPosition = transform.position;
    }

    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
        transform.position = startPosition + Vector3.forward * newPosition;
    }
}
