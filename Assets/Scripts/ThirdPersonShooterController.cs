using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using Photon;
using Photon.Pun;
public class ThirdPersonShooterController : MonoBehaviour
{

    [HideInInspector]
    public static Vector3[] spawnPoints = {new Vector3(-0.23f,1f,16.3f),new Vector3(-0.23f,1f,-18.2f),new Vector3(-12.4f,1f,0f),new Vector3(14f,1f,0f)};
    [HideInInspector] 
    public bool inWaitingRoom=false;

    [Header("Mouse Sensitivity Options")]
    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity = 0.6f;
    
    [Header("Inverse Kinematics")]
    [SerializeField] GameObject aim,bodyAim;

    [Header("Camera Settings")]
    [SerializeField] CinemachineVirtualCamera followcam,aimcam;
    [SerializeField] private float camShakeIntensity = 0.5f;

    [Header("Required Transforms")]
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] Transform aimTransform;
    [SerializeField] Transform bulletPrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] private Transform vfxHit;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private PhotonView view;
    float fireRate = 0.3f;
    float nextFire = 0.0f;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        view = GetComponent<PhotonView>();
        if(view.IsMine){
            followcam = Instantiate(followcam,transform.position,Quaternion.identity);
            aimcam = Instantiate(aimcam,transform.position,Quaternion.identity);

            followcam.Follow = transform.GetChild(0);
            aimcam.Follow = transform.GetChild(0);

            thirdPersonController = GetComponent<ThirdPersonController>();
            starterAssetsInputs = GetComponent<StarterAssetsInputs>();
            aimTransform = Instantiate(aimTransform,transform.position,Quaternion.identity);
            
            var data = aim.GetComponent<MultiAimConstraint>().data.sourceObjects;
            data.Clear();
            data.Add(new WeightedTransform(aimTransform,1));
            aim.GetComponent<MultiAimConstraint>().data.sourceObjects = data;
            
            data = bodyAim.GetComponent<MultiAimConstraint>().data.sourceObjects;
            data.Clear();
            data.Add(new WeightedTransform(aimTransform,0.65f));
            bodyAim.GetComponent<MultiAimConstraint>().data.sourceObjects = data;
            
            GetComponent<RigBuilder>().Build();
        }
        


    }
    private void Update(){
        if(!view.IsMine) return;
        if(transform.position.y<-10) StartCoroutine(Respawn());
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width /2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray,out RaycastHit raycastHit, 999f, aimColliderLayerMask)){
            aimTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if(starterAssetsInputs.aim || starterAssetsInputs.shoot){
            thirdPersonController.SetRotateOnMove(false); // Stop character controller rotation
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = aimDirection;
        }
        else thirdPersonController.SetRotateOnMove(true);


        if(starterAssetsInputs.aim){
            thirdPersonController.SetSensitivity(aimSensitivity);
            aimcam.gameObject.SetActive(true);

        }
        else{
            aimcam.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
        }
        
        if(starterAssetsInputs.shoot && Time.time > nextFire){
            CamShake.Instances[0].Shake(camShakeIntensity,0.1f);
            nextFire = Time.time + fireRate;
            if(CamShake.Instances[1]!=null) // if the player is aiming
                CamShake.Instances[1].Shake(camShakeIntensity,0.1f);
                view.RPC ("Shoot",RpcTarget.All,mouseWorldPosition,raycastHit.point);

            
            if(raycastHit.transform.tag=="Player"){
                raycastHit.collider.gameObject.GetComponentInParent<ThirdPersonShooterController>().TakeDamage(20);
            }
        }
        else starterAssetsInputs.shoot = false;
    }

    [PunRPC]
    void Shoot(Vector3 mouseWorldPos,Vector3 particlePos){
        Vector3 aimDir = (mouseWorldPos-shootPoint.position).normalized;
        Instantiate(bulletPrefab,shootPoint.position,Quaternion.LookRotation(aimDir));
        Instantiate(vfxHit,particlePos,Quaternion.identity);
    }

    void TakeDamage(int damage){
        view.RPC("RPC_TakeDamage",RpcTarget.All,damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage){
        gameObject.GetComponent<Health>().Harm(damage);
        if(gameObject.GetComponent<Health>().GetHealth()<=0)StartCoroutine(Respawn());
    }

    public IEnumerator Respawn(){
        if(!view.IsMine) yield break;
        inWaitingRoom = true;
        int index = Random.Range(0,3);
        transform.position = new Vector3(-67.80f,0,19.44f); //waiting room
        yield return new WaitForSeconds(5f);
        inWaitingRoom=false;
        transform.position = spawnPoints[index]; //waiting room
        view.RPC("ResetHP",RpcTarget.All);
    }
    [PunRPC]
    void ResetHP(){
        gameObject.GetComponent<Health>().ResetHealth();
    }


}
