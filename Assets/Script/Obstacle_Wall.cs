﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Wall : MonoBehaviour {
    private bool obstacle = true;
    private float HP = 100.0f;
    private int nowPik;
    private float t = 0.0f;
    private GameObject text;

    // Use this for initialization
    void Start()
    {
        text = this.gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (nowPik > 0)
        {
            t += Time.deltaTime;
            if (t >= 1.0f && !obstacle)
            {
                HP -= nowPik * 0.8f;//100匹で5秒
                t = 0.0f;
            }
            if (HP >= 66)
            {
                transform.localScale = new Vector3(10, 10, 1.5f);
            }
            else if (HP >= 33)
            {
                transform.localScale = new Vector3(10, 6, 1.5f);
            }
            else if (HP > 0)
            {
                transform.localScale = new Vector3(10, 3, 1.5f);
            }
        }
        if (HP > 0)
        {
            obstacle = false;
        }
        else
        {
            obstacle = true;
            Destroy(this.gameObject);
        }
        printHP();
    }

    void OnTriggerStay(Collider other)
    {
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

    void printHP()
    {
        if (nowPik > 0)
        {
            text.SetActive(true);
            GetComponentInChildren<TextMesh>().text = "HP:" + HP + ", " + nowPik;
            //GetComponentInChildren<TextMesh>().transform.rotation = cam.transform.rotation;
        }
        else
        {
            text.SetActive(false);
        }
    }

    public void SetWallType(Vector3 pos, int hp)
    {
        transform.position = pos;
        HP = hp;
    }

    public void RemoveNowPik()
    {
        nowPik--;
    }

    public bool Check()
    {
        return obstacle;
    }
}
