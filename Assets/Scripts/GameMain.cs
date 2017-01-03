/**
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
//===========================================================
// 変数宣言
//===========================================================

    //---------------------------------------------------
    // public
    //---------------------------------------------------

    //None.

    //---------------------------------------------------
    // private
    //---------------------------------------------------

    //GUI parts
    [SerializeField]
    private Dropdown numberSelect;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject endButton;

    // For the future upgrade
    //[SerializeField]
    //private GameObject giveupButton;
    //[SerializeField]
    //private GameObject backButton;

    //GUI Labels
    [SerializeField]
    private GameObject labelPos;
    [SerializeField]
    private GameObject gameClear;
    [SerializeField]
    private GameObject gameOver;

    //出題される時計盤パズルのノード数の最大値
    private const int maxNode = 17;
    //選択された時計盤パズルのノード数
    private int nodeCount;
    //時計盤パズルのノードを並べる円の半径
    private int r = 190;

    //---------------------------------------------------
    // others
    //---------------------------------------------------

    //Game State用の列挙型
    enum State
    {
        ST_START,
        ST_MAIN,
        ST_CLEAR,
        ST_OVER
    }
    //Game State
    State state = State.ST_START;
    //時計盤パズルのノードのゲームオブジェクトを入れる配列
    GameObject[] nodes = new GameObject[maxNode];
    //時計盤パズルにおいて何番目に選択されたノードかを記憶する配列
    int[] order = new int[maxNode];
    //時計盤パズルの各ノードの数字を記憶する配列
    int[] nodeNums = new int[maxNode];
    //時計盤パズルの各ノードが既に選択済かどうかを記憶する配列
    bool[] select = new bool[maxNode];

//===========================================================
// 関数定義
//===========================================================

    //---------------------------------------------------
    // public
    //---------------------------------------------------

    /**
     * ゲームの初期化処理
     * @params
     * @return
     **/
    public void gameStart()
    {
        //GUIボタンの表示設定
        startButton.SetActive(false);
        numberSelect.gameObject.SetActive(false);
        endButton.SetActive(true);
        
        // For the future upgrade
        //giveupButton.SetActive(true);
        //backButton.SetActive(true);
        nodeCount = int.Parse(numberSelect.GetComponentInChildren<Text>().text);

        //
        // 時計盤パズル出題のランダム生成処理
        //

        //初期ノードの決定
        int index = Random.Range(0, nodeCount);
        order[index] = 1;
        int num = 0;

        //次ノード以降の決定処理
        for (int i = 2; i <= nodeCount; i++)
        {
            //現在の位置から移動可能なノードの位置の取得
            int nextIndex = getNext();
            order[nextIndex] = i;

            //現在位置から次のノードへの距離からノードの数字を計算
            num = Mathf.Abs(nextIndex - index);
            if (num > Mathf.Floor(nodeCount / 2))
            {
                num = nodeCount - num;
            }
            //現在のノードに数字を登録
            setNode(index, num);
            index = nextIndex;
        }

        //最後のノードの数字をランダムに決定
        num = Random.Range(1, (int)Mathf.Floor(nodeCount / 2) + 1);
        setNode(index, num);

        state = State.ST_MAIN;
    }

    /**
     * ゲーム状態のリセット処理
     * @param
     * @return
     **/
    public void gameReset()
    {
        //GUIボタン状態のリセット
        endButton.SetActive(false);
        gameClear.SetActive(false);
        gameOver.SetActive(false);
        startButton.SetActive(true);
        numberSelect.gameObject.SetActive(true);

        // For the future upgrade
        //giveupButton.SetActive(false);
        //backButton.SetActive(false);

        //時計盤パズルのノードのゲームオブジェクトの削除
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

    //---------------------------------------------------
    // private
    //---------------------------------------------------

    /**
     * 時計盤パズル出題のランダム生成時の次ノードの位置を決定する
     * @params
     * @return
     **/
    private int getNext()
    {
        //まずはランダムに位置を決定
        int nextIndex = Random.Range(0, nodeCount);

        //ランダムに選んだ位置がまだ未選択であればその位置を返す
        if (order[nextIndex] == 0)
        {
            return nextIndex;
        }

        //ランダムに選んだ位置が選択済であれば、その両隣のどちらかを選択
        int plmi = Random.Range(0, 2);
        if (plmi == 0)
        {
            plmi = -1;
        }

        //選択済でないノードにいくまで順にその隣を選択
        while (order[nextIndex] != 0)
        {
            nextIndex += plmi;
            nextIndex = calcIndex(nextIndex);
        }
        return nextIndex;
    }

    /**
     * 円環上のインデックス(mod numCount)を計算する
     * @params index
     * @return 円環上のインデックス
     **/
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

    /**
     * 該当indexのノードのノードオブジェクト、位置を登録
     * @params index 
     * @param num
     * @return 
     **/
    private void setNode(int index, int num)
    {
        //ノードの数字情報を記憶
        nodeNums[index] = num;
        //該当するノードのnumに対応するGUIオブジェクトをプレハブから取得
        GameObject load = (GameObject)Resources.Load("Prefabs/" + num);
        //オブジェクトを格納
        nodes[index] = (GameObject)Instantiate(load, Vector3.zero, Quaternion.identity);
        nodes[index].transform.SetParent(labelPos.transform);
        nodes[index].transform.localScale = Vector3.one;
        //indexから時計盤パズルのノードを配置するゲーム上の位置を計算
        nodes[index].transform.localPosition = new Vector3(r * Mathf.Sin(Mathf.PI * 2 * index / nodeCount), r * Mathf.Cos(Mathf.PI * 2 * index / nodeCount), 0);
    }

    //---------------------------------------------------
    // other
    //---------------------------------------------------

    /**
     * 本クラス生成時の初期化処理
     * @params
     * @retrun
     **/
    void Start()
    {
        //GUIボタンの表示状態の初期化
        endButton.SetActive(false);
        gameClear.SetActive(false);
        gameOver.SetActive(false);
        // For the future upgrade
        //giveupButton.SetActive(false);
        //backButton.SetActive(false);

        //各ゲーム状態の記憶用配列の初期化
        for (int i = 0; i < maxNode; i++)
        {
            nodeNums[i] = 0;
            order[i] = 0;
            select[i] = false;
        }
    }

    /**
     * ゲームループで毎回呼ばれる処理
     * @params
     * @retrun
     **/
    void Update()
    {
        switch (state)
        {
            case State.ST_START:
                break;
            case State.ST_CLEAR:
                gameClear.SetActive(true);
                break;
            case State.ST_OVER:
                gameOver.SetActive(true);
                break;
            case State.ST_MAIN:
                Main();
                break;
        }
    }

    /**
     * マウスクリック時のイベント処理
     * @params
     * @return
     **/
    private void Main()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(tapPoint);
            if (collider)
            {
                //マウスでクリックされたゲームオブジェクトのImageを取得
                Image target = collider.transform.gameObject.GetComponent<Image>();
                //クリックされたノードの位置の初期化
                int selectNode = 0;
                for (int i = 0; i < nodeCount; i++)
                {
                    if (select[i]) //既に選択済のノードは処理スキップ
                    {
                        continue;
                    }

                    //選択されたもの以外のノードの色を半透明に設定する
                    nodes[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);

                    //選択されたノードの位置、ノード選択状態を記憶
                    if (nodes[i].GetComponent<Image>().Equals(target))
                    {
                        //Debug.Log(nodes[i].name + i);
                        selectNode = i;
                        select[selectNode] = true;
                    }
                }
                //選択されたノードの色の明度を暗めに設定する
                target.color = new Color(0.3f, 0.3f, 0.3f, 1);

                //次に移動できるノードの位置を計算
                int nextNode = selectNode + nodeNums[selectNode];
                nextNode = calcIndex(nextNode);

                //ゲームオーバまたはクリアか、そうでないかの判定結果を入れる変数
                bool over = true;
                if (!select[nextNode])
                {
                    //次に移動できるノードがある場合は、そのノードの色の設定をデフォルトに戻す
                    nodes[nextNode].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    //次に移動できるノードがある場合は、まだゲーム終了でないと判定
                    over = false;
                }

                //次に移動できるノードの位置を計算
                nextNode = selectNode - nodeNums[selectNode];
                nextNode = calcIndex(nextNode);
                if (!select[nextNode])
                {
                    //次に移動できるノードがある場合は、そのノードの色の設定をデフォルトに戻す
                    nodes[nextNode].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    //次に移動できるノードがある場合は、まだゲーム終了でないと判定
                    over = false;
                }

                if (over)
                {
                    //ゲームが終了ならゲーム状態をOVERに設定
                    state = State.ST_OVER;
                }

                //クリアかオーバかを判定
                bool result = true;
                for (int i = 0; i < nodeCount; i++)
                {
                    //すべてのノードが選択済であればゲームクリア
                    result &= select[i];
#if DEGUB
                    if(over)
                    {
                        Debug.Log(i+""+select[i]);
                    }
#endif
                }
                if (result)
                {
                    state = State.ST_CLEAR;
                }
            }
        }
    }
}
