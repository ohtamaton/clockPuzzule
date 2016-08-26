﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GameMain : MonoBehaviour {

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

    private int nodeCount;

    private Image one;

    [SerializeField]
    private GameObject clock;

    private int r = 190;
    private const int maxNode = 17;

    GameObject[] nodes = new GameObject[maxNode];
    int[] order = new int[maxNode];
    int[] nodeNums = new int[maxNode];

    // Use this for initialization
    void Start () {
        giveupButton.SetActive(false);
        backButton.SetActive(false);
        endButton.SetActive(false);
        for (int i = 0; i < nodeCount; i++)
        {
            nodeNums[i] = 0;
            order[i] = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {

        //マウスがクリックされたときの処理
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //クリックされたcollider
            Collider2D collider = Physics2D.OverlapPoint(tapPoint);
            if (collider)
            {
                Image target = collider.transform.gameObject.GetComponent<Image>();

                for (int i = 0; i < nodeCount; i++)
                {
                    nodes[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);

                    if (nodes[i].GetComponent<Image>().Equals(target))
                    {
                        Debug.Log(nodes[i].name + i);
                    }
                }
                collider.transform.gameObject.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
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
            if( num > Mathf.Floor(nodeCount / 2))
            {
                num = nodeCount - num;
            }
            setNode(index, num);
            index = nextIndex;
        }

        //最後のノードの数字をランダムに決定
        num = Random.Range(1, (int)Mathf.Floor(nodeCount / 2) + 1);
        setNode(index, num);
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
            if (nextIndex < 0)
            {
                nextIndex += nodeCount;
            }
            else if (nextIndex >= nodeCount)
            {
                nextIndex -= nodeCount;
            }
        }
        return nextIndex;
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
        }

    }

}
