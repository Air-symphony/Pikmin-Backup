using UnityEngine;
using System.Collections;

public class Pointer : MonoBehaviour {
    private InputAccess input;
    private GameObject player;
    public GameObject cam;
    
    float moveSpeed = 30.0f;
    private Vector3 v_0 = new Vector3(0, 10.0f, 5.0f);
    private RaycastHit hitObject;//外部利用

    // Use this for initialization
    void Start () {
        input = new InputAccess();
        player = GameObject.Find("Player");
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
        Vector3 move = new Vector3(x, 0, -z).normalized;//単位ベクトル化
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
                                        player.transform.position.y + v_0.y,
                                        transform.position.z);
            
            //Layer = Fieldの中で、ヒットした場所にポインタを表示
            int layerMask = LayerMask.GetMask("Field");
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

    public RaycastHit getHItObject()
    {
        return hitObject;
    }
}