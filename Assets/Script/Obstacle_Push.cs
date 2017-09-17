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
    private bool move_finish = true;

    // Use this for initialization
    void Start () {
        cam = GameObject.Find("Camera");
        text = this.gameObject.transform.GetChild(0).gameObject;
        endposition = transform.position + transform.TransformDirection(new Vector3(0, 0, 10));
    }
	
	// Update is called once per frame
	void Update () {
        //障害物であるとき
        if (obstacle)
        {
            if (nowPik > 0)
            {
                text.SetActive(true);
                GetComponentInChildren<TextMesh>().text = nowPik + "/" + needPik;
                GetComponentInChildren<TextMesh>().transform.rotation = cam.transform.rotation;
            }
            else
            {
                text.SetActive(false);
            }
        }
        else
        {
            if (move_finish == false)
            {
                Vector3 dir = (endposition - transform.position) / move_t;
                t += Time.deltaTime;
                if (t >= move_t)
                {
                    transform.position = endposition;
                    move_finish = true;
                    t = 0.0f;
                }
                transform.position = transform.position + dir * t;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        //まだ動かしてない時
        if (obstacle) {
            GameObject pikmin = other.gameObject;
            if (pikmin.tag == "Pikmin")
            {
                if (pikmin.GetComponent<Pikmin_move>().brakeMove == false &&
                    pikmin.GetComponent<Pikmin_move>().status == false)
                {
                    other.GetComponent<Pikmin_move>().setTarget(this.gameObject);
                    nowPik++;
                }
            }
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
        text.SetActive(false);
        //transform.SetParent(GameObject.Find("Field").transform);//ただの置物として扱う
        move_finish = false;//動き始める
    }
}
