
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class FallingTile : PhysicsObject
{
    public float fallSpeed;
    public float timeTillDropInSeconds;
    public float cooldownTimeInSeconds;
    
    private bool _isPaused;
    private bool _isDisabled;
    private bool _isTouched;
    private bool _isTimerTillDropOn;
    private bool _isCooldownOn;

    private float _remainingSecondsOnTimer1;
    private float _remainingSecondsOnTimer2;
    private float _remainingSecondsOnTimer3;
    private float _prevAnimatorSpeed;
    private float _startingAnimatorSpeed;

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

        _startingAnimatorSpeed = _animator.GetFloat("AnimSpeed");
    }
    
    protected override void ChildUpdate()
    {
        if (_isPaused) return;
        if (_isTouched && _coroutine == null)
        {
            _animator.SetBool("Shake", true);
            _coroutine = StartCoroutine(StartTimerTillDrop());
        }
    }

    private IEnumerator StartTimerTillDrop()
    {
        _isTimerTillDropOn = true;
        var timer1Length = _remainingSecondsOnTimer1 > 0 ? _remainingSecondsOnTimer1 : timeTillDropInSeconds;
        for(_remainingSecondsOnTimer1 = timer1Length; _remainingSecondsOnTimer1 > 0; _remainingSecondsOnTimer1 -= Time.deltaTime)
            yield return null;
        
        var timer2Length = _remainingSecondsOnTimer2 > 0 ? _remainingSecondsOnTimer2 : timeTillDropInSeconds;
        for(_remainingSecondsOnTimer2 = timer2Length; _remainingSecondsOnTimer2 > 0; _remainingSecondsOnTimer2 -= Time.deltaTime)
            yield return null;
        _isTimerTillDropOn = false;
        _animator.SetBool("Fall", true);
        _collider.enabled = false;
        _coroutine = StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        _isCooldownOn = true;
        var timer2Length = _remainingSecondsOnTimer3 > 0 ? _remainingSecondsOnTimer3 : cooldownTimeInSeconds;
        for(_remainingSecondsOnTimer3 = timer2Length; _remainingSecondsOnTimer3 > 0; _remainingSecondsOnTimer3 -= Time.deltaTime)
            yield return null;
        _isCooldownOn = false;
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

    public void OnPause()
    {
        _isPaused = true;
        _prevAnimatorSpeed = _animator.GetFloat("AnimSpeed");
        _animator.SetFloat("AnimSpeed", 0);
        if (_coroutine == null) return;
        StopCoroutine(_coroutine);
    }

    public void OnPlay()
    {
        _isPaused = false;
        _animator.SetFloat("AnimSpeed", _prevAnimatorSpeed);
        if (_isTimerTillDropOn)
        {
            _coroutine = StartCoroutine(StartTimerTillDrop());
        } else if (_isCooldownOn)
        {
            _coroutine = StartCoroutine(StartCooldown());
        }
    }

    public void OnStart()
    {
        _animator.SetFloat("AnimSpeed", _startingAnimatorSpeed);
        _animator.SetBool("Shake", false);
        _animator.SetBool("Fall", false);
        _animator.Rebind();
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
            _remainingSecondsOnTimer1 = 0;
            _remainingSecondsOnTimer2 = 0;
            _remainingSecondsOnTimer3 = 0;
            _isPaused = false;
            _isTouched = false;
            _isDisabled = false;
            _isCooldownOn = false;
            _isTimerTillDropOn = false;
            _renderer.enabled = true;
            _collider.enabled = true;
        }
    }

    public void Touch()
    {
        _isTouched = true;
    }
}
