﻿using UnityEngine;
using System.Collections;
using System;

public class PlatformerCharacter2D : MonoBehaviour
{

    [SerializeField]
    private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)]
    [SerializeField]
    private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField]
    private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField]
    private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings

    

    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.

    public int boxSpeedShot=1   ;
    private Transform box;
    private float shot_position; // временно
    private float localscale;

    

    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        box = null;
    }


    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_Grounded = true;
        }
        m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

        
        if (box)
        {
            box.Translate(boxSpeedShot * Time.deltaTime * localscale, 0, 0);
            

            
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch && m_Anim.GetBool("Crouch"))
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        // Set whether or not the character is crouching in the animator
        m_Anim.SetBool("Crouch", crouch);

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // Reduce the speed if crouching by the crouchSpeed multiplier
            //move = (crouch ? move*m_CrouchSpeed : move);

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", Mathf.Abs(move));


            // Move the character
            if (!crouch)
                m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump && m_Anim.GetBool("Ground"))
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void getBox(Collision2D box)
    {
        if (box.gameObject.tag=="Box")
        {
            box.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z);
        }

        if (box.gameObject.tag == "Apple")
        {
            box.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.2f, gameObject.transform.position.z);

            m_JumpForce = m_JumpForce/1.33f;
        }

        box.gameObject.transform.parent = gameObject.transform;

        GetComponent<Platformer2DUserControl>().handBox = true;

    }

    public void shotBox()
    {
        Transform box = findBox();

        
        this.box = box;
        box.position = new Vector2(box.transform.position.x, box.transform.position.y - 1);
        shot_position = box.transform.position.x;
        box.parent = null;
        localscale = transform.localScale.x;

        box.transform.localScale = new Vector3(Mathf.Abs(box.transform.localScale.x), box.transform.localScale.y, box.transform.localScale.z);

                
        if(box.tag == "Apple")
        {
            m_JumpForce = m_JumpForce * 1.33f;
        }

        //m_Anim.SetBool("Shot",true);
        m_Anim.SetTrigger("Shot1");
        GetComponent<Platformer2DUserControl>().handBox = false;
        
                   
    }

    public void Couch_box(bool couch_box)
    {
        Transform box = findBox();

        Transform _box;
        if (box.tag == "Box")
        {

            _box = box;
        }
        else _box = null;

        


        if(_box)
        { 
            if (couch_box)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                _box.position = new Vector2(_box.transform.position.x, _box.transform.position.y - 2);
            }
            else
            {
                GetComponent<SpriteRenderer>().enabled = true;
                _box.position = new Vector2(_box.transform.position.x, _box.transform.position.y + 2);
            }
        }
    }

    public void shotBoxCouch()
    {
        Transform box = findBox();

        this.box = box;
        //box.position = new Vector2(box.transform.position.x, box.transform.position.y - 2);
        shot_position = box.transform.position.x;
        box.parent = null;
        localscale = transform.localScale.x;

        box.transform.localScale = new Vector3(Mathf.Abs(box.transform.localScale.x), box.transform.localScale.y, box.transform.localScale.z);

        GetComponent<SpriteRenderer>().enabled = true;

        m_Anim.SetTrigger("Shot1");
        GetComponent<Platformer2DUserControl>().handBox = false;
    }

    private Transform findBox()
    {
        Transform[] trns = GetComponentsInChildren<Transform>();

        foreach (Transform box in trns)
        {
            if (box.tag == "Box" || box.tag == "Apple")
            {

                return box;
            }

        }

        return null;
    }


}
