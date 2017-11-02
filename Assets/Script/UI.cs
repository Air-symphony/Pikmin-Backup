using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    private InputAccess input;
    public Text text_follow;
    public Text text_field;
    public Text text_total;
    public Text text_InOnion;
    public Text text_Outside;
    public Text text_Move;
    public Text text_time;
    private GameObject follow;
    private GameObject indepedent;
    public GameObject onion_menu;
    private Onion onion;
    private GameObject menu;
    public bool open;
    private int count;//フィールド内の数
    private int move;//移動させる数
    public GameObject cameramode;
    private int mode = 0;
    private float t = 0;
    public GameObject finish;
    private GameObject finishwindow;
    private float playtime;
    private bool finishbool;
    int min = 0;

    // Use this for initialization
    void Start()
    {
        input = new InputAccess();
        playtime = 0;
        follow = GameObject.Find("Follow");
        indepedent = GameObject.Find("Indepedent");
        menu = Instantiate(onion_menu, transform.position, transform.rotation) as GameObject;
        menu.transform.SetParent(GameObject.Find("Canvas").transform);
        menu.transform.localScale = new Vector3(1, 1, 1);
        finishwindow = Instantiate(finish, transform.position, transform.rotation) as GameObject;
        finishwindow.transform.SetParent(GameObject.Find("Canvas").transform);
        finishwindow.transform.localScale = new Vector3(1, 1, 1);
        finishwindow.SetActive(false);
        //onion_menu = GameObject.Find("Onion_menu");
        onion = GameObject.Find("Onion").GetComponent<Onion>();
        open = false;
        count = 0;
        move = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            Time.timeScale = 0.0f;
            menu.SetActive(true);
            Control();
        }
        else if (! finishbool)
        {
            Time.timeScale = 1.0f;
            text_follow.text = follow.transform.childCount + "";
            count = follow.transform.childCount + indepedent.transform.childCount;
            text_field.text = count + "";
            text_total.text = count + onion.GetKeepPikmin() + "";
            if (playtime / 60 >= 1)
            {
                min++;
                playtime = playtime % 60;
            }
            text_time.text = min + "m" + Mathf.FloorToInt(playtime) + "s";
            move = 0;
            menu.SetActive(false);
            DeleteMode();
        }
        if (!finishbool)
        {
            playtime += Time.deltaTime;
        }
    }

    private void Control()
    {
        input.InputOnionKey();
        if (input.decide)//決定ボタン
        {
            if (move >= 0)
            {
                onion.Add(move);
            }
            else
            {
                onion.Take(-move);
            }
            Debug.Log("Close");
            open = false;
        }
        if (input.down)//排出
        {
            if (move < onion.GetKeepPikmin() && move + count < onion.GetMax())
            {
                move++;
            }
            else
            {
                StartCoroutine("errorMessage");
            }
        }
        else if (input.up)//帰宅
        {
            if (-follow.transform.childCount < move)
            {
                move--;
            }
            else
            {
                StartCoroutine("errorMessage");
            }
        }
        menu.transform.Find("Text_onion").GetComponent<Text>().text = (onion.GetKeepPikmin() - move) + "";//中身
        menu.transform.Find("Text_move").GetComponent<Text>().text = move + "";//排出数
        menu.transform.Find("Text_out").GetComponent<Text>().text = (count + move) + "";//外の合計
    }

    public void ChangeMode()//cameramode
    {
        string[] text = { "Self", "Auto" };
        Color[] color = { new Color(255, 0, 0), new Color(0, 0, 255) };
        cameramode.SetActive(true);
        mode++;
        cameramode.transform.GetChild(0).transform.GetComponent<Text>().text = text[mode % text.Length];
        cameramode.transform.GetChild(1).transform.GetComponent<Image>().color = color[mode % color.Length];
        t = 1.5f;
    }

    private void DeleteMode()
    {
        if (t > 0)
        {
           t -= Time.deltaTime;
        }
        else
        {
            cameramode.SetActive(false);
        }
    }

    public void FinishWindow()
    {
        finishbool = true;
        Time.timeScale = 0.0f;
        finishwindow.SetActive(true);
        finishwindow.transform.GetChild(2).GetComponent<Text>().text
            = "Score : " + follow.transform.childCount;
        if (playtime / 60 >= 1)
        {
            min++;
            playtime = playtime % 60;
        }
        finishwindow.transform.GetChild(3).GetComponent<Text>().text = 
            "Time : " + min + "m" + Mathf.FloorToInt(playtime) + "s";
    }

    public void ChangeScene()
    {
        input.InputStartKey();
        if (input.decide)
        {
            //Debug.Log("Finish");
            SceneManager.LoadScene("Title");
        }
    }

    public bool Getfinishbool()
    {
        return finishbool;
    }

    IEnumerable errorMessage()
    {
        //エラーメッセージ
        Debug.Log("無効な処理");
        yield return new WaitForSeconds(1.0f);
    }
}
