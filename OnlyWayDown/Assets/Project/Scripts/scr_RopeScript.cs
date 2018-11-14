using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scr_RopeScript : MonoBehaviour
{

    public Vector2 r_Destiny;

    public float r_Speed = 1f;
    public float r_Distance = 2f;
    public float r_MaxLength = 7f;
    private float r_CurLength = 0f;
    public bool r_isHooked = false;
    public Rigidbody2D r_Rigidbody2D;

    public GameObject r_NodePrefab;
    public GameObject r_Player;
    public GameObject r_LastNode;

    public LineRenderer lr;

    int vertexCount = 2;

    public List<GameObject> Nodes = new List<GameObject>();

    public bool r_Done = false;
    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();

        r_Rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        r_Player = GameObject.FindGameObjectWithTag("Player");
        r_LastNode = transform.gameObject;

        Nodes.Add(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (r_CurLength >= r_MaxLength)
        {
            r_Destiny = transform.position;

            if (!r_isHooked)
            {
                Invoke("NoHookDestroy", 0.3f);
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, r_Destiny, r_Speed);

        if ((Vector2)transform.position != r_Destiny)
        {
            if(Vector2.Distance(r_Player.transform.position, r_LastNode.transform.position) > r_Distance)
            {
                CreateNode();
            }
        }

        else if (!r_Done)
        {
            r_Done = true;

            while(Vector2.Distance(r_Player.transform.position, r_LastNode.transform.position) > r_Distance)
            {
                CreateNode();
            }

            r_LastNode.GetComponent<HingeJoint2D>().connectedBody = r_Player.GetComponent<Rigidbody2D>();
        }

        RenderLine();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag != "NoHook" && collision.gameObject.tag != "Player")
        {
            Debug.Log(collision.gameObject.name);
            r_Destiny = transform.position;
            r_Rigidbody2D.isKinematic = true;
            r_Rigidbody2D.velocity = Vector2.zero;

            r_isHooked = true;

            r_Player.GetComponent<scr_CubeControl>().Hooked();
        }
    }

    void RenderLine()
    {
        lr.positionCount = vertexCount;

        int i;

        for (i = 0; i < Nodes.Count; i++)
        {
            lr.SetPosition(i, Nodes[i].transform.position);
        }

        lr.SetPosition(i, r_Player.transform.position);
    }
    void CreateNode()
    {
        Vector2 SpawnPos = r_Player.transform.position - r_LastNode.transform.position;
        SpawnPos.Normalize();
        SpawnPos *= r_Distance;
        SpawnPos += (Vector2)r_LastNode.transform.position;

        GameObject go = (GameObject)Instantiate(r_NodePrefab, SpawnPos, Quaternion.identity);

        go.transform.SetParent(transform);

        r_LastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

        r_LastNode = go;

        Nodes.Add(r_LastNode);

        r_CurLength++;
        vertexCount++;
    }

    void NoHookDestroy()
    {
        if (!r_isHooked)
        {
            Destroy(this.gameObject);
        }
    }
}
