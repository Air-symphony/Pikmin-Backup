using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Balance : MonoBehaviour {
    //private bool left;
    private GameObject left;
    private GameObject right;
    private int left_c;//count
    private int right_c;
    Vector3 high = new Vector3(0, 0.5f, 0);
    Vector3 low = new Vector3(0, -2.4f, 0);
    Vector3 mid;

    // Use this for initialization
    void Start () {
		left = transform.GetChild(0).gameObject;
        right = transform.GetChild(1).gameObject;
        mid = (high + low) / 2.0f;
    }
	
	// Update is called once per frame
	void Update () {
        left.transform.GetChild(0).gameObject.SetActive(left_c + right_c > 0);
        right.transform.GetChild(0).gameObject.SetActive(left_c + right_c > 0);

        if (left_c + right_c > 0)
        {
            left.transform.GetComponentInChildren<TextMesh>().text = left_c + "";
            right.transform.GetComponentInChildren<TextMesh>().text = right_c + "";
        }
        if (left_c > right_c)
        {
            left.transform.localPosition += new Vector3(0, low.y - left.transform.localPosition.y, 0) * Time.deltaTime;
            right.transform.localPosition += new Vector3(0, high.y - right.transform.localPosition.y, 0) * Time.deltaTime;
        }
        else if (left_c < right_c)
        {
            left.transform.localPosition += new Vector3(0, high.y - left.transform.localPosition.y, 0) * Time.deltaTime;
            right.transform.localPosition += new Vector3(0, low.y - right.transform.localPosition.y, 0) * Time.deltaTime;
        }
        else
        {
            left.transform.localPosition += new Vector3(0, mid.y - left.transform.localPosition.y, 0) * Time.deltaTime;
            right.transform.localPosition += new Vector3(0, mid.y - right.transform.localPosition.y, 0) * Time.deltaTime;
        }
    }

    public void ChangeLeft(int i)
    {
        left_c += i;
    }
    public void ChangeRight(int i)
    {
        right_c += i;
    }
}
