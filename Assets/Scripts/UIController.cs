using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public float distanceToMove;
    public float speed;
    public GameObject panel;

    private float _startY;
    private bool _isOpen = false;
    private bool _isMoving;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _startY = panel.transform.position.y;
        _isMoving = false;
        _isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(_isOpen)
        {
            if(_isMoving)
            {
                if(_startY-distanceToMove < panel.transform.position.y)
                {
                    panel.transform.position -= new Vector3(0.0f, speed*Time.deltaTime, 0.0f); 
                }
                else
                {
                    _isMoving = false;
                }
            }
        }
        else
            if(_isMoving)
            {
                if(_startY > panel.transform.position.y)
                {
                    panel.transform.position += new Vector3(0.0f, speed*Time.deltaTime, 0.0f); 
                }
                else
                    _isMoving = false;    
            }
    }

    public void OpenClose()
    {
        _isOpen = !_isOpen;
        _isMoving = true;
    }
}
