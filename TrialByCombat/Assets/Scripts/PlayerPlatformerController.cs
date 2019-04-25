using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPlatformerController : PhysicsObject {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 15;
    
    [Range(0.01f, 2)]
    public float gravityMultiplierAtApex = 1.5f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    private bool _gravMultHasBeenApplied;
    

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
        if (grounded)
        {
            _gravMultHasBeenApplied = false;
        }
        
        var move = Vector2.zero;

        move.x = Input.GetAxis ("Horizontal");

        if (Input.GetButtonDown ("Jump") && grounded) {
            velocity.y = jumpTakeOffSpeed;
        } 
        else if (Input.GetButtonUp("Jump") && !_gravMultHasBeenApplied || IsFallingFromApex())
        {
            _gravMultHasBeenApplied = true;
            //Debug.Log("!grounded: " + !grounded + "\nMath.Abs(velocity.y): " + Math.Abs(velocity.y) + "\nMath.Abs(velocity.y) <= 0.01f: " + (Math.Abs(velocity.y) <= 0.01f));
            if (Math.Abs(velocity.y) > 0)
            {
                var negate = velocity.y > 0 ? -1 : 3f;
                velocity.y = velocity.y * negate * Math.Abs(gravityMultiplierAtApex);
            }
        }

        var flipSprite = spriteRenderer.flipX ? move.x > 0.01f : move.x < 0.01f;
        if (flipSprite) 
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        if (animator != null)
        {
            animator.SetBool("grounded", grounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        }

        targetVelocity = move * maxSpeed;
    }

    private bool IsFallingFromApex()
    {
        return !grounded && velocity.y < 0 && Math.Abs(velocity.y) >= 0.3f && Math.Abs(velocity.y) <= 0.5f;
    }
}