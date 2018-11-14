using UnityEngine;
using System.Collections;

public class scr_DamagePlayer : MonoBehaviour
{

    [SerializeField] private float damageDealt = 1f;
    public float BounceStrength = 1000f;
    public Vector2 BounceDir;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<scr_CubeControl>().DamageMe(damageDealt);
            BounceDir = -collision.contacts[0].normal;
            collision.gameObject.GetComponent<scr_CubeControl>().GetPushed(BounceDir, BounceStrength);
        }
    }
}
