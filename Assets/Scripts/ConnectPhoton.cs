using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
public class ConnectPhoton : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField name;
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text buttonText;
    // Start is called before the first frame update

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.JoinLobby();
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server for reason" + cause.ToString());
    }

    public override void OnJoinedLobby()
    {
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom("00",option,TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        CreatePlayer();
    }
    GameObject player;
    void CreatePlayer(){
        int index = Random.Range(0,3);
        player = PhotonNetwork.Instantiate("PlayerArmature",ThirdPersonShooterController.spawnPoints[index],Quaternion.identity,0,null);
        
        panel.SetActive(false);
    }

    public void Join(){
        buttonText.text = "Joining...";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = name.text;//lgn_usrname_input.text;
        PhotonNetwork.GameVersion = "0.0.1";//MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

}