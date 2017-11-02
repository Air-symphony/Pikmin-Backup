using UnityEngine;
using System.Collections;

public class Pointer : MonoBehaviour {
    private InputAccess input;
    private GameObject player;
    public GameObject cam;
    private float moveSpeed = 50.0f;
    private float movePointer_r = 8.0f;
    private Vector3 v_0 = new Vector3(0, 10.0f, 0.0f);

    private RaycastHit hitObject;//外部利用

    // Use this for initialization
    void Start () {
        input = new InputAccess();
        player = GameObject.Find("Player");
        transform.position = player.transform.position + Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        PointerMove();
    }

    //ポインタの移動と表示
    private void PointerMove()
    {
        float x = 0, z = 0;
        input.InputPointerKey();

        if (input.up || input.down || input.left || input.right)
        {
            if (input.up) z = -1;
            else if (input.down) z = 1;
            if (input.left) x = -1;
            else if (input.right) x = 1;
            //カメラと軸を合わせる
            transform.right = cam.transform.right;
        }
        if (input.horizontal != 0 || input.vertical != 0)
        {
            x = input.horizontal;
            z = input.vertical;
            transform.right = cam.transform.right;
        }
        Vector3 move = new Vector3(x, 0, -z).normalized;//単位ベクトル化

        float move_s = 0.0f;
        //Playerが半径内にいるかどうか
        Vector3 playerPos = player.transform.position;
        Vector3 pointerPos = transform.position;
        playerPos.y = pointerPos.y = 0;

        float distance = Vector3.Distance(playerPos, pointerPos);
        if (distance <= movePointer_r)
        {
            move_s = moveSpeed;
        }
        else
        {
            move_s = player.GetComponent<Player_move>().moveSpeed;
        }
        move = transform.TransformDirection(move * move_s);//自分の向きに合うmoveに変更~= 
        Vector3 move_after = transform.position + move * Time.deltaTime;

        move_after.y = 0;
        float distance_after = Vector3.Distance(playerPos, move_after);

        transform.rotation = Quaternion.Euler(90, 0, -90);//ポイントを正しく表示 

        if (distance < movePointer_r + 0.4f || distance > distance_after)
        {
            transform.position = move_after;//移動

            //投げられる最大の高さ
            Vector3 apex = new Vector3(transform.position.x,
                                        player.transform.position.y + v_0.y,
                                        transform.position.z);

            //Layer = Fieldの中で、ヒットした場所にポインタを表示
            int layerMask = LayerMask.GetMask("Field");
            Debug.DrawRay(apex, Vector3.down * 30, Color.green);//Rayの描画
            if (Physics.Raycast(apex, Vector3.down, out hitObject, 30, layerMask))
            {
                if (hitObject.transform.tag == "Field" ||
                    hitObject.transform.tag == "Obstacle")
                {
                    transform.position = hitObject.point + Vector3.up * 0.3f;
                }
            }
        }
    }

    public float GetmovePointer_r()
    {
        return movePointer_r;
    }

    public RaycastHit getHItObject()
    {
        return hitObject;
    }
}