using UnityEngine;
using System.Collections;

public class Pointer : MonoBehaviour {
    private Vector3 v0 = new Vector3(0, 8.0f, 5.0f);
    private Transform playerTrans;
    private Vector3 pos;
    private RaycastHit hitObject;//外部利用
    private string hitName;
    private float player_y;
    private Vector3 memoryLocalPoint;

    // Use this for initialization
    void Start () {
        playerTrans = transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("point");
    }

    IEnumerator point()
    {
        float y;
        float t = 0.0f;
        pos = playerTrans.position + playerTrans.TransformDirection(new Vector3(0, 0, -1));//射出初期位置
        while (Time.timeScale != 0.0f)//Time.timeScale == 0.0fの時はなし
        {
            t += Time.deltaTime;// Scale修正
            y = v0.y * t - 0.5f * 9.8f * t * t;
            transform.localPosition = new Vector3(0, y, v0.z * t) + new Vector3(0, 0, -1);//playerの真後ろ(-1)から投げる
            if (Physics.Raycast(pos, transform.position - pos, out hitObject, Vector3.Distance(pos, transform.position)))
            {
                if (hitName == hitObject.collider.name && player_y == playerTrans.position.y)//hitObjectに変化なし⋀平地
                {
                    transform.localPosition = memoryLocalPoint;
                    break;
                }
                else if (hitObject.collider.tag == "Field" || hitObject.collider.tag == "Obstacle")//床と障害物の判定
                {
                    hitName = hitObject.collider.name;//名前の保存
                    memoryLocalPoint = playerTrans.InverseTransformPoint(hitObject.point) + new Vector3(0, 0.1f, 0);//着地点の保存
                    player_y = playerTrans.position.y;//Playerの座標の保存
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