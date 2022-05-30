using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomContainer : MonoBehaviour
{
    public GameObject[] objects;
    public Platform[] platforms;
    public CameraManager cameraManager;
    public int music = -1;
    public PlayerController player;

    public void Enable()
    {
        foreach (GameObject g in objects)
        {
            g.SetActive(true);
        }
        
        foreach (Platform p in platforms)
        {
            p.ResetPlatform();
        }
        
        if (player != null)
        {
            if (!player.dying)
            {
                AudioManager.instance.ChangeSong(music);
            }
            else
            {
                player.fallSong = music;
            }
        }
        else
        {
            AudioManager.instance.ChangeSong(music);
        }
    }
    public void Disable()
    {
        foreach (GameObject g in objects)
        {
            g.SetActive(false);
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
