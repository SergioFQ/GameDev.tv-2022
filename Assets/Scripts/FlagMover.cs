using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagMover : MonoBehaviour
{
    public Vector2 pos;
    public float offset;
    public GameObject flag;
    private bool _moved;
    void Start()
    {
        pos = flag.transform.position;
        if (PlayerPrefs.GetInt("MovedFlag", 0) == 1)
        {
            StartCoroutine(MoveFlag(true));
        }
    }

    public IEnumerator MoveFlag(bool instant = false)
    {
        _moved = true;
        PlayerPrefs.SetInt("MovedFlag", 1);
        float elapsedTime = 0;
        if (!instant)
        {
            while (elapsedTime < 1)
            {
                elapsedTime += Time.deltaTime;
                flag.transform.position = Vector2.Lerp(pos, pos + new Vector2(offset, 0), elapsedTime);

                yield return null;
            }
        }

        flag.transform.position = pos + new Vector2(offset, 0);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !_moved)
        {
            StartCoroutine(MoveFlag());
        }
    }
}
