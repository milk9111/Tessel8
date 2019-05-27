using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
	[Tooltip("The min ground normal Y")]
	public float minGroundNormalY = 0.65f;
	
	[Tooltip("The gravity force amount")]
	public float gravityModifier = 1f;

	protected Vector2 targetVelocity;
	protected bool grounded;
	protected Vector2 groundNormal;
	protected Rigidbody2D rb2d;
	protected Vector2 velocity;
	protected ContactFilter2D contactFilter;
	protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
	protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

	protected const float minMoveDistance = 0.001f;
	protected const float shellRadius = 0.01f;

	void OnEnable()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Use this for initialization
	void Start ()
	{
		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
		contactFilter.useLayerMask = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		targetVelocity = Vector2.zero;
		ComputeVelocity();
		ChildUpdate();
	}

	protected virtual void ComputeVelocity()
	{
		
	}

	protected virtual void ChildUpdate()
	{
		
	}

	void FixedUpdate()
	{
		velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
		velocity.x = targetVelocity.x;

		grounded = false;
		
		var deltaPosition = velocity * Time.deltaTime;
		
		var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

		var move = moveAlongGround * deltaPosition.x;
		Movement(move, false);

		move = Vector2.up * deltaPosition.y;
		Movement(move, true);
	}

	void Movement(Vector2 move, bool yMovement)
	{
		var distance = move.magnitude;
		if (distance > minMoveDistance)
		{
			var count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
			hitBufferList.Clear();
			for (var i = 0; i < count; i++)
			{
				hitBufferList.Add(hitBuffer[i]);
			}

			for (var i = 0; i < hitBufferList.Count; i++)
			{
				var currentNormal = hitBufferList[i].normal;
				if (currentNormal.y > minGroundNormalY)
				{
					grounded = true;
					if (yMovement)
					{
						groundNormal = currentNormal;
						currentNormal.x = 0;
					}
				}

				var projection = Vector2.Dot(velocity, currentNormal);
				if (projection < 0)
				{
					velocity = velocity - projection * currentNormal;
				}

				var modifiedDistance = hitBufferList[i].distance - shellRadius;
				distance = modifiedDistance < distance ? modifiedDistance : distance;
			}
		}
		
		rb2d.position = rb2d.position + move.normalized * distance;
	}
	
}

public static class hasComponent
{
	public static bool HasComponent<T>(this GameObject flag) where T : Component{
		return flag.GetComponent<T> () != null;
	}
}
