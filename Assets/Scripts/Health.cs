using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] int health = 100;
    [SerializeField] Slider healthBar;
    PhotonView view;
    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(view.IsMine) view.RPC("UpdateHealthBar",RpcTarget.All);
    }

    void Update()
    {
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(health);
        }
        else{ //reading
            health = (int)stream.ReceiveNext();
        }
    }

    public void Harm(int damage){ 
            health-=damage;
            if(view.IsMine) view.RPC("UpdateHealthBar",RpcTarget.All);
            Debug.Log("Health:"+health);
    }

    [PunRPC]
    void UpdateHealthBar(){
        healthBar.value = (float)GetHealth()/100f;
    }

    public int GetHealth(){
        return health;
    }
    public void ResetHealth(){
        health=100;
        if(view.IsMine) view.RPC("UpdateHealthBar",RpcTarget.All);
    }

}
