using UnityEngine;
using System.Collections;

public class Pikmin_move : MonoBehaviour {
    private GameObject player;
    private GameObject point;
    private GameObject follow;
    private GameObject indepedent;
    private float moveSpeed = 7.0f;
    public bool status;//操作可能かどうか
    //private UnityEngine.AI.NavMeshAgent agent;
    private Renderer bodyColor;
    private GameObject target;//目標物
    //---shot()-----------------------
    private const float shotSpeed = 2.0f;
    public bool flying;//飛んでいるかどうか
    private RaycastHit hitObject;//投げて当たった物体
    private bool hitWall;
    private Pointer pointer;
    private const float flying_time = 2.0f;
    private float t = 0.0f;
    private Vector3 landing_point = new Vector3(0, 0, 0);//投げた時の着地点
    private Vector3 v_0 = new Vector3(0, 10.0f, 0);
    private Vector3 d_position, start_position;
    //---carryItem()-----------------------
    private bool carryMove = false;//運んでいるかどうか
    private GameObject ItemList;
    //---MoveObstacle()--------------------
   public bool brakeMove = false;//障害物を処理しているか
    //
    private GameObject balance;
    //private GameObject ObstacleList;
    //test
    private CharacterController pikmin;
    private float move_t = 0.0f;

    private float body_size;

    // Use this for initialization
    void Start () {
        pikmin = GetComponent<CharacterController>();
        body_size = GetComponent<Renderer>().bounds.size.x;

        player = GameObject.Find("Player");
        point = GameObject.Find("PlayerBack");
        follow = GameObject.Find("Follow");
        indepedent = GameObject.Find("Indepedent");
        ItemList = GameObject.Find("ItemList");
        pointer = GameObject.Find("Throwpoint").GetComponent<Pointer>();
        bodyColor = GetComponent<Renderer>();
        //agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //agent.speed = moveSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        //agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //agent.enabled = true;
        BalanceMove();
        if (status)//連れている状態
        {
            bodyColor.material.color = new Color(255, 0, 0);
            transform.SetParent(follow.transform);
            MoveContoller();

            if (carryMove)
            {
                target.GetComponent<Pellet>().RemoveCarryPik();
                carryMove = false;
            }
            else if (brakeMove && target != null)
            {
                if (target.name == "WallType")
                {
                    target.GetComponent<Obstacle_Wall>().RemoveNowPik();
                }
                else if (target.name == "PushType")
                {
                    target.GetComponent<Obstacle_Push>().RemoveNowPik();
                }
                brakeMove = false;
            }
            target = null;
            /*if (Vector3.Distance(transform.position,player.transform.position) > 40.0f)//original
            {
                status = false;
            }*/
            //agent.SetDestination(point.transform.position);
        }
        else//連れていない状態
        {
            bodyColor.material.color = new Color(0, 255, 0);//待機
            transform.SetParent(indepedent.transform);
            if (flying)
            {
                //agent.enabled = false;
            }
            else
            {
                pikmin.Move(Gravity(new Vector3(0, 0, 0)));
                if (carryMove)//運んでいる状態
                {
                    bodyColor.material.color = new Color(0, 0, 255);//緑
                    CarryItem();
                }
                else if (brakeMove)//障害物処理
                {
                    bodyColor.material.color = new Color(255, 255, 0);//黄
                    MoveObstacle();
                }
                else//待機中
                {
                    HitMeeting();//StartCoroutine("hitMeeting");
                    TargetItem();
                }
            }
        }
    }
    
    private void MoveContoller()
    {
        Vector3 move = new Vector3(0, 0, 0);
        if (Vector3.Distance(point.transform.position, transform.position) > 0.5f)
        {
            transform.LookAt(new Vector3(point.transform.position.x, transform.position.y, point.transform.position.z));
            move += transform.forward * moveSpeed * Time.deltaTime;
            //transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        pikmin.Move(Gravity(move));
        if (transform.position.y <= -2.0f)
        {
            Debug.Log(transform.name + " is dead.");
            Destroy(this.gameObject);
        }
    }

    private Vector3 Gravity(Vector3 move)
    {
        //Vector3 move = new Vector3(0, 0, 0);
        RaycastHit hit;
        Vector3 down = transform.InverseTransformDirection(new Vector3(0, -0.5f, 0));
        Debug.DrawRay(transform.position, down);
        if (Physics.Raycast(transform.position, down, out hit, down.magnitude))
        {
            if (hit.collider.tag == "Field" || hit.collider.tag == "Obstacle")
            {
                //hit.point;
                move_t = 0.0f;
            }
        }
        move_t += Time.deltaTime;
        return move -= new Vector3(0, 0.5f * 9.8f * move_t * move_t, 0);
    }

    public bool Shot()
    {
        //初期設定
        if (!flying)
        {
            status = false;
            hitWall = false;
            transform.rotation = player.transform.rotation;
            transform.position = player.transform.position + player.transform.TransformDirection(new Vector3(0, 0, -1));
            start_position = d_position = transform.position;
            //Z軸の初速度の設定、ワールド座標からローカルへ変更
            v_0.z = transform.InverseTransformDirection(pointer.transform.position - transform.position).z / flying_time;
            Debug.Log("v_0.z = " + v_0.z);
        }
        
        //初期座標から変化した座標 = 射出初期座標 + 移動先のベクトル
        t += shotSpeed * Time.deltaTime;
        float y = v_0.y * t - 0.5f * 9.8f * t * t;
        Vector3 deltaPos = start_position + transform.TransformDirection(new Vector3(0, y, v_0.z * t));//初期座標から変化した座標

        //(42.4, 0.1, 0.0) => (53.6, -2.4, 0.0) 不正な値
        Debug.Log(gameObject.name + "," + transform.position + "=>" + deltaPos);
        Debug.Log(new Vector3(0, y, v_0.z * t) + " => " + transform.TransformDirection(new Vector3(0, y, v_0.z * t)));
        //床があるかどうか、あれば終了
        Vector3 down = transform.InverseTransformDirection(new Vector3(0, -0.5f, 0));
        RaycastHit[] hits = Physics.RaycastAll(d_position, down, down.magnitude);
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            if (hitObject.collider.tag == "Field" || hitObject.collider.tag == "Obstacle")
            {
                Debug.Log("着地");
                transform.position = hitObject.point + transform.TransformDirection(new Vector3(0, 1.0f * transform.localScale.y, 0));
                flying = false;
                return false;
            }
        }
        //壁にぶつかったとき、当たった先端からfor
        Debug.DrawRay(d_position, (deltaPos - d_position), Color.green, 1.0f);//Rayの描画
        if (hitWall == false)
        {
            hits = Physics.RaycastAll(d_position, deltaPos - d_position, Vector3.Distance(d_position, deltaPos));
            for (int i = hits.Length - 1; i >= 0; i--)
            {
                Debug.Log("hits[" + i + "]:" + hits[i].collider.name);
            }
            for (int i = hits.Length - 1; i >= 0; i--)
            {
                //壁にぶつかった場合
                if (hits[i].collider.tag == "Field" || hits[i].collider.tag == "Obstacle")//床と障害物の判定
                {
                    //当たった場所から少し手前に移動
                    transform.position = hits[i].point + transform.TransformDirection(new Vector3(0, 0, -1.0f * transform.localScale.z));
                    Debug.Log(hits[i].collider.name);
                    Debug.Log(this.gameObject.name + "壁ヒット = " + transform.position);

                    v_0.z = 0.0f;
                    flying = hitWall = true;
                    return true;
                }
            }
        }
        //想定外の場合,、不時着
        if (transform.position.y < -2.0f)
        {
            t = 0.0f;
            transform.position = deltaPos + new Vector3(0, 2.0f * transform.localScale.y, 0);
            Debug.Log(transform.name + " didn't arrive.");
            flying = false;
            return false;
        }
        //何にもぶつからずに飛んでいる場合
        transform.position = deltaPos;
        d_position = transform.position;
        flying = true;
        return true;
    }

    private bool PositionSetting(Collider hitname)
    {
        RaycastHit[] hits;
        RaycastHit hit;
        hits = Physics.RaycastAll(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, 1.0f);
        //床設置用
        for (int i = 0; i < hits.Length; i++)
        {
            hit = hits[i];
            if (hit.collider.tag == "Field" || hit.collider.tag == "Obstacle")
            {
                transform.position = hit.point + new Vector3(0, 1.0f * transform.localScale.y, 0);
                return true;
            }
        }
        Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0, 0.5f, -1.0f)), Vector3.down * 20.0f, Color.red, 2.0f);
        hits = Physics.RaycastAll(transform.position + transform.TransformDirection(new Vector3(0, 0.5f, -1.0f))
            , Vector3.down, 20.0f);//最初にあたったものが最後(hits[hits.Length - 1])に
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            hit = hits[i];
            if (hit.collider.tag == "Field" || hit.collider.tag == "Obstacle")
            {
                transform.position = hit.point + new Vector3(0, 1.0f * transform.localScale.y, 0);
                return true;
            }
        }
        Debug.Log(transform.name + " don't arrive.(obstacle)");
        return false;
    }

    //IEnumerable hitMeeting()
    private void HitMeeting()
    {
        float r = 0.8f;
        if (r > Vector3.Distance(player.transform.position, transform.position))//半径以内のpikminを探す
        {
            status = true;
        }
        //yield return new WaitForSeconds(0);
    }

    private void TargetItem()
    {
        float r = 3.0f;
        if (target == null)
        {
            foreach (Transform child in ItemList.transform)
            {
                if (r > Vector3.Distance(child.transform.position, transform.position))//半径以内のpelletを探す
                {
                    if (child.GetComponent<Pellet>().Carryable())
                    {
                        carryMove = true;
                        target = child.gameObject;
                        target.GetComponent<Pellet>().AddCarryPik();
                    }
                }
            }
        }
    }

    private void CarryItem()
    {
        transform.LookAt(target.transform.position);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        RaycastHit hit;
        Vector3 front = transform.InverseTransformDirection(new Vector3(0, 0, 0.5f));
        //Debug.DrawRay(transform.position, front);
        //agent.SetDestination(target.gameObject.transform.position);
        if (Physics.Raycast(transform.position, front, out hit, front.magnitude))
        {
            if (hit.collider.tag == "Item" && hit.collider.name == target.transform.name)
            {
                Vector3 back = transform.InverseTransformDirection(new Vector3(0, 0, -0.5f));
                transform.position = hit.point + back;
                //agent.enabled = false;
            }
        }
    }

    private void MoveObstacle()
    {
        //壁タイプの作業
        if (target.name == "WallType")
        {
            if(target.GetComponent<Obstacle_Wall>().Check()){//作業中か否か
                target = null;
                brakeMove = false;
            }
        }
        //押すタイプの作業
        else if (target.name == "PushType")
        {
            if (target.GetComponent<Obstacle_Push>().Check())
            {
                target.GetComponent<Obstacle_Push>().Finish();
                target = null;
                brakeMove = false;
            }
        }
    }

    private void BalanceMove()
    {
        int mass = 1;
        if (Physics.Raycast(transform.position, Vector3.up, out hitObject, 0.5f))
        {
            if (hitObject.collider.tag == "Obstacle" &&
                (hitObject.transform.name == "Left" || hitObject.transform.name == "Right"))
            {
                pikmin.Move(hitObject.point);
            }
        }
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
            if(Physics.Raycast(transform.position, Vector3.down, out hitObject, 1.0f))
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
                else if (hitObject.collider.tag != "Player")
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

    public bool GetFlying()
    {
        return flying;
    }
    
    public void setbrakeMove(bool boolean)
    {
        brakeMove = boolean;
    }

    public void setTarget(GameObject Object)
    {
        target = Object;
        brakeMove = true;
    }
}