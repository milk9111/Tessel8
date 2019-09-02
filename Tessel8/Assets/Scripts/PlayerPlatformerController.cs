using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPlatformerController : MonoBehaviour {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 15;
    public float gravityModifier = 2f;
    
    [Range(1, 100)]
    public int pausedFrames = 50;

    public string walkingFxName;
    public float secondsBetweenWalkingFx = 0.55f;

    public Animator animator;
    
    private bool _hasEndedPause;
    private bool _isPaused;
    private bool _isDisabled;
    private int _currPausedFrame;

    private GameObject _lastHit;

    private AudioManager _audioManager;
    private Guid _walkingFxAudioGuid;
    private bool _canPlayWalkingFx = true;
    private Coroutine _walkingFxCoroutine;

    private float _remainingSecondsOnTimer;
    
    public CharacterController2D controller;

    private float _horizontalMove;
    private bool _jump;

    void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _walkingFxAudioGuid = _audioManager.PrepareSound(walkingFxName);
        
        controller.SetJumpForce(jumpTakeOffSpeed);
        controller.SetGravityModifier(gravityModifier);
    }

    void Update()
    {
        if (_isDisabled) return;
        
        if (Grounded())
        {
            TouchFallingTiles();
        }
        
        _horizontalMove = Input.GetAxisRaw("Horizontal") * maxSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
        }
        
        if (_isPaused)
        {
            _currPausedFrame++;
        }
    }

    void FixedUpdate()
    {
        if (_isPaused || _isDisabled)
        {
            return;
        }
        
        controller.Move(_horizontalMove * Time.fixedDeltaTime, false, _jump);
        _jump = false;
        
        animator.SetBool("Walking", Mathf.Abs(Velocity().x) / maxSpeed > 0);

        if (Math.Abs(_horizontalMove) > 0 && _canPlayWalkingFx && !_audioManager.IsPlaying(_walkingFxAudioGuid))
        {
            _walkingFxCoroutine = StartCoroutine(WalkingFxCooldown());
            _audioManager.Play(_walkingFxAudioGuid);
        }
    }

    private void TouchFallingTiles()
    {
        var foundHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.1f), transform.up * -1, 0.7f, 1<<LayerMask.NameToLayer("Ground"));
        if (foundHit.rigidbody != null && foundHit.rigidbody.gameObject != null && string.Equals(foundHit.rigidbody.gameObject.name, "FallingTile"))
        {
            foundHit.rigidbody.gameObject.GetComponent<FallingTile>().Touch();
        }
    }

    /*protected override void ChildUpdate()
    {
        if (_isDisabled) return;

        if (grounded)
        {
            var foundHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.1f), transform.up * -1, 0.7f, 1<<LayerMask.NameToLayer("Ground"));
            if (foundHit.rigidbody != null && foundHit.rigidbody.gameObject != null && string.Equals(foundHit.rigidbody.gameObject.name, "FallingTile"))
            {
                foundHit.rigidbody.gameObject.GetComponent<FallingTile>().Touch();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.NameToLayer("Ground"));
            
            var clickPosRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = new RaycastHit2D[6];
            var noOfHits = Physics2D.Raycast(clickPosRay.origin, clickPosRay.direction, contactFilter, hits);
            
            var hitsString = "";
            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;
                hitsString += LayerMask.LayerToName(hit.collider.gameObject.layer) + ", ";
            }
        }

        if (_isPaused)
        {
            _currPausedFrame++;
        }
    }*/

    /*protected override void ComputeVelocity()
    {
        var move = Vector2.zero;

        if (_isPaused || _isDisabled)
        {
            return;
        }
        
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            //animator.SetTrigger("Jumping");
            velocity.y = jumpTakeOffSpeed;
        } 

        bool flipSprite;
        if (hasAnimationBones)
        {
            flipSprite = _isFlipped ? move.x > 0.0f : move.x < 0.0f;
        }
        else
        {
            flipSprite = spriteRenderer.flipX ? move.x > 0.0f : move.x < 0.0f;
        }

        if (flipSprite)
        {
            if (hasAnimationBones)
            {
                FlipSprite(!_isFlipped);
            }
            else
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
		
        animator.SetBool("Walking", Mathf.Abs(velocity.x) / maxSpeed > 0);

        if (Math.Abs(move.x) > 0 && _canPlayWalkingFx && !_audioManager.IsPlaying(_walkingFxAudioGuid))
        {
            _walkingFxCoroutine = StartCoroutine(WalkingFxCooldown());
            _audioManager.Play(_walkingFxAudioGuid);
        }
        targetVelocity = move * maxSpeed;        
    }*/

    public void OnPause()
    {
        DisableMovement();
        FreezePosition(true, true);
    }

    public void OnPlay()
    {
        EnableMovement();
        FreezePosition(false, false);
    }

    public void DisableMovement()
    {
        if (_walkingFxCoroutine != null)
        {
            StopCoroutine(_walkingFxCoroutine);
        }
        _isDisabled = true;
    }
    
    public void EnableMovement()
    {
        _walkingFxCoroutine = StartCoroutine(WalkingFxCooldown());
        _isDisabled = false;
    }

    public void StartPlayerMotionPause()
    {
        if (Grounded()) return;
        _isPaused = true;
        _hasEndedPause = false;
        _currPausedFrame = 0;
        StartCoroutine(PausePlayerMotion());
    }

    public bool Grounded()
    {
        return controller.Grounded();
    }

    public void StopPlayerMotionPause()
    {
        _hasEndedPause = true;
        _isPaused = false;
    }

    private IEnumerator WalkingFxCooldown()
    {
        _canPlayWalkingFx = false;
        var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : secondsBetweenWalkingFx;
        for (_remainingSecondsOnTimer = timerLength;
            _remainingSecondsOnTimer > 0;
            _remainingSecondsOnTimer -= Time.deltaTime)
            yield return null;
        _canPlayWalkingFx = true;
    }

    private IEnumerator PausePlayerMotion()
    {
        var lastVelocity = Velocity();
        SetVelocity(Vector2.zero);
        
        FreezePosition(true, true);
        
        yield return new WaitUntil(() => _currPausedFrame >= pausedFrames || _hasEndedPause );
        StopPlayerMotionPause();
        SetVelocity(lastVelocity);
        FreezePosition(false, false);
    }
    
    public Vector2 Velocity()
    {
        var vel = controller.Velocity();
        if (Mathf.Abs(vel.x) / maxSpeed <= 0.01f)
        {
            vel.x = 0;
        }

        if (Mathf.Abs(vel.y) / jumpTakeOffSpeed <= 0.01f)
        {
            vel.y = 0;
        }

        return vel;
    }

    public void SetVelocity(Vector2 velocity)
    {
        controller.SetVelocity(velocity);
    }

    public float Gravity()
    {
        return controller.Gravity();
    }

    public void FreezePosition(bool freezeX, bool freezeY)
    {
        controller.FreezePosition(freezeX, freezeY);
    }
}