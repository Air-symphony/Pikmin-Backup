using UnityEngine;
using System.Collections;

public class CameraAngle : MonoBehaviour {
    private GameObject player;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(player.transform.position);
    }
}
