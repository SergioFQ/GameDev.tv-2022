using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool isMoving;
    public Vector2 offsets;
    public float maxFallTime;
    private float _fallTimer;
    private bool _activated;
    public bool isMovingRight;
    private Vector3 _startPos;
    private Rigidbody2D _rb2d;
    public float speed = 2;
    public SpriteRenderer sprite;
    #region Sounds
    public AudioSource platformSound;
    #endregion

    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _startPos = transform.position;
        ResetPlatform();
    }

    void FixedUpdate()
    {
        Vector2 vel = new Vector2();

        if (_activated)
        {
            if (_fallTimer > 0)
            {
                _fallTimer -= Time.fixedDeltaTime;
            }
            else
            {
                vel.x = 0;
                vel.y = -12;
            }
        }
        if (_fallTimer > 0 || _fallTimer == -1)
        {
            if (isMoving)
            {
                if (isMovingRight)
                {
                    if (transform.position.x < _startPos.x + offsets.y)
                    {
                        vel.x = speed;
                    }
                    else
                    {
                        isMovingRight = !isMovingRight;
                    }
                }
                else
                {
                    if (transform.position.x > _startPos.x - offsets.x)
                    {
                        vel.x = -speed;
                    }
                    else
                    {
                        isMovingRight = !isMovingRight;
                    }
                }
            }
        }


        _rb2d.velocity = vel;

        if (sprite != null)
        {
            sprite.flipX = !isMovingRight;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {            
            if (maxFallTime == -1) return;
            if (!col.enabled) return;
            if(!_activated)platformSound.Play();
            _activated = true;
        }
    }

    public void ResetPlatform()
    {
        _fallTimer = maxFallTime;
        transform.position = _startPos;
        _activated = false;
        _rb2d.velocity = Vector2.zero;
    }
}
