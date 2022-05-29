using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private Vector2 _movement;
    private bool _jump;
    public bool jump { get {return _jump;} }
    private float _jumpTimer;
    private bool _grounded;
    private float _rotateFactor;
    [SerializeField] private Vector2 _maxSpeed;
    [SerializeField] private Vector2 _acceleration;
    [SerializeField] private float _maxJumpTimer;
    [SerializeField] private float _maxFallSpeed;
    public bool isGhost;
    private bool _dying;
    public bool dying { get {return _dying;} set {_dying = value;} }
    private float _dieTimer;
    private bool _ghosting;
    public bool elevator;
    public bool kicked;
    public float holeX, holeY;
    public int fallSong;

    #region Object References

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _spriteRoot;

    #endregion
    #region Ground Detection
    private Vector2 _groundDetectionOffset = new Vector2(0, -0.505f);
    private Vector2 _groundDetectionSize = new Vector2(0.75f, 0.1366799f);
    #endregion

    #region Sounds
    public AudioSource jumpSound;
    public AudioSource fallSound;
    public AudioSource fakeDeathAlive;
    public AudioSource fakeDeathGhost;
    public AudioSource realDeath;
    public AudioSource realDeathGhost;
    public AudioSource hitBirdsSound;
    public AudioSource reviveSound;
    #endregion

    
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _movement = new Vector2();
        _grounded = true;
    }
    void FixedUpdate()
    {
        if (elevator)
        {
            if (!kicked)
            {
                _rb2d.gravityScale = 0;
                _spriteRoot.transform.rotation = Quaternion.identity;
                _rb2d.velocity = Vector2.zero;
            }
            else
            {
                _rb2d.velocity = Vector2.zero;
                _spriteRoot.transform.Rotate(new Vector3(0, 0, 5));
            }
            return;
        }

        if (_dying)
        {
            _rb2d.gravityScale = 0;
            _dieTimer += Time.fixedDeltaTime;
            if (_dieTimer > 1)
            {
                if (_dieTimer < 2)
                {
                    _rb2d.velocity = new Vector2(0, 3);
                }
                else
                {
                    _rb2d.AddForce(new Vector2(0, -10));
                }
                

                _spriteRoot.transform.Rotate(new Vector3(0, 0, 5));
            }
            else
            {
                _rb2d.velocity = Vector2.zero;
            }
            RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + _groundDetectionOffset, _groundDetectionSize, 0, Vector2.zero, 1, _groundLayer);
            if (hit.collider != null)
            {
                //TO-DO: Fall sound effect
                StartCoroutine(TurnIntoGhost());
            }
        }
        else
        {
            _rb2d.gravityScale = isGhost ? 2 : 4;
            //Check if grounded
            RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + _groundDetectionOffset, _groundDetectionSize, 0, Vector2.zero, 1, _groundLayer);
            if (hit.collider != null)
            {
                _jumpTimer = _maxJumpTimer;
                if (!_grounded)
                {
                    _grounded = true;
                    //Squash
                    if(!isGhost)fallSound.Play();
                }
            }
            else
            {
                _jumpTimer -= Time.fixedDeltaTime;
                if (_jumpTimer <= 0)
                {
                    _jumpTimer = 0;
                }
            }

            //Set movement
            Vector2 vel = _rb2d.velocity;
            if (_movement.x == 0)
            {
                vel.x = 0;

                if (Mathf.Abs(_rotateFactor) > 0)
                {
                    float sign = Mathf.Sign(_rotateFactor);
                    _rotateFactor -= sign * _acceleration.x * 1f;
                    if (sign != Mathf.Sign(_rotateFactor))
                    {
                        _rotateFactor = 0;
                    }
                }
            }
            else
            {
                vel.x = Mathf.Sign(_movement.x) * _maxSpeed.x;
                
                _rotateFactor += Mathf.Sign(_movement.x) * _acceleration.x;
                _rotateFactor = Mathf.Sign(_rotateFactor) * Mathf.Min(Mathf.Abs(_rotateFactor), _maxSpeed.x);
            }
            if (_jump)
            {
                if (_jumpTimer == 0)
                {
                    //vel.y += _acceleration.y;
                }
                else
                {
                    vel.y += _acceleration.y;
                }

                if (_grounded)
                {
                    _grounded = false;
                    //Stretch                    
                    jumpSound.Play();
                }
            }
            
            vel.y = Mathf.Clamp(vel.y, _maxFallSpeed, _maxSpeed.y);

            _rb2d.velocity = vel;

            //Set rotation
            Vector3 rotation = _spriteRoot.transform.eulerAngles;
            
            rotation.z = -10 * Mathf.Sign(_rotateFactor) * (1 - (_maxSpeed.x - Mathf.Abs(_rotateFactor)) / _maxSpeed.x);

            _spriteRoot.transform.eulerAngles = rotation;
        }
    }

    private void OnNavigate(InputValue value)
    {
        _movement = value.Get<Vector2>();
    }
        private void OnJump(InputValue value)
    {
        _jump = (value.Get<float>() == 1);
        if (!_jump)
        {
            StopJump();
        }
    }

    private void StopJump()
    {
        if (_jumpTimer != 0)
        {
            if (_rb2d.velocity.y > 0)
            {
                Vector2 vel = _rb2d.velocity;
                vel.y *= 0.5f;
                _rb2d.velocity = vel;
                _jumpTimer = 0;
            }
        }
    }
    void OnDrawGizmos()
    {
        //Ground detection hitbox
        Gizmos.DrawCube((Vector2)transform.position + _groundDetectionOffset, _groundDetectionSize);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (((col.CompareTag("KillZone") && !isGhost) || (col.CompareTag("KillZone2"))) && !_dying)
        {
            _dying = true;
            _ghosting = false;
            _dieTimer = 0;
            if(isGhost)
                fakeDeathGhost.Play();
            else
                fakeDeathAlive.Play();
            AudioManager.instance.ChangeSong(-1, 0, 0);
        }
        else if (col.CompareTag("Bird") && !kicked)
        {
            hitBirdsSound.Play();
            StartCoroutine(Bird());
        }
        else if (col.CompareTag("Reviver") && !elevator && isGhost)
        {
            reviveSound.Play();
            StartCoroutine(Revive());
        }
    }

    public IEnumerator TurnIntoGhost()
    {
        if (_ghosting) yield break;
        if(isGhost)
            realDeathGhost.Play();
        else
            realDeath.Play();
        _ghosting = true;

        GameObject body = Instantiate(_spriteRoot, _spriteRoot.transform.position, _spriteRoot.transform.rotation);
        body.layer = 6;
        Rigidbody2D bodyRB = body.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        bodyRB.mass = 9999;
        body.AddComponent(typeof(CapsuleCollider2D));
        _spriteRoot.SetActive(false);
        _rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1.5f);
        _rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (!isGhost)
        {
            transform.position = body.transform.position + new Vector3(0, 1, 0);
            isGhost = true;
            _spriteRoot.SetActive(true);
            _spriteRoot.transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.position = body.transform.position;
            Destroy(body);
            _spriteRoot.SetActive(true);
            _spriteRoot.transform.rotation = Quaternion.identity;
        }

        AudioManager.instance.ChangeSong(fallSong);

        _dying = false;
        _rb2d.velocity = Vector2.zero;
        _ghosting = false;
    }

    public IEnumerator Bird()
    {
        float elapsedTime = 0;
        float maxTime = 5;
        Vector2 firstPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 lastPos = new Vector2(holeX, holeY);

        elevator = true;
        kicked = true;

        while (elapsedTime < maxTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            transform.position = Vector2.Lerp(firstPos, lastPos, elapsedTime/maxTime);

            yield return new WaitForFixedUpdate();
        }

        elevator = false;
        kicked = false;

        _dying = true;
        _dieTimer = 2;
    }

    private IEnumerator Revive()
    {
        float elapsedTime = 0;
        float maxTime = 1.7f;
        Vector2 firstPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 lastPos = new Vector2(transform.position.x, transform.position.y + 1.0f);

        elevator = true;

        while (elapsedTime < maxTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            transform.position = Vector2.Lerp(firstPos, lastPos, elapsedTime/maxTime);

            yield return new WaitForFixedUpdate();
        }

        elevator = false;

        isGhost = false;
    }
}
