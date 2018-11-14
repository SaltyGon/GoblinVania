using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_LavaPlatHandler : MonoBehaviour {

    [SerializeField] private float m_DangerTime = 3f;

    private bool m_Toggled = false;

    private ParticleSystem part_Idle;
    private ParticleSystem part_Danger;
    private ParticleSystem part_Destroy;
    private SpriteRenderer spr_Sprite;

    public AudioSource a_Bye;

    private void Awake()
    {
        part_Idle = transform.Find("IdleParticle").GetComponent<ParticleSystem>();
        part_Danger = transform.Find("DangerParticle").GetComponent<ParticleSystem>();
        part_Destroy = transform.Find("DestroyParticle").GetComponent<ParticleSystem>();
        spr_Sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (part_Danger.gameObject.activeSelf == false)
                part_Danger.gameObject.SetActive(true);
            if (!m_Toggled)
            {
                m_Toggled = true;
                Invoke("PlatDeath", m_DangerTime);
            }
        }
    }

    void PlatDeath()
    {
        a_Bye.Play();
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        part_Idle.gameObject.SetActive(false);
        part_Danger.gameObject.SetActive(false);
        spr_Sprite.GetComponent<SpriteRenderer>().enabled = false;
        part_Destroy.gameObject.SetActive(true);
        Invoke("SpawnMe", 4f);
    }

    void SpawnMe()
    {
        this.gameObject.GetComponent<Collider2D>().enabled = true;
        part_Idle.gameObject.SetActive(true);
        spr_Sprite.GetComponent<SpriteRenderer>().enabled = true;
        part_Destroy.gameObject.SetActive(true);
        m_Toggled = false;
    }
}
