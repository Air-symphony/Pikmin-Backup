using UnityEngine;
using System.Collections;

public class Player_move : MonoBehaviour {
    private CharacterController player;
    private InputAccess input;
    public GameObject cam;
    public GameObject whistle;
    private GameObject follow;
    private GameObject indepedent;
    private UI ui;
    private GameObject pointer;
    
    private const float GRAVITY = 9.8f;
    public GameObject setpoint;//集合点
    private int distancsetpoint = 2;//z座標の値
    public float moveSpeed = 10.0f;
    private int controlPik_MAX = 5;//同時射出限界値
    private GameObject[] controlPik;
    private GameObject whi;
    private bool start_whi;
    private float whistle_t;

    private GameObject balance;

    // Use this for initialization
    void Start () {
        player = GetComponent<CharacterController>();
        follow = GameObject.Find("Follow");
        indepedent = GameObject.Find("Indepedent");
        controlPik = new GameObject[controlPik_MAX];
        ui = GameObject.Find("Canvas").GetComponent<UI>();
        pointer = GameObject.Find("Throwpoint");

        input = new InputAccess();
    }

    // Update is called once per frame
    void Update()
    {
        input.InputPlayerKey();
        if (Time.timeScale != 0.0f)
        {
            PlayerMove();
            Setpoint();
            RayCastFloor(input.decide);//パネルの確認
            //StartCoroutine("rayCastFloor");
            ThrowPikmin(input.decide);//射出
            Meeting(input.meeting);//集合
            Wait(input.wait);//待機
            //moveMeeting();//衝突時に集合
        }
        else if (!ui.open && !ui.Getfinishbool())
        {
            Time.timeScale = 1.0f;
        }
        if (ui.Getfinishbool())
        {
            ui.ChangeScene();
        }
    }
   
    private void PlayerMove()
    {
        transform.LookAt(new Vector3(pointer.transform.position.x, transform.position.y, pointer.transform.position.z));
        Vector3 move = new Vector3(0, 0, 0);
        if (Vector3.Distance(pointer.transform.position, transform.position)
            >= pointer.GetComponent<Pointer>().GetmovePointer_r())
        {
            move = transform.forward * moveSpeed;
        }
        move.y -= GRAVITY;
        player.Move(move * Time.deltaTime);
    }

    private void RayCastFloor(bool key)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,-Vector3.up, out hit))
        {
            GameObject onion = GameObject.Find("Onion");
            if (hit.collider.name == "Onion_Plate")
            {
                onion.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                if (key)
                {
                    ui.open = true;
                }
            }
            else
            {
                onion.GetComponent<Renderer>().material.color = new Color(255, 255, 255);
            }
            if (hit.collider.tag == "Finish")
            {
                //Debug.Log("Goal");
                ui.FinishWindow();
            }
        }

        RaycastHit hitObject;
        int mass = 2;
        if (balance == null)//Left かRightか
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hitObject))
            { 
                if (hitObject.collider.tag == "Obstacle")
                {
                    if (hitObject.collider.name == "Left")
                    {
                        balance = hitObject.transform.gameObject;
                        balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeLeft(mass);
                    }
                    else if (hitObject.collider.name == "Right")
                    {
                        balance = hitObject.transform.gameObject;
                        balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeRight(mass);
                    }
                }
                else
                {
                    balance = null;
                }
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hitObject))
            {
                if (hitObject.collider.tag == "Obstacle")
                {
                    if (balance.transform.parent.transform.name != hitObject.transform.parent.transform.name)
                    {
                        if (balance.transform.name == "Left")
                        {
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeLeft(-mass);
                        }
                        else if (balance.transform.name == "Right")
                        {
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeRight(-mass);
                        }
                        if (hitObject.collider.name == "Left")
                        {
                            //balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeRight(-mass);
                            balance = hitObject.transform.gameObject;
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeLeft(mass);
                        }
                        else if (hitObject.collider.name == "Right")
                        {
                            //balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeLeft(-mass);
                            balance = hitObject.transform.gameObject;
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeRight(mass);
                        }
                    }
                    else if (balance.transform.name != hitObject.transform.name)
                    {
                        if (balance.transform.name == "Left")
                        {
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeLeft(-mass);
                            balance = hitObject.transform.gameObject;
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeRight(mass);
                        }
                        else if (balance.transform.name == "Right")
                        {
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeRight(-mass);
                            balance = hitObject.transform.gameObject;
                            balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeLeft(mass);
                        }
                    }
                }
                else if (hitObject.collider.tag != "Pikmin")
                {
                    if (balance.name == "Left")
                    {
                        balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeLeft(-mass);
                    }
                    else if (balance.name == "Right")
                    {
                        balance.transform.parent.transform.GetComponent<Obstacle_Balance>().ChangeRight(-mass);
                    }
                    balance = null;
                }
            }
        }
    }

    private void ThrowPikmin(bool key)
    {
        if (key && !ui.open)
        {
            GameObject obj = Shootable();//いない場合はnull
            if (obj != null)
            {
                for (int i = 0; i < controlPik_MAX; i++)
                {
                    if (controlPik[i] == null)
                    {
                        controlPik[i] = obj;
                        //controlPik[i].GetComponent<pikmin_move>().flying = true;
                        break;
                    }
                }
            }
            else
            {
                //Debug.Log("Not having pikmins");
            }
        }
        for (int i = 0; i < controlPik_MAX; i++)
        {
            if (controlPik[i] == null)
            {
                break;
            }
            else if (!controlPik[i].GetComponent<Pikmin_move>().Shot())
            {
                controlPik[i] = null;
                CompressionPik(i);
            }
        }
    }

    private GameObject Shootable()
    {
        GameObject obj = null;      
        foreach (Transform child in follow.transform)
        {
            if (distancsetpoint + 3 > Vector3.Distance(child.transform.position, transform.position))//半径以内のpikminを探す
            {
                obj = child.gameObject;
                break;
            }
        }
        return obj;
    }

    private void CompressionPik(int i)
    {
        if (i + 1 < controlPik_MAX)
        {
            if (controlPik[i] == null && controlPik[i + 1] != null)
            {
                controlPik[i] = controlPik[i + 1];
                controlPik[i + 1] = null;
                CompressionPik(i + 1);
            }
        }
    }

    private void Meeting(bool key)
    {
        float maxr = 6.0f;
        float speed = 5.0f;
        if (key)//押した1Fのみ
        {
            start_whi = true;
        }
        if (start_whi)//1秒間の表示
        {
            if (whistle_t <= 0.0f)//初期設定
            {
                //SE start
                whi = Instantiate(whistle, pointer.transform.position, pointer.transform.rotation) as GameObject;
                whi.transform.localEulerAngles = new Vector3(90.0f, 0, 0);
                whi.transform.parent = pointer.transform;
            }
            else{
                float r = maxr * whistle_t * speed;
                if (r > maxr)
                {
                    r = maxr;
                }
                whi.transform.localScale = new Vector3(r, r, 1);
                //whi.transform.localEulerAngles = new Vector3(0, 0, 10.0f * Time.deltaTime);
                foreach (Transform child in indepedent.transform)
                {
                    float x = child.transform.position.x - pointer.transform.position.x;
                    float z = child.transform.position.z - pointer.transform.position.z;
                    if (2 * r * r > x * x + z * z)//半径以内のpikminを回収
                    {
                        if (!child.GetComponent<Pikmin_move>().GetFlying())
                        {
                            child.GetComponent<Pikmin_move>().status = true;
                        }
                    }
                }
            }
            whistle_t += Time.deltaTime;
        }
        if (whistle_t > 1.0f || !key)//1秒越え、もしくはボタンを離した場合
        {
            //SE finish
            start_whi = false;
            Destroy(whi);
            whistle_t = 0.0f;
        }
    }

    /*private void moveMeeting()
    {
        float r = 0.8f;
        foreach (Transform child in indepedent.transform)
        {
            if (!child.GetComponent<pikmin_move>().GetFlying())
            {
                if (r > Vector3.Distance(child.transform.position, transform.position))//半径以内のpikminを探す
                {
                    child.GetComponent<pikmin_move>().status = true;
                }
            }
        }
    }*/

    private void Wait(bool key)
    {
        if (key)
        {
            foreach (Transform child in follow.transform)
            {
                child.GetComponent<Pikmin_move>().status = false;   
            }
        }
    }

    private void Setpoint()
    {
        distancsetpoint = follow.transform.childCount / 30 + 1;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Horizontal_R") == 0 && Input.GetAxis("Vertical_R") == 0)
            setpoint.transform.localPosition = new Vector3(0, 0, -distancsetpoint);
        }
        float max_r = 10.0f;
        Vector3 move = new Vector3(Input.GetAxis("Horizontal_R"), 0, -Input.GetAxis("Vertical_R"));
        move = cam.transform.TransformDirection(move) * Time.deltaTime * 5.0f;
        if (Vector3.Distance(setpoint.transform.position + move, transform.position) < max_r)
        {
            setpoint.transform.position += move;
        }
    }
}