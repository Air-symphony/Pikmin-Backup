using UnityEngine;
using System.Collections;

public class Camera_move : MonoBehaviour {
    private GameObject player;
    private float rotationSpeed = 90.0f;
    private float[] angleList = { 0, 7, 10 };
    int i = 1;

    private InputAccess input;
    private bool autoMode = false;
    private bool modeLock = true;
    private float lock_t = 0.0f;
    private UI ui;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        input = new InputAccess();
        ui = GameObject.Find("Canvas").GetComponent<UI>();
    }

    // Update is called once per frame
    void Update() {
        input.InputCameraKey();
        UpDownMove();
        LeftRightMove();
    }

    private void UpDownMove()
    {
        if (input.up)
            i = (i + 1) % angleList.Length;
        else if (input.down)
        {
            i--;
            if (i < 0)
            {
                i += angleList.Length;
            }
        }
        transform.position = player.transform.position + new Vector3(0, angleList[i], 0);//カメラの高さ
    }

    private void LeftRightMove()
    {
        float x = 0.0f;
        if (autoMode)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                player.transform.rotation, rotationSpeed * Time.deltaTime);
        }
        if (lock_t <= 0)
        {
            if (modeLock && (input.left && input.right))
            {
                ui.ChangeMode();
                autoMode = !autoMode;
                modeLock = false;
                lock_t = 1.0f;
            }
            if (!autoMode)
            {
                if (input.left)
                {
                    x = 1.0f;
                }
                else if (input.right)
                {
                    x = -1.0f;
                }
            }
        }
        else
        {
            lock_t -= Time.deltaTime * 3.0f;
        }
        if (( input.left || input.right) == false)
        {
            modeLock = true;
        }
        Vector3 cameraDir = new Vector3(x, 0, 0);//カメラの回転方向
        cameraDir = transform.TransformDirection(cameraDir);
        if (cameraDir.magnitude > 0.1f)
        {
            Quaternion q = Quaternion.LookRotation(cameraDir);//向き先
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotationSpeed * Time.deltaTime);//徐々に回転
        }
    }
}
