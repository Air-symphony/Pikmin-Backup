using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Push : MonoBehaviour {
    private bool obstacle = true;
    private GameObject text;
    private Vector3 endposition;
    private int needPik = 5;
    private int nowPik;
    private GameObject cam;
    private float t = 0.0f;
    private float move_t = 5.0f;//移動時間
    private bool move;


    // Use this for initialization
    void Start () {
        cam = GameObject.Find("Camera");
        text = this.gameObject.transform.GetChild(0).gameObject;
        endposition = transform.position + transform.TransformDirection(new Vector3(0, 0, 10));
        move = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (nowPik >= 1)
        {
            text.SetActive(true);
            GetComponentInChildren<TextMesh>().text = nowPik + "/" + needPik;
            GetComponentInChildren<TextMesh>().transform.rotation = cam.transform.rotation;
        }
        else
        {
            text.SetActive(false);
        }
        if (!obstacle || move)
        {
            Vector3 dir = (endposition - transform.position) / move_t;
            t += Time.deltaTime;
            if (t >= move_t)
            {
                transform.position = endposition;
                move = false;
                t = 0.0f;
            }
            transform.position = transform.position + dir * t;
        }
    }

    public void SetPushType(Vector3 start,Vector3 end,int need)
    {
        transform.position = start;
        endposition = end;
        needPik = need;
    }

    public bool Check()
    {
        return needPik <= nowPik;
    }

    public void AddNowPik()
    {
        nowPik++;
    }

    public void RemoveNowPik()
    {
        nowPik--;
    }

    public void Finish()
    {
        //transform.position = endposition;
        nowPik = 0;
        obstacle = false;
        transform.tag = "Field";
        //transform.SetParent(GameObject.Find("Field").transform);//ただの置物として扱う
        move = true;
        //Destroy(this);
    }
}
