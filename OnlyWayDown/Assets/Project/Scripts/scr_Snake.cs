using UnityEngine;
using System.Collections;

public class scr_Snake : MonoBehaviour
{
    public float speed = 10;
    public float deathTime = 3;

    private Transform snakeCheck;
    private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatFlips;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float damageDealt = 1f;
    public float bounceStrength = 1000f;
    public float knockStrength = 1000f;
    public Vector2 BounceDir;
    private bool stunned = false;

    private void Awake()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        snakeCheck = transform.Find("SnakeCheck");
        groundCheck = transform.Find("GroundCheck");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !stunned)
        {
            stunned = true;
            collision.gameObject.GetComponent<scr_CubeControl>().JumpAsisst(0, bounceStrength);
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1);
            this.gameObject.GetComponent<Animator>().enabled = false;
            Invoke("Revive", deathTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !stunned)
        {
            collision.gameObject.GetComponent<scr_CubeControl>().DamageMe(damageDealt);
            BounceDir = -collision.contacts[0].normal;
            collision.gameObject.GetComponent<scr_CubeControl>().GetPushed(BounceDir, knockStrength);
        }
        else Flip();
    }

    private void Flip()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(snakeCheck.position, checkRadius, whatFlips);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                speed *= -1;
                transform.localScale = theScale;
            }
        }
        Collider2D[] fallColliders = Physics2D.OverlapCircleAll(groundCheck.position, checkRadius, whatIsGround);
        Debug.Log(fallColliders.Length);

        if (fallColliders.Length < 1)
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            speed *= -1;
            transform.localScale = theScale;
        }
    }

    private void MoveSnake()
    {
        this.gameObject.transform.position += new Vector3(speed, 0, 0);
    }

    private void Revive()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        this.gameObject.GetComponent<Animator>().enabled = true;
        stunned = false;
    }
}
