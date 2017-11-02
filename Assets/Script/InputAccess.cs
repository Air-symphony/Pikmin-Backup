using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputAccess : MonoBehaviour {
    public bool up, down, left, right;
    public float horizontal;
    public float vertical;//1 = down , -1 = up;
    public float horizontal_R;
    public float vertical_R;//1 = down , -1 = up;
    public bool decide;
    public bool meeting;
    public bool wait;
    
    public void InputPlayerKey()
    {
        decide = false;
        meeting = false;
        wait = false;
        
        horizontal_R = Input.GetAxis("Horizontal_R");
        vertical_R = Input.GetAxis("Vertical_R");
        

        decide |= Input.GetButtonDown("Circle");
        decide |= Input.GetKeyDown(KeyCode.Space);

        meeting |= Input.GetButton("Square");
        meeting |= Input.GetKey(KeyCode.B);

        wait |= Input.GetButtonDown("Triangle");
        wait |= Input.GetKeyDown(KeyCode.V);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        //DownKeyCheck();
    }

    public void InputPointerKey()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        horizontal = 0;
        vertical = 0;
        
        up |= Input.GetKey(KeyCode.UpArrow);
        down |= Input.GetKey(KeyCode.DownArrow);
        left |= Input.GetKey(KeyCode.LeftArrow);
        right |= Input.GetKey(KeyCode.RightArrow);

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    public void InputCameraKey()
    {
        up = false;
        down = false;
        left = false;
        right = false;

        up |= Input.GetKeyDown(KeyCode.W);

        down |= Input.GetKeyDown(KeyCode.S);

        left |= Input.GetButton("L1");
        left |= Input.GetKey(KeyCode.A);
        right |= Input.GetButton("R1");
        right |= Input.GetKey(KeyCode.D);
    }

    bool oneFrame = false;
    public void InputOnionKey()
    {
        up = false;
        down = false;
        decide = false;
        
        up |= Input.GetKey(KeyCode.UpArrow);
        up |= (Input.GetAxis("Vertical") < 0);

        down |= Input.GetKey(KeyCode.DownArrow);
        down |= (Input.GetAxis("Vertical") > 0);

        if (Input.GetButtonUp("Circle") || Input.GetKeyUp(KeyCode.Space))
        {
            oneFrame = true;
        }
        if (oneFrame)
        {
            decide |= Input.GetButtonDown("Circle");
            decide |= Input.GetKeyDown(KeyCode.Space);
            if (decide)
            {
                oneFrame = false;
            }
        }
    }

    public void InputStartKey()
    {
        decide = false;

        decide |= Input.GetButtonDown("Circle");
        decide |= Input.GetKeyDown(KeyCode.Space);
    }

    /*デバッグ用*/
    void DownKeyCheck()
    {
        if (Input.anyKey)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    //処理を書く
                    Debug.Log(code);
                    break;
                }
            }
        }
    }
}
