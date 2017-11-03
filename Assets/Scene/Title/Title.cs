using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
    private InputAccess input;
    public GameObject text;
    private Text push;
    private float t;
    private bool colortime;

	// Use this for initialization
	void Start () {
        input = new InputAccess();
        Time.timeScale = 1.0f;
        t = 0;
        colortime = false;
        push = text.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        if (t < 1.0f)
        {
            if (colortime)
            {
                push.color = Color.red;
            }
            else
            {
                push.color = Color.black;
            }
        }
        else
        {
            colortime = !colortime;
            t = 0;
        }

        input.InputStartKey();
        if (input.decide)
        {
            SceneManager.LoadScene("Game");
        }
	}
}
