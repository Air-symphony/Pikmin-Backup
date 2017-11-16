using UnityEngine;
using System.Collections;

public class Onion : MonoBehaviour {
    public GameObject pikmin;
    private GameObject follow;
    private GameObject indepedent;
    private int keep_Pikmin = 110;
    private int feild_Max = 100;
    private int start_num = 10;

    public GameObject player;
    private int num = 0;

    // Use this for initialization
    void Start()
    {
        follow = GameObject.Find("Follow");
        indepedent = GameObject.Find("Indepedent");
        for (int i = 1; i <= start_num; i++)
        {
            float rndx = Random.Range(-1.0f, 1.0f);
            float rndz = Random.Range(-1.0f, 1.0f);
            //GameObject obj = Instantiate(pikmin, transform.position + new Vector3(rndx, -3.0f, rndz), transform.rotation) as GameObject;
            GameObject obj = Instantiate(pikmin, player.transform.position + new Vector3(rndx, 3.0f, rndz), transform.rotation) as GameObject;
            obj.GetComponent<Pikmin_move>().status = true;
            obj.transform.name = "Pikmin(" + i + ")";
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void MoveAdd(int num)
    {
        this.num = num;
        StartCoroutine("Add");
    }

    public void MoveTake(int num)
    {
        int i = 0;
        foreach (Transform child in follow.transform)
        {
            Destroy(child.gameObject);
            keep_Pikmin++;
            i++;
            if (num <= i)
            {
                break;
            }
        }
    }

    public int GetKeepPikmin()
    {
        return keep_Pikmin;
    }

    public int GetMax()
    {
        return feild_Max;
    }

    IEnumerator Add()
    {
        for (int i = 1; i <= num; i++)
        {
            float rndx = Random.Range(-3.0f, 3.0f);
            float rndz = Random.Range(-3.0f, 3.0f);
            GameObject obj = Instantiate(pikmin, transform.position + new Vector3(rndx, -1.0f, rndz), transform.rotation) as GameObject;
            obj.GetComponent<Pikmin_move>().status = true;
            int number = follow.transform.childCount + indepedent.transform.childCount + i;
            obj.transform.name = "Pikmin(" + number + ")";
            keep_Pikmin--;
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }

    IEnumerator Take()
    {
        for (int i = 1; i <= num; i++)
        {
            float rndx = Random.Range(-3.0f, 3.0f);
            float rndz = Random.Range(-3.0f, 3.0f);
            GameObject obj = Instantiate(pikmin, transform.position + new Vector3(rndx, -1.0f, rndz), transform.rotation) as GameObject;
            obj.GetComponent<Pikmin_move>().status = true;
            int number = follow.transform.childCount + indepedent.transform.childCount + i;
            obj.transform.name = "Pikmin(" + number + ")";
            keep_Pikmin--;
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }
}
