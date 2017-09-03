using UnityEngine;
using System.Collections;

public class Pellet : MonoBehaviour {
    private UnityEngine.AI.NavMeshAgent agent;
    private UnityEngine.AI.NavMeshObstacle obstacle;
    private float movespeed = 1;
    private int needPik;
    private int maxPik;//基本的にneedの2倍
    private int carryPik;
    private GameObject cam;
    private GameObject point;

    // Use this for initialization
    void Start () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        cam = GameObject.Find("Camera");
        point = GameObject.Find("Onion_Plate");
        agent.enabled = false;
        obstacle.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.transform.GetChild(0).gameObject.SetActive(carryPik > 0);
        if (carryPik > 0)
        {
            GetComponentInChildren<TextMesh>().text = carryPik + "/" + needPik;
            GetComponentInChildren<TextMesh>().transform.rotation = cam.transform.rotation;
            MovePellet();
        }
    }

    private void MovePellet()
    {
        if (carryPik >= needPik)
        {
            obstacle.enabled = false;
            agent.enabled = true;
            //transform.position = 
            agent.SetDestination(point.transform.position);
            agent.speed = movespeed * (carryPik / needPik);
        }
        else
        {
            agent.enabled = false;
            obstacle.enabled = true;
        }
    }

    public void AddCarryPik()
    {
        carryPik++;
    }

    public void RemoveCarryPik()
    {
        carryPik--;
    }

    public int GetCarryPik()
    {
        return carryPik;
    }

    public int GetNeedPik()
    {
        return needPik;
    }

    public int GetMaxPik()
    {
        return maxPik;
    }

    public bool Carryable()
    {
        return carryPik < maxPik; 
    }

    public void SetPellet(int num, int max)
    {
        needPik = num;
        maxPik = max;
    }
}
