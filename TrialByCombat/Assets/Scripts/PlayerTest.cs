using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{

	public Transform playerSpawn;
	
	public float moveSpeed = 5f;
	public float rayCastCollisionCheckDistance = 5.0f;
	public float rayCastCollisionCheckSlowdownSpeed = 1.0f;
	public float jumpHeight = 2f;
	public float jumpDurationInSeconds = 0.28f;
	public float gravityMultiplierAtApex = 1.5f;
	
	public bool useGravityMultiplier = true;
	public bool startAtSpawnPoint = true;

	public LayerMask whatIsGround;

	private Collider2D _collider;
	private Rigidbody2D _rigidBody;
	private Vector2 _velocity;
	private bool _isGrounded;
	private bool _highPt;
	private float _jumpGravity;
	private RaycastHit _lastFrameRayCastHit;
    
	void Start () {
		if (!startAtSpawnPoint)
		{
			ConfigurePlayerSpawnPoint();
		}
		Restart();
		CalculateGravity();
		_rigidBody = GetComponent<Rigidbody2D> ();
		_collider = GetComponent<Collider2D>();
		_highPt = false;
		_isGrounded = true;
	}

	void Update ()
	{

		_isGrounded = Physics2D.IsTouchingLayers(_collider, whatIsGround);
		if (_isGrounded)
		{
			_highPt = false;
			CalculateGravity();
		}
		
		transform.Translate(transform.forward * Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed, Space.World);
		transform.Translate(transform.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed, Space.World);

		if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
		{
			Jump();
		}

		if (useGravityMultiplier && !_isGrounded && _rigidBody.velocity.y <= 0 && !_highPt)
		{
			_highPt = true;
			CalculateGravity(Math.Abs(gravityMultiplierAtApex));
		}

		if (!_isGrounded && _highPt && Physics.Raycast(transform.position, 
			    -Vector3.up, out _lastFrameRayCastHit, rayCastCollisionCheckDistance))
		{
			_rigidBody.velocity = new Vector2(0, rayCastCollisionCheckSlowdownSpeed);
			CalculateGravity();
		}
	}

	void Jump()
	{
		var initVelocity = 2 * jumpHeight / jumpDurationInSeconds;
		_rigidBody.velocity = new Vector2(0, initVelocity);
		_isGrounded = false;
	}

	/*void OnCollisionEnter2D(Collision2D collision)
	{
		_isGrounded = true;
		_highPt = false;
		CalculateGravity();
	}*/

	public void Restart()
	{
		if (!startAtSpawnPoint)
		{
			
		}
		
		transform.SetPositionAndRotation(playerSpawn.position, playerSpawn.rotation);
	}

	public void ConfigurePlayerSpawnPoint()
	{
		playerSpawn.position = new Vector2(transform.position.x, transform.position.y);
		playerSpawn.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
	}

	private void CalculateGravity(float gravityMultiplier = 1)
	{
		var gravity = -(2 * jumpHeight * gravityMultiplier) / (float)Math.Pow(jumpDurationInSeconds, 2);
		Physics2D.gravity = new Vector2(0, gravity);
	}
}
