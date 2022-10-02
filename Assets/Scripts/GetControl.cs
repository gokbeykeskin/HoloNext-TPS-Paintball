using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using TMPro;
public class GetControl : MonoBehaviourPunCallbacks
{
    private PhotonView view;
    [SerializeField] private TMP_Text nameText;
    private Transform mainCamTransform;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        if(view.IsMine){
            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;
            nameText.enabled = false;
        }
        else{
            SetName();
        }
        mainCamTransform = Camera.main.transform;
    }

    private void SetName()
    {
        nameText.text = view.Owner.NickName;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        nameText.transform.LookAt(mainCamTransform);
    }
}
