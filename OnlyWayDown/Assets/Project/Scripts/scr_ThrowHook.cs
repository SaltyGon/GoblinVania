using UnityEngine;
using System.Collections;

public class scr_ThrowHook : MonoBehaviour
{
    public string myPlayer;

    public GameObject Hook;
    public float distanceThreshold = 100f;

    GameObject cur_Hook;

    private Vector2 target;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(myPlayer + "Hook"))
        {
            if (cur_Hook == null)
            {
                if(myPlayer == "P2")
                {
                    target = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized * distanceThreshold;
                }
                else
                {
                    Vector2 direction = new Vector2(Input.GetAxis(myPlayer + "Horizontal"), Input.GetAxis(myPlayer + "Vertical"));
                    direction *= distanceThreshold;
                    target = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y);
                }

                if ((new Vector2(Input.GetAxis(myPlayer + "Horizontal"), Input.GetAxis(myPlayer + "Vertical")) != new Vector2(0, 0) || myPlayer == "P2") && GetComponent<scr_CubeControl>().m_CanHook == true)
                {
                    cur_Hook = (GameObject)Instantiate(Hook, transform.position, Quaternion.identity);
                    cur_Hook.GetComponent<scr_RopeScript>().r_Destiny = target;
                    GetComponent<scr_CubeControl>().m_CanHook = false;
                }
            }
            else
            {
                UnHook();
            }
        }
    }

    public void UnHook()
    {
        GetComponent<scr_CubeControl>().UnHooked();
        Destroy(cur_Hook);
    }
}