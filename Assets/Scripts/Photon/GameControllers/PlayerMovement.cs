using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM
    private PhotonView PV;
    private CharacterController myCC;
    public float movementSpeed = 4;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myCC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            BasicMovement();
        }
    }

    void BasicMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            myCC.Move(
                ((transform.up * vertical) + (transform.right * horizontal)).normalized
                * Time.deltaTime * movementSpeed
                );
        }
    }
}
