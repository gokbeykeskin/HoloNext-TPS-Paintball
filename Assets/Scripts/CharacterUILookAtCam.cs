using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class CharacterUILookAtCam : MonoBehaviour
{
    private PhotonView view;
    private Transform mainCamTransform;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider healthBar;


    // Start is called before the first frame update
    void Start()
    {
        if(view.IsMine){
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
        healthBar.transform.LookAt(mainCamTransform);
    }
}
