using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Net.Sockets;
using System.Transactions;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Node nextNode;
    float walkingSpeed = 20f;
    public bool isWaitingForPath = false;
    Vector2[] dirToLeft = { Vector2.down, Vector2.up, Vector2.left, Vector2.right, Vector2.down + Vector2.left, Vector2.up + Vector2.left, Vector2.down + Vector2.right, Vector2.up + Vector2.right };
    Vector2[] dirToRight = { Vector2.up, Vector2.down, Vector2.right, Vector2.left, Vector2.up + Vector2.left, Vector2.up + Vector2.right, Vector2.down + Vector2.left, Vector2.down + Vector2.right };
    float[] angle = { 270f, 90f, 180f, 0f, 225f, 135f, 315f, 45f };
    public Vector2 offset = Vector2.zero;
    float offsetMultiplier = 0f;
    [SerializeField]
    GameObject rotateVisionAround;
    float timer;
    bool isStayingPut = false;
    Collision2D ai_collision = null;
    List<Collider2D> inVision = new List<Collider2D>();

    Vector2 bankPos = new Vector2(9f, 83f);
    List<Vector2> goals = new List<Vector2> { new Vector2(2f, 8f), new Vector2(5f, 8f), new Vector2(2f, -4f) };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextNode == null && !isWaitingForPath)
        {
            float x = UnityEngine.Random.Range(46, 123);
            float y = UnityEngine.Random.Range(34, 66);
            if (PathingController.Instance.FindPath(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), goals[0], this))
                isWaitingForPath = true;
        }
        if (nextNode == null)
        {
            return;
        }
        float speed = walkingSpeed;
        isWaitingForPath = false;
        

        Vector2 dir = (nextNode.Position - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(dir.x, dir.y) * 180 / Mathf.PI;
        rotateVisionAround.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

        if(isStayingPut)
        {
            timer += Time.deltaTime;
            if(timer >= 0.5)
            {
                isStayingPut = false;
                ai_collision = null;
                timer = 0f;
            }
            return;
        }

        GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(dir.x, dir.y, 0f) * speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, nextNode.Position) < 0.2f)
        {
            nextNode = nextNode.Child;
            offset = Vector2.zero;
            timer = 0f;
            GetComponent<CapsuleCollider2D>().enabled = true;
        }
        else
            timer += Time.deltaTime;

        if (timer >= 1f)
        {
            Node current = nextNode;
            while (current.Child != null)
                current = current.Child;
            while(!PathingController.Instance.FindPath(nextNode.Position, current.Position, this))
            { }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        /*if (collision.collider.CompareTag(tag))
        {
            isCollidingWithAI = true;
            if (offset == Vector2.zero)
                OnCollisionEnter2D(collision);
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


       if (!collision.collider.CompareTag(tag))
            return;
        if (nextNode == null || nextNode.Parent == null)
            return;
        
        Vector2 left = Vector2.zero;
        Vector2 right = Vector2.zero;
        Vector2 dir = nextNode.Position - nextNode.Parent.Position;
        for (int i = 0; i < PathingController.Instance.neighbours.Length; i++)
        {
            if(dir == PathingController.Instance.neighbours[i])
            {
                left = dirToLeft[i];
                right = dirToRight[i];
            }    
        }
        if (isStayingPut && ai_collision != null && collision.collider == ai_collision.collider)
            return;
        else if (PathingController.Instance.IsClear(nextNode.Position + left))
        {
            nextNode.Position += left;
            collision.collider.GetComponent<AI>().TellToStay(collision);
        }
        else if (PathingController.Instance.IsClear(nextNode.Position + right))
        {
            nextNode.Position += right;
            collision.collider.GetComponent<AI>().TellToStay(collision);
        }
        /*
        if(inVision.Count > 0)
            offsetMultiplier += Time.deltaTime;
        else if (inVision.Count <= 0)
            offsetMultiplier -= Time.deltaTime;
        if (offsetMultiplier <= -1f)
            offsetMultiplier = 0f;

        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + left/2, dir, 1f);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + right/2, dir, 1f);
        dir = Vector2.zero;
        if (hitLeft.collider != null && hitLeft.collider.CompareTag(tag))
            dir += left;
        if (hitRight.collider != null && hitRight.collider.CompareTag(tag))
            dir += right;

        if (dir == Vector2.zero && hitLeft.collider != null)
        {
            hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + left / 2, dir + left.normalized, 1f);
            hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + right / 2, dir + right.normalized, 1f);
            if (hitLeft.collider != null && hitLeft.collider.CompareTag(tag))
                dir += left;
            if (hitRight.collider != null && hitRight.collider.CompareTag(tag))
                dir += right;

            if (dir == Vector2.zero)
            {
                // Both hit what do we do in this case?
                dir = left;
            }
        }
        else
            dir = left;

        offset = dir;
        
        collision.collider.GetComponent<AI>().offset = offset * -1;
        isCollidingWithAI = true;*/
    }

    public void TellToStay(Collision2D col)
    {
        isStayingPut = true;
        ai_collision = col;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        /*if (!collision.collider.CompareTag(tag))
            return;

        isCollidingWithAI = false;*/
    }

    public void EnteredVision(Collider2D col)
    {
        /*
        if (col.CompareTag(tag))
        {
            // Can see AI
            inVision.Add(col);
            Vector2 colDir = col.GetComponent<AI>().nextNode.Position - col.GetComponent<AI>().nextNode.Parent.Position;
            Vector2 myDir = nextNode.Position - nextNode.Parent.Position;
            Vector2 myLeft = Vector2.zero;
            for (int i = 0; i < PathingController.Instance.neighbours.Length; i++)
            {
                if (myDir == PathingController.Instance.neighbours[i])
                {
                    myLeft = dirToLeft[i];
                }
            }

            offset = myLeft;
            if(myDir - colDir == Vector2.zero)
            {
                col.GetComponent<AI>().offset = offset * -1;
            }
        }*/
    }

    public void InVision(Collider2D col)
    {/*
        if (col.CompareTag(tag))
        {
            offsetMultiplier += Time.deltaTime;
            if (offsetMultiplier >= 1f)
                offsetMultiplier = 1f;
        }*/
    }

    public void ExitVision(Collider2D col)
    {/*
        if (col.CompareTag(tag))
        {
            inVision.Remove(col);
        }*/
    }
}
