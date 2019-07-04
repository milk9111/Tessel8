
using System.Collections;
using UnityEngine;

public class FallingTile : PhysicsObject
{
    public float fallSpeed;
    public float timeTillDropInSeconds;
    public float cooldownTimeInSeconds;
    
    private bool _isPaused;
    private bool _isDisabled;
    private bool _isTouched;

    private float _remainingSecondsOnTimer;

    private Color _originalColor;

    private Coroutine _coroutine;

    private SpriteRenderer _renderer;

    private Collider2D _collider;

    private Animator _animator;

    void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();

        gravityModifier = 0;
        
        _originalColor = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b);
    }
    
    protected override void ChildUpdate()
    {
        if (_isTouched && _coroutine == null)
        {
            _animator.SetBool("Shake", true);
            _coroutine = StartCoroutine(StartTimerTillDrop());
        }
    }

    /*protected override void ComputeVelocity()
    {
        var move = Vector2.zero;

        if (_isPaused || _isDisabled || !_isTouched)
        {
            return;
        }
        
        move.y = fallSpeed;
        
        targetVelocity = move;        
    }*/

    private IEnumerator StartTimerTillDrop()
    {
        yield return new WaitForSeconds(timeTillDropInSeconds);
        
        var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : timeTillDropInSeconds;
        for(_remainingSecondsOnTimer = timerLength; _remainingSecondsOnTimer > 0; _remainingSecondsOnTimer -= Time.deltaTime)
            //_renderer.material.color = new Color(_renderer.material.color.r + 20, _renderer.material.color.g + 20, _renderer.material.color.b + 20);
            yield return null;
        _animator.SetBool("Fall", true);
        _collider.enabled = false;
        _coroutine = StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(cooldownTimeInSeconds);
        _isTouched = false;
        _renderer.enabled = true;
        _collider.enabled = true;
        _renderer.color = _originalColor;
        _coroutine = null;
    }

    public void TurnOffRenderer()
    {
        _renderer.enabled = false;
        _animator.SetBool("Fall", false);
        _animator.SetBool("Shake", false);
    }

    public void Touch()
    {
        _isTouched = true;
    }
}
