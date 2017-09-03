using UnityEngine;
using System.Collections;

public class CreatePellet : MonoBehaviour {
    public GameObject pellet;

    // Use this for initialization
    void Start () {//デバッグ用
        //int num = 10;
        //MakePellet(new Vector3(0, 3, 7), 1);
        /*for (int i = 1; i < num; i++)
        {
            float rndx = Random.Range(-30, 30);
            float rndz = Random.Range(-30, 30);
            Vector3 pos = new Vector3(rndx, 5.0f, rndz);
            makePellet(pos, i);
        }*/
    }

    private void MakePellet(Vector3 pos, int i)
    {
        GameObject obj = Instantiate(pellet, pos, transform.rotation) as GameObject;
        obj.transform.SetParent(this.transform);
        obj.transform.name = "Pellet(" + i + ")";
        obj.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
        obj.GetComponent<Pellet>().SetPellet(i * 2, (i * 2) * 2);
    }
}
