using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnemyController : PhysicsObject {

	[Tooltip("The movement speed of the enemy")]
	[UnityEngine.Range(0.01f, 1)]
	public float speed = 0.5f;
	
	[Tooltip("The speed of the jump take off")]
	public float jumpTakeOffSpeed = 15;
	
	[Tooltip("The distance of the forward ground detection ray cast")]
	public float raycastDistance = 5f;
	
	[Tooltip("The distance the enemy stops from the target")]
	public float stoppingDistance = 0.1f;

	[Tooltip("Follow target debug. Defaults to right direction.")]
	public bool followTarget;
	
	[Tooltip("Show debug rays")]
	public bool showRays;
	
	[Tooltip("The target transform to follow")]
	public Transform target;

	private SpriteRenderer _spriteRenderer;
	private Animator _animator;
	private bool _foundHit;
	private LineRenderer _lineRenderer;
	private int _direction;
	
	// Use this for initialization
	void Awake ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer> ();

		if (gameObject.HasComponent<Animator>())
		{
			_animator = GetComponent<Animator>();
		}

		if (showRays)
		{
			_lineRenderer = gameObject.AddComponent<LineRenderer>();
		}

		if (target == null)
		{
			target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		}
	}
	
	protected override void ChildUpdate()
	{
		_foundHit = Physics2D.Raycast(transform.position, transform.right, raycastDistance, (1<<LayerMask.NameToLayer("Ground")));
		if (_foundHit && grounded)
		{
			velocity.y = jumpTakeOffSpeed;
		}

		var newX = Vector2.MoveTowards(rb2d.position,
			           target.position,
			           speed * Time.deltaTime).x;
		if (Math.Abs(newX - target.position.x) <= stoppingDistance)
		{
			_direction = 0;
		}
		else
		{
			_direction = newX > rb2d.position.x ? 1 : -1;
		}

		if (!followTarget)
		{
			_direction = 1;
		}
		
		DrawRay();
	}

	protected override void ComputeVelocity()
	{
		var move = Vector2.zero;

		move.x = _direction * speed;

		var flipSprite = _spriteRenderer.flipX ? move.x > 0.0f : move.x < 0.0f;
		if (flipSprite)
		{
			_spriteRenderer.flipX = !_spriteRenderer.flipX;
		}
		
		_animator.SetBool("Walking", Mathf.Abs(velocity.x) / speed > 0);
        
		targetVelocity = move;        
	}

	private void DrawRay()
	{
		if (_lineRenderer == null)
		{
			return;
		}
		
		var material = new Material(Shader.Find("Custom/DefaultRayCast"));
		material.color = _foundHit ? Color.green : Color.red;
		
		_lineRenderer.startColor = _lineRenderer.endColor = _foundHit ? Color.green : Color.red;
		_lineRenderer.material = material;
		_lineRenderer.startWidth =  0.25f;
		_lineRenderer.endWidth = 0.25f;
		_lineRenderer.SetVertexCount(2);
		_lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1.02f));
		_lineRenderer.SetPosition(1, new Vector3(transform.position.x + raycastDistance, transform.position.y, -1.02f));
	}
}
