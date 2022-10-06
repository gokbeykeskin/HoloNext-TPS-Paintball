using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
public class Health : MonoBehaviourPunCallbacks/*, IPunObservable*/, IHealthManager
{
    [SerializeField] int health = 100;
    [SerializeField] Slider healthBar;
    
    [HideInInspector] 
    public bool inWaitingRoom=false;
    PhotonView view;
    ThirdPersonShooterController thirdPersonShooterController;
    void Awake()
    {
        thirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(view.IsMine) view.RPC("UpdateHealthBar",RpcTarget.All);
    }


    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(health);
        }
        else{ //reading
            health = (int)stream.ReceiveNext();
        }
    }*/

    public void Harm(int damage){ 
            if(inWaitingRoom) return;
            health-=damage;
            if(view.IsMine) view.RPC("UpdateHealthBar",RpcTarget.All);
    }

    public void TakeDamage(int damage){
        view.RPC("RPC_TakeDamage",RpcTarget.All,damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage){
        Harm(damage);
        if(GetHealth()<=0)StartCoroutine(Respawn());
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

    public IEnumerator Respawn(){
        if(!view.IsMine) yield break;
        inWaitingRoom = true;
        int index = Random.Range(0,3);
        transform.position = SpawnPoints.waitingRoomPoint; //waiting room
        yield return new WaitForSeconds(5f);
        inWaitingRoom=false;
        Debug.Log("index:"+index + SpawnPoints.spawnPoints[2]);
        transform.position = SpawnPoints.spawnPoints[index];
        view.RPC("ResetHP",RpcTarget.All);
    }

    [PunRPC]
    void ResetHP(){
        ResetHealth();
    }

}
