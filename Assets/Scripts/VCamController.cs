using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using StarterAssets;

public class VCamController : MonoBehaviour
{
    private PhotonView view;
    private StarterAssetsInputs starterAssetsInputs;

    [SerializeField] CinemachineVirtualCamera followcam,aimcam;

    // Start is called before the first frame update
    void Awake()
    {
        view = GetComponent<PhotonView>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    void Start(){
        if(view.IsMine){
            followcam = Instantiate(followcam,transform.position,Quaternion.identity);
            aimcam = Instantiate(aimcam,transform.position,Quaternion.identity);

            followcam.Follow = transform.GetChild(0);
            aimcam.Follow = transform.GetChild(0);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if(starterAssetsInputs.aim){
            aimcam.gameObject.SetActive(true);

        }
        else{
            aimcam.gameObject.SetActive(false);
        }
    }
}
