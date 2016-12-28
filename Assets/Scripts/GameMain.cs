﻿/**
 * GameMain.cs
 * 
 * 時計盤パズルのゲームメイン処理クラス. 
 *
 * @author ys.ohta
 * @version 1.0
 * @date 2016/08/08
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * GameMain.cs
 */
public class GameMain : MonoBehaviour
{
    enum State
    {
        ST_START, ST_IDLE, ST_SELECT, ST_CLEAR, ST_OVER
    }

    //Game State
    State state = State.ST_START;

    //GUI parts
    [SerializeField]
    private GameObject giveupButton;
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private Dropdown numberSelect;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject endButton;

    //GUI Labels
    [SerializeField]
    private GameObject gameClear;
    [SerializeField]
    private GameObject gameOver;

    private int nodeCount;

    [SerializeField]
    private GameObject clock;

    private int r = 190;
    private const int maxNode = 17;

    GameObject[] nodes = new GameObject[maxNode];
    int[] order = new int[maxNode];
    int[] nodeNums = new int[maxNode];
    bool[] select = new bool[maxNode];

    // Use this for initialization
    void Start()
    {
        giveupButton.SetActive(false);
        backButton.SetActive(false);
        endButton.SetActive(false);
        gameClear.SetActive(false);
        gameOver.SetActive(false);

        for (int i = 0; i < maxNode; i++)
        {
            nodeNums[i] = 0;
            order[i] = 0;
            select[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {


        switch (state)
        {
            case State.ST_IDLE:
                break;
            case State.ST_START:
                break;
            case State.ST_CLEAR:
                gameClear.SetActive(true);
                break;
            case State.ST_OVER:
                gameOver.SetActive(true);
                break;
            case State.ST_SELECT:
                MouseEvent();
                
                break;
        }

    }

    //マウスがクリックされたときの処理
    private void MouseEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //クリックされたcollider
            Collider2D collider = Physics2D.OverlapPoint(tapPoint);
            if (collider)
            {
                Image target = collider.transform.gameObject.GetComponent<Image>();
                int selectNode = 0;
                for (int i = 0; i < nodeCount; i++)
                {

                    if (select[i])
                    {
                        continue;
                    }

                    nodes[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);

                    if (nodes[i].GetComponent<Image>().Equals(target))
                    {
                        //Debug.Log(nodes[i].name + i);
                        selectNode = i;
                        select[selectNode] = true;
                    }
                }
                target.color = new Color(0.3f, 0.3f, 0.3f, 1);

                int nextNode = selectNode + nodeNums[selectNode];
                nextNode = calcIndex(nextNode);
                bool over = true;
                if (!select[nextNode])
                {
                    nodes[nextNode].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    over = false;
                }

                nextNode = selectNode - nodeNums[selectNode];
                nextNode = calcIndex(nextNode);
                if (!select[nextNode])
                {
                    nodes[nextNode].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    over = false;
                }

                if(over)
                {
                    state = State.ST_OVER;
                }
                bool result = true;
                for (int i = 0; i < nodeCount; i++)
                {
                    result &= select[i];
                    if(over)
                    {
                        Debug.Log(i+""+select[i]);
                    }
                }
                if (result)
                {
                    state = State.ST_CLEAR;
                }
            }
        }
    }

    //ゲームの初期化処理
    public void gameStart()
    {
        startButton.SetActive(false);
        numberSelect.gameObject.SetActive(false);
        endButton.SetActive(true);
        giveupButton.SetActive(true);
        backButton.SetActive(true);
        nodeCount = int.Parse(numberSelect.GetComponentInChildren<Text>().text);

        //初期ノードの決定
        int index = Random.Range(0, nodeCount);
        order[index] = 1;
        int num = 0;

        //次ノード以降の決定処理
        for (int i = 2; i <= nodeCount; i++)
        {
            int nextIndex = getNext();
            order[nextIndex] = i;
            num = Mathf.Abs(nextIndex - index);
            if (num > Mathf.Floor(nodeCount / 2))
            {
                num = nodeCount - num;
            }
            setNode(index, num);
            index = nextIndex;
        }

        //最後のノードの数字をランダムに決定
        num = Random.Range(1, (int)Mathf.Floor(nodeCount / 2) + 1);
        setNode(index, num);

        state = State.ST_SELECT;
    }

    private int getNext()
    {
        int nextIndex = Random.Range(0, nodeCount);

        if (order[nextIndex] == 0)
        {
            return nextIndex;
        }

        int plmi = Random.Range(0, 2);
        if (plmi == 0)
        {
            plmi = -1;
        }

        while (order[nextIndex] != 0)
        {
            nextIndex += plmi;
            nextIndex = calcIndex(nextIndex);
        }
        return nextIndex;
    }

    private int calcIndex(int index)
    {
        if (index < 0)
        {
            index += nodeCount;
        }

        if (index >= nodeCount)
        {
            index -= nodeCount;
        }

        return index;
    }

    private void setNode(int index, int num)
    {
        nodeNums[index] = num;
        GameObject load = (GameObject)Resources.Load("Prefabs/" + num);
        nodes[index] = (GameObject)Instantiate(load, Vector3.zero, Quaternion.identity);
        nodes[index].transform.SetParent(clock.transform);
        nodes[index].transform.localScale = Vector3.one;
        nodes[index].transform.localPosition = new Vector3(r * Mathf.Sin(Mathf.PI * 2 * index / nodeCount), r * Mathf.Cos(Mathf.PI * 2 * index / nodeCount), 0);
    }

    public void gameReset()
    {
        giveupButton.SetActive(false);
        backButton.SetActive(false);
        endButton.SetActive(false);
        gameClear.SetActive(false);
        gameOver.SetActive(false);
        startButton.SetActive(true);
        numberSelect.gameObject.SetActive(true);

        for (int i = 0; i < nodeCount; i++)
        {
            Destroy(nodes[i]);
        }
        //ノードの数字とハミルトンパスの順序初期化
        for (int i = 0; i < nodeCount; i++)
        {
            nodeNums[i] = 0;
            order[i] = 0;
            select[i] = false;
        }
        state = State.ST_START;
    }
}
