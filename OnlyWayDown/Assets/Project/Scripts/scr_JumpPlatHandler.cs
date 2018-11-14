using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scr_JumpPlatHandler : MonoBehaviour
{

    public float BounceStrength = 1200f;
    public Vector2 BounceDir;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            BounceDir = -collision.contacts[0].normal;
            collision.gameObject.GetComponent<scr_CubeControl>().GetPushed(BounceDir, BounceStrength);
        }
    }
}
