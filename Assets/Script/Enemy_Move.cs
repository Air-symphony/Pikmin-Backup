using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour {
    private CharacterController character;
    private Animator animator;
    private GameObject player;
    private Collider attackArea;
    public float moveSpeed = 2.0f;
    float move_t = 0.0f;

    private bool attack = false;

    // Use this for initialization
    void Start () {
        character = GetComponent<CharacterController>();
        this.animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        attackArea = transform.GetComponentInChildren<Collider>();
    }
	
	// Update is called once per frame
	void Update () {
        MoveContoller();
    }

    private void MoveContoller()
    {
        Vector3 move = new Vector3(0, 0, 0);
        if (Vector3.Distance(player.transform.position, transform.position) > 3.0f)
        {
            this.animator.SetBool("IsMove", true);
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            move += transform.forward * moveSpeed * Time.deltaTime;
        }
        character.Move(Gravity(move));
        if (transform.position.y <= -2.0f)
        {
            Debug.Log(transform.name + " is dead.");
            Destroy(this.gameObject);
        }
    }

    private Vector3 Gravity(Vector3 move)
    {
        RaycastHit hit;
        Vector3 down = transform.InverseTransformDirection(new Vector3(0, -0.5f, 0));
        Debug.DrawRay(transform.position, down);
        if (Physics.Raycast(transform.position, down, out hit, down.magnitude))
        {
            if (hit.collider.tag == "Field" || hit.collider.tag == "Obstacle")
            {
                //hit.point;
                move_t = 0.0f;
            }
        }
        move_t += Time.deltaTime;
        return move -= new Vector3(0, 0.5f * 9.8f * move_t * move_t, 0);
    }

    void OnTriggerStay(Collider other)
    {
        GameObject _object = other.gameObject;
        if (_object.tag == "Pikmin" || _object.tag == "Player")
        {
            this.animator.SetBool("Attack", true);
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Enemy attacks");

        yield break;
    }
}
