using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PhysicsObject {

	public float maxSpeed = 7;
	public float jumpTakeOffSpeed = 15;

	private SpriteRenderer spriteRenderer;
	private Animator animator;
	
	// Use this for initialization
	void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();

		if (gameObject.HasComponent<Animator>())
		{
			animator = GetComponent<Animator>();
		}
	}

	protected override void ChildUpdate()
	{
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
				Debug.Log(hit.collider);
				Debug.Log(hit.collider.gameObject);
				hitsString += LayerMask.LayerToName(hit.collider.gameObject.layer) + ", ";
			}
		}
	}

	protected override void ComputeVelocity()
	{
		var move = Vector2.zero;

		move.x = maxSpeed;

		var flipSprite = spriteRenderer.flipX ? move.x > 0.0f : move.x < 0.0f;
		if (flipSprite)
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}
		
		animator.SetBool("Walking", Mathf.Abs(velocity.x) / maxSpeed > 0);
        
		targetVelocity = move * maxSpeed;        
	}
}
