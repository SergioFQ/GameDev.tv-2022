using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public PlayerController player;
    public SpriteRenderer doors;
    public Animator doorAnimator;
    public Animator legAnimator;
    public float endY;
    private float _startY;
    private bool _moving;
    public float endPlayerX;
    public AudioSource kickSound;

    
    void Start()
    {
        _startY = transform.position.y;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !_moving)
        {
            _moving = true;
            StartCoroutine(ElevatorMovement());
        }
    }
    
    public IEnumerator ElevatorMovement()
    {
        player.elevator = true;
        player.col.isTrigger = true;
        yield return new WaitForSeconds(1);
        player.transform.SetParent(this.transform);
        doors.sortingOrder = 3;
        doorAnimator?.SetTrigger("Close");
        AudioManager.instance.ChangeSong(3, 2, 2);
        yield return new WaitForSeconds(2);
        float elapsedTime = 0;
        float maxTime = 29;
        Vector2 firstPos = new Vector2(transform.position.x, _startY);
        Vector2 lastPos = new Vector2(transform.position.x, endY);

        while (elapsedTime < maxTime)
        {
            AudioManager.instance.SetSpeed(player.jump ? 2 : 1);
            elapsedTime += Time.fixedDeltaTime * (player.jump ? 2 : 1);
            transform.position = Vector2.Lerp(firstPos, lastPos, elapsedTime/maxTime);

            yield return new WaitForFixedUpdate();
        }

        AudioManager.instance.SetSpeed(1);
        AudioManager.instance.ChangeSong(-1);
        transform.position = lastPos;
        doorAnimator?.SetTrigger("Open");
        yield return new WaitForSeconds(2);
        doors.sortingOrder = 1;

        legAnimator?.SetTrigger("Kick");

        yield return new WaitForSeconds(3.25f);
        kickSound.Play();

        player.kicked = true;
        player.transform.SetParent(null);
        player.col.isTrigger = false;
        
        elapsedTime = 0;
        maxTime = 6;
        firstPos = new Vector2(player.transform.position.x, player.transform.position.y);
        lastPos = new Vector2(endPlayerX, player.transform.position.y);

        while (elapsedTime < maxTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            player.transform.position = Vector2.Lerp(firstPos, lastPos, elapsedTime/maxTime);

            yield return new WaitForFixedUpdate();
        }

        player.kicked = false;

        yield return new WaitForEndOfFrame();

        player.elevator = false;

        transform.position = new Vector2(transform.position.x, _startY);
        _moving = false;
    }
}
