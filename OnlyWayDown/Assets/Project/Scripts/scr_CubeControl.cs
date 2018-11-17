using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_CubeControl : MonoBehaviour {

    /*[SerializeField] myInput input;*/

    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_MaxFallSpeed = 12f;
    [SerializeField] private float m_MaxGlideSpeed = 3f;

    private float trueFallSpeed;

    [SerializeField] private float m_JumpForce = 400f;
    [SerializeField] private float m_GravityUp = 10f;
    [SerializeField] private float m_GravityStop = 10f;
    [SerializeField] private float m_GravityDown = 10f;
    [SerializeField] private float m_GravityWall = 10f;
    [SerializeField] private float m_WallJumpFactor = 0.8f;
    [SerializeField] private float m_SwingJumpFactor = 0.8f;
    [SerializeField] private float m_KnockTime = 0.25f;
    [SerializeField] private float m_PushTime = 0.1f;
    [SerializeField] private float m_SwingForce = 10f;
    [SerializeField] public bool m_Grounded;
    [SerializeField] public bool m_Walled;
    [SerializeField] public bool m_UseMove = true;
    [SerializeField] private bool m_CanDoubleJump;
    [SerializeField] private bool m_CanWallJump;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private bool m_Jumped = false;
    [SerializeField] private bool m_Knocked = false;
    [SerializeField] private bool m_CanUnknocked = false;
    [SerializeField] public bool m_CanHook;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] public Animator m_Animator;

    [SerializeField] private float m_maxHP = 6f;
    [SerializeField] private float m_currentHP = 6f;

    public Sprite[] healthbar;
    public GameObject HPbar;
    private bool dead = false;
    public GameObject endScreen;

    private Rigidbody2D m_Rigidbody2D;
    private float m_currentVelocityX;
    private Transform m_GroundCheck;
    const float k_GroundedRadius = .3f;
    private Transform m_WallCheck;
    const float k_WallRadius = .1f;
    private bool m_FacingRight = true;
    private ParticleSystem part_Trail;
    private ParticleSystem part_Jump;
    private ParticleSystem part_Lose;

    private Vector3 SpawnPoint;

    public string MyPlayer;
    private string controller = "P1";
    private string mouse = "P2";

    private bool m_Jump;

    public bool vemando = true;

    public AudioSource a_Jump;
    public AudioSource a_Knocked;

    /*public enum myInput
    {
        Mouse,
        Controller
    }*/
    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_GroundCheck = transform.Find("Checks").transform.Find("GroundCheck");
        m_WallCheck = transform.Find("Checks").transform.Find("WallCheck");
        part_Trail = transform.Find("Particles").transform.Find("TrailParticle").GetComponent<ParticleSystem>();
        part_Jump = transform.Find("Particles").transform.Find("JumpParticle").GetComponent<ParticleSystem>();
        part_Lose = transform.Find("Particles").transform.Find("LoseParticle").GetComponent<ParticleSystem>();
        /*switch (input)
        {
            case myInput.Mouse:
                MyPlayer = mouse;
                break;

            case myInput.Controller:
                MyPlayer = controller;
                break;
        }*/

        SpawnPoint = transform.position;

        


    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (m_currentHP <= 0 && !dead)
        {
            KillMe();
        }

        if (m_Rigidbody2D.velocity == new Vector2 (0,0) || m_Walled)
        {
            part_Trail.Stop();
        }
        else
        {
            part_Trail.Play();
        }

        //this.gameObject.GetComponent<scr_ThrowHook>().myPlayer = MyPlayer;
        if (vemando)
        {
            MyPlayer = controller;
        }
        else if (!vemando)
        {
            MyPlayer = mouse;
        }
        // Read inputs in Update so button presses aren't missed.
        if (!m_Jump)
            m_Jump = Input.GetButtonDown(MyPlayer + "Jump");

        if (Input.GetKeyDown(KeyCode.C))
        {
            vemando = !vemando;
        }
    }

    private void FixedUpdate()
    {

        if (Input.GetAxisRaw(MyPlayer + "Horizontal") != 0 && m_CanUnknocked /*&& m_Grounded*/)
        {
            if (Input.GetAxisRaw(MyPlayer + "Horizontal") < 0 && m_Rigidbody2D.velocity.x > 0)
            {
                m_Knocked = false;
            }
            else if (Input.GetAxisRaw(MyPlayer + "Horizontal") > 0 && m_Rigidbody2D.velocity.x < 0)
            {
                m_Knocked = false;
            }
            else if (m_Rigidbody2D.velocity.x == 0)
            {
                m_Knocked = false;
            }
        }

        float h = Input.GetAxis(MyPlayer + "Horizontal");

        m_Animator.SetFloat("myInput", Mathf.Abs(h));
        m_Animator.SetFloat("YSpeed", m_Rigidbody2D.velocity.y);
        m_Animator.SetBool("Grounded", m_Grounded);
        m_Animator.SetBool("Swinging", !m_UseMove);
        m_Animator.SetBool("Gliding", m_Walled);

        if (m_UseMove)
        {
            Move(h, m_Grounded, m_Jump);
        }
        else if (!m_UseMove)
        {
            Swing(m_Jump);
        }

        
        m_Jump = false;
        m_Grounded = false;
        m_Walled = false;
        m_CanWallJump = false;
        m_currentVelocityX = m_Rigidbody2D.velocity.x;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                m_CanDoubleJump = true;
                m_CanHook = true;
                if (m_Rigidbody2D.velocity.y < 0)
                {
                    m_Jumped = false;
                }
            }
        }

        Collider2D[] colliders1 = Physics2D.OverlapCircleAll(m_WallCheck.position, k_WallRadius, m_WhatIsGround);
        for (int i = 0; i < colliders1.Length; i++)
        {
            if (colliders1[i].gameObject != gameObject)
            {
                m_Walled = true;
                m_CanWallJump = true;
            }
        }

        trueFallSpeed = m_MaxFallSpeed;

        if (m_Rigidbody2D.velocity.y > 0 && Input.GetButton(MyPlayer + "Jump") && !m_Knocked && m_Jumped)
        {
            m_Rigidbody2D.gravityScale = m_GravityUp;
        }
        else if (m_Rigidbody2D.velocity.y > 0 && !Input.GetButton(MyPlayer + "Jump") && !m_Knocked && m_Jumped)
        {
            m_Rigidbody2D.gravityScale = m_GravityStop;
        }
        else if (m_Rigidbody2D.velocity.y <= 0 && !m_Grounded && m_Walled)
        {
            m_Rigidbody2D.gravityScale = m_GravityWall;
            trueFallSpeed = m_MaxGlideSpeed;
        }
        else
        {
            m_Rigidbody2D.gravityScale = m_GravityDown;
        }
    }

    public void Move(float move, bool fly, bool jump)
    {
        if (!dead)
        {
            if (m_AirControl)
            {
                if (m_Knocked && !m_Grounded /*&& Input.GetAxisRaw(MyPlayer + "Horizontal") == 0*/)
                {
                    if (Input.GetAxisRaw(MyPlayer + "Horizontal") < 0 && m_Rigidbody2D.velocity.x < 0)
                    {
                        m_Rigidbody2D.velocity = new Vector2(m_currentVelocityX, m_Rigidbody2D.velocity.y);

                    }
                    if (Input.GetAxisRaw(MyPlayer + "Horizontal") > 0 && m_Rigidbody2D.velocity.x > 0)
                    {
                        m_Rigidbody2D.velocity = new Vector2(m_currentVelocityX, m_Rigidbody2D.velocity.y);

                    }
                }
                else
                    m_Rigidbody2D.velocity = new Vector2(move * m_Speed, m_Rigidbody2D.velocity.y);
            }

            // If the player should jump...
            if ((m_Grounded) && jump)
            {
                m_Knocked = false;
                JumpAsisst(0, m_JumpForce);
            }
            else if (m_CanWallJump && jump)
            {
                if (m_FacingRight)
                {
                    GetPushed(new Vector3(-1, 1, 0), m_JumpForce * m_WallJumpFactor);
                }
                else if (!m_FacingRight)
                {
                    GetPushed(new Vector3(1, 1, 0), m_JumpForce * m_WallJumpFactor);
                }
            }

            if (m_Rigidbody2D.velocity.y < 0 && Mathf.Abs(m_Rigidbody2D.velocity.y) > trueFallSpeed && !m_Knocked)
            {
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -trueFallSpeed);
            }

            if (m_Rigidbody2D.velocity.x > 0 && !m_FacingRight)
                Flip();
            else if (m_Rigidbody2D.velocity.x < 0 && m_FacingRight)
                Flip();
        }
    }

    public void Swing(bool jump)
    {
        if (!dead)
        {
            m_Rigidbody2D.AddForce(Vector2.right * Input.GetAxisRaw(MyPlayer + "Horizontal") * m_SwingForce);

            if (jump)
            {
                GetComponent<scr_ThrowHook>().UnHook();
                GetPushed(new Vector3(Input.GetAxisRaw(MyPlayer + "Horizontal"), 1, 0), m_JumpForce * m_SwingJumpFactor);
            }

            /*if (Input.GetAxisRaw(MyPlayer + "Horizontal") > 0 && !m_FacingRight)
                Flip();
            else if (Input.GetAxisRaw(MyPlayer + "Horizontal") < 0 && m_FacingRight)
                Flip();*/
        }
    }

    public void Hooked()
    {
        m_UseMove = false;
    }

    public void UnHooked()
    {
        m_CanDoubleJump = true;
        m_AirControl = true;
        m_UseMove = true;
    }

    public void JumpAsisst(float x, float y)
    {
        a_Jump.Play();
        part_Jump.Play();
        m_Jumped = true;
        m_Grounded = false;
        if (!m_Grounded)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_CanDoubleJump = false;
        }
        m_Rigidbody2D.AddForce(new Vector2(x, y));
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void GetKnocked(Vector3 KnockDir, float Force)
    {
        a_Knocked.Play();
        KnockDir = KnockDir.normalized;

        m_Rigidbody2D.velocity = new Vector2(0, 0);
        m_AirControl = false;
        m_Knocked = true;
        m_CanUnknocked = false;
        m_Rigidbody2D.AddForce(KnockDir * Force);
        part_Jump.Play();
        Invoke("RestoreAirControl", m_KnockTime);
        
    }

    public void GetPushed(Vector3 PushDir, float Force)
    {
        a_Jump.Play();
        m_Grounded = false;
        m_Jumped = false;
        PushDir = PushDir.normalized;

        if (PushDir.x == 0)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        }
        else
        {
            m_Rigidbody2D.velocity = new Vector2(0, 0);
            m_AirControl = false;
            Invoke("RestoreAirControl", m_PushTime);
            m_Knocked = true;
            m_CanUnknocked = false;
        }

        m_CanDoubleJump = true;
        m_Rigidbody2D.AddForce(PushDir * Force);
        part_Jump.Play();
        //Invoke("RestoreAirControl", 0.2f);
    }

    void RestoreAirControl()
    {
        m_AirControl = true;
        m_CanUnknocked = true;
    }

    public void Defeat()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        m_Rigidbody2D.isKinematic = true;
        part_Trail.gameObject.SetActive(false);
        part_Jump.gameObject.SetActive(false);
        part_Lose.Play();
        m_Speed = 0;
        Invoke("KillMe", 2.5f);
    }

    public void Victory()
    {
        Invoke("NextLevel", 2.5f);
    }

    public void DamageMe(float dmg)
    {
        m_currentHP -= dmg;
        HPbar.GetComponent<SpriteRenderer>().sprite = healthbar[Mathf.RoundToInt(m_currentHP)];
    }

    public void NewCheckpoint()
    {
        m_currentHP = m_maxHP;
        SpawnPoint = transform.position;
    }

    void KillMe()
    {
        dead = true;
        GetComponent<scr_ThrowHook>().UnHook();
        m_Rigidbody2D.isKinematic = true;
        m_Rigidbody2D.velocity = new Vector2 (0, 0);
        transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("Backpack").GetComponent<SpriteRenderer>().enabled = false;
        Invoke("SpawnMe", 3f);
    }

    void SpawnMe()
    {
        m_currentHP = m_maxHP;
        HPbar.GetComponent<SpriteRenderer>().sprite = healthbar[Mathf.RoundToInt(m_currentHP)];
        transform.position = SpawnPoint;
        dead = false;
        m_Rigidbody2D.isKinematic = false;
        transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = true;
        transform.Find("Backpack").GetComponent<SpriteRenderer>().enabled = true;
    }

    public void NextLevel()
    {
        endScreen.SetActive(true);
        dead = true;
        GetComponent<scr_ThrowHook>().UnHook();
        m_Rigidbody2D.isKinematic = true;
        m_Rigidbody2D.velocity = new Vector2(0, 0);
        transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("Backpack").GetComponent<SpriteRenderer>().enabled = false;

    }
}
