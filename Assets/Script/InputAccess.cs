using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAccess : MonoBehaviour {
    public bool up, down, left, right;
    public float horizontal;
    public float vertical;//1 = down , -1 = up;
    public bool decide;
    public bool meeting;
    public bool wait;
    
    public void InputPlayerKey()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        horizontal = 0;
        vertical = 0;

        decide = false;
        meeting = false;
        wait = false;

        up |= Input.GetKey(KeyCode.UpArrow);
        down |= Input.GetKey(KeyCode.DownArrow);
        left |= Input.GetKey(KeyCode.LeftArrow);
        right |= Input.GetKey(KeyCode.RightArrow);

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

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

    public void InputOnionKey()
    {
        up = false;
        down = false;
        decide = false;
        horizontal = 0;
        vertical = 0;
        
        up |= Input.GetKey(KeyCode.UpArrow);
        up |= Input.GetAxis("Vertical") < 0;

        down |= Input.GetKey(KeyCode.DownArrow);
        down |= Input.GetAxis("Vertical") > 0;

        decide |= Input.GetButtonDown("Circle");
        decide |= Input.GetKeyDown(KeyCode.Space);
    }

    public void InputStartKey()
    {
        decide = false;

        decide |= Input.GetButtonDown("Circle");
        decide |= Input.GetKeyDown(KeyCode.Space);
    }
}
