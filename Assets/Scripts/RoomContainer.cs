using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomContainer : MonoBehaviour
{
    public GameObject[] objects;
    public Platform[] platforms;
    public CameraManager cameraManager;

    public void Enable()
    {
        foreach (GameObject g in objects)
        {
            g.SetActive(true);
        }
        //TO-DO: Change music
    }
    public void Disable()
    {
        foreach (GameObject g in objects)
        {
            g.SetActive(false);
        }
        foreach (Platform p in platforms)
        {
            p.ResetPlatform();
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        cameraManager.lastRoom?.Disable();
        cameraManager.lastRoom = this;
        Enable();
    }
}
