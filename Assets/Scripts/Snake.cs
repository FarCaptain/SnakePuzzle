using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    [SerializeField] private List<SnakeBody> m_Bodies;
    [SerializeField] private CameraShake m_CamShake;
    [SerializeField] private GameObject m_EndScreen;
    [SerializeField] private Text m_EndText; 
    [SerializeField] private Text m_ScoreText;

    [SerializeField] private Vector2 m_FacingDirection = Vector2.up;

    // these should be a seperate
    [SerializeField] private ActionRecorder actionRecorder;
    [SerializeField] private ItemSpawner spawner;

    private bool m_isMoving = true;
    private Vector2Int prevTailCoord;

    private void Start()
    {
        //m_Bodies[0].m_CoordProperty = new Vector2Int(0, 0);
        prevTailCoord = GetTail();
    }

    public Vector2Int GetHead()
    {
        return m_Bodies[0].m_CoordProperty;
    }

    public Vector2Int GetTail()
    {
        return m_Bodies[m_Bodies.Count - 1].m_CoordProperty;
    }

    public int GetLength()
    {
        return m_Bodies.Count;
    }

    public void PauseMoving()
    {
        m_isMoving = false;
    }

    public Vector2 GetDirection()
    {
        return m_FacingDirection;
    }

    public Vector2Int GetPreTail()
    {
        return prevTailCoord;
    }

    public void SetDirection(Vector2 preDirection)
    {
        m_FacingDirection = preDirection;
    }

    public void ResumeMoving()
    {
        m_isMoving = true;
    }

    public void Move(Vector2Int direction)
    {
        if (!m_isMoving)
            return;

        Vector2Int headNext= m_Bodies[0].m_CoordProperty + direction;
        if (CheckAbyss(headNext))
            return;

        prevTailCoord = GetTail();

        for (int i = m_Bodies.Count - 1; i > 0; i--)
        {
            m_Bodies[i].m_CoordProperty = m_Bodies[i - 1].m_CoordProperty;

            bool bumpIntoBody = m_Bodies[i].m_CoordProperty == headNext;
            if(bumpIntoBody)
            {
                Debug.Log("GameOver! Bump Into Body!");
                Die("GameOver! Bump Into Body!");
            }
        }

        // headMoveForward
        m_Bodies[0].m_CoordProperty = headNext;

        CheckItems(headNext);

        CheckWater();

        m_FacingDirection = (Vector2)direction;
    }

    public void Retreat(Vector2Int tail)
    {
        for (int i = 0; i < m_Bodies.Count - 1; i++)
        {
            m_Bodies[i].m_CoordProperty = m_Bodies[i + 1].m_CoordProperty;
        }
        m_Bodies[m_Bodies.Count - 1].m_CoordProperty = tail;
    }

    private void CheckWater()
    {
        foreach (var body in m_Bodies)
        {
            Item item = MapLocator.instance.FindItem(body.m_CoordProperty);
            if(item == null || item.m_Type != CollectableType.WATER)
            {
                return;
            }
        }

        Die("Game Over! Sinked into Water!");
    }

    private bool CheckAbyss(Vector2Int coord)
    {
        Item item = MapLocator.instance.FindItem(coord);
        if (item == null)
            return false;

        if(item.m_Type == CollectableType.ABYSS)
        {
            Debug.Log("GameOver! Bumped into Abyss!");
            Die("GameOver! Bumped into Abyss!");
            return true;
        }
        return false;
    }

    public void Expand()
    {
        SnakeBody head = m_Bodies[0];
        SnakeBody tail = Instantiate(head, head.transform.parent);
        // overlap the last two bodies
        tail.m_CoordProperty = m_Bodies[m_Bodies.Count - 1].m_CoordProperty;
        m_Bodies.Add(tail);
    }

    public void Expand(Vector2Int coord)
    {
        Expand();
        m_Bodies[m_Bodies.Count - 1].m_CoordProperty = coord;
    }

    public void Shrink()
    {
        SnakeBody tailBody = m_Bodies[m_Bodies.Count - 1];
        m_Bodies.RemoveAt(m_Bodies.Count - 1);
        Destroy(tailBody.gameObject);
    }

    private void CheckItems(Vector2Int headCoord)
    {
        Item item = MapLocator.instance.FindItem(headCoord);
        if (item == null)
            return;

        switch (item.m_Type)
        {
            case CollectableType.EXPAND:
                var exaction = new ExpandAction(this, spawner);
                actionRecorder.Record(exaction);

                AudioManager.instance.Play("FruitCollect");
                break;
            case CollectableType.SHRINK:
                AudioManager.instance.Play("BadFruitCollect");
                bool onlyOneBodyLeft = m_Bodies.Count < 2;
                if (onlyOneBodyLeft)
                {
                    //Debug.Log("GameOver! Nothing Left!");
                    Die("You Win! You've had nothing left!");
                }
                else
                {
                    var shraction = new ShrinkAction(this, spawner);
                    actionRecorder.Record(shraction);
                }
                break;
            case CollectableType.WATER:
                return;
        }
        MapLocator.instance.RemoveItem(item.m_CoordProperty);
        item.gameObject.SetActive(false);
    }

    private void Update()
    {
        bool arrowKeyButtonDown = Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical");
        if (arrowKeyButtonDown)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            int horizontal = Input.GetAxis("Horizontal") == 0.0f ? 0 : (int)Mathf.Sign(h) * 1;
            int vertical = 0;
            if(horizontal == 0)
                vertical = Input.GetAxis("Vertical") == 0.0f ? 0 : (int)Mathf.Sign(v) * 1;

            Vector2Int inputdir = new Vector2Int(horizontal, vertical);

            if (inputdir == -m_FacingDirection)
                return;

            var action = new MoveAction(this, spawner, inputdir);
            actionRecorder.Record(action);
            actionRecorder.clearUndone();
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            actionRecorder.Rewind();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            actionRecorder.Redo();
        }
    }

    public void Die(string alarm)
    {
        AudioManager.instance.Play("Die");
        PauseMoving();
        StartCoroutine(m_CamShake.Shake(.15f, .2f));

        m_ScoreText.text = "Length:" + m_Bodies.Count;

        m_EndText.text = alarm;
        m_EndScreen.SetActive(true);
    }
}
