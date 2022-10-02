using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;

public class GetControl : MonoBehaviour
{
    private PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        if(view.IsMine){
            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }

    }


}
