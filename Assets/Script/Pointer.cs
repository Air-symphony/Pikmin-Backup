using UnityEngine;
using System.Collections;

public class Pointer : MonoBehaviour {
    private InputAccess input;
    public GameObject cam;
    
    float moveSpeed = 30.0f;
    Vector3 move;
    private Vector3 v0 = new Vector3(0, 8.0f, 5.0f);
    private GameObject player;
    private Vector3 pos;
    private RaycastHit hitObject;//外部利用
    private string hitName;
    private float player_y;
    private Vector3 memoryLocalPoint;

    // Use this for initialization
    void Start () {
        input = new InputAccess();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        PointerMove();
        Debug_ThrowLine();
        //StartCoroutine("point");
    }

    //ポインタの移動と表示
    private void PointerMove()
    {
        float x = 0, z = 0;
        input.InputPointerKey();

        /*
        x = input.horizontal;
        z = input.vertical;
        if (x != 0 || z != 0) transform.right = cam.transform.right;
        */
        if (input.up || input.down || input.left || input.right)
        {
            if (input.up) z = -1;
            else if (input.down) z = 1;
            if (input.left) x = -1;
            else if (input.right) x = 1;
            //カメラと軸を合わせる
            transform.right = cam.transform.right;
        }
        move = new Vector3(x, 0, -z).normalized;//単位ベクトル化
        move = transform.TransformDirection(move * moveSpeed);//自分の向きに合うmoveに変更

        //キー入力を受け付けた後の座標が、半径内かどうか
        Vector3 move_after = transform.position + move * Time.deltaTime;
        float distance = Vector3.Distance(move_after, player.transform.position);
        transform.rotation = Quaternion.Euler(90, 0, -90);//ポイントを正しく表示 
        if (distance < 9.0f)
        {
            transform.position = move_after;//移動

            //投げられる最大の高さ
            Vector3 apex = new Vector3(transform.position.x,
                                        player.transform.position.y + v0.y,
                                        transform.position.z);
            Debug.DrawLine(apex, apex + Vector3.down * 100, Color.red);//デバッグ

            //Layer = Fieldの中で、ヒットした場所にポインタを表示
            int layerMask = LayerMask.GetMask("Field");
            if (Physics.Raycast(apex, Vector3.down, out hitObject, 100, layerMask))
            {
                if (hitObject.transform.tag == "Field" ||
                    hitObject.transform.tag == "Obstacle")
                {
                    transform.position = hitObject.point + Vector3.up * 0.3f;
                }
            }
        }
    }

    private void Debug_ThrowLine()
    {
        Vector3 apex = (transform.position - player.transform.position) / 2.0f;
        apex.y = 0;
        apex += player.transform.position + new Vector3(0, v0.y, 0);
        Debug.DrawLine(player.transform.position, apex, Color.green);
        Debug.DrawLine(apex, transform.position, Color.green);

    }

    IEnumerator point()
    {
        float y;
        float t = 0.0f;
        pos = player.transform.position + player.transform.TransformDirection(new Vector3(0, 0, -1));//射出初期位置
        while (Time.timeScale != 0.0f)//Time.timeScale == 0.0fの時はなし
        {
            t += Time.deltaTime;// Scale修正
            y = v0.y * t - 0.5f * 9.8f * t * t;
            transform.localPosition = new Vector3(0, y, v0.z * t) + new Vector3(0, 0, -1);//playerの真後ろ(-1)から投げる
            if (Physics.Raycast(pos, transform.position - pos, out hitObject, Vector3.Distance(pos, transform.position)))
            {
                if (hitName == hitObject.collider.name && player_y == player.transform.position.y)//hitObjectに変化なし⋀平地
                {
                    transform.localPosition = memoryLocalPoint;
                    break;
                }
                else if (hitObject.collider.tag == "Field" || hitObject.collider.tag == "Obstacle")//床と障害物の判定
                {
                    hitName = hitObject.collider.name;//名前の保存
                    //memoryLocalPoint = playerTrans.InverseTransformPoint(hitObject.point) + new Vector3(0, 0.1f, 0);//着地点の保存
                    //player_y = playerTrans.position.y;//Playerの座標の保存
                    transform.localPosition = memoryLocalPoint;
                    break;
                }
            }
            if (transform.position.y < -2.0f)//hitObject = null
            {
                Debug.Log("Cursor doesn't hit.");
                break;
            }
            pos = transform.position;//Raycastの始点の更新
        }
        yield return new WaitForSeconds(0.5f);
    }

    public RaycastHit getHItObject()
    {
        return hitObject;
    }

    public void SetV0(Vector3 v)
    {
        v0 = v;
    }
}