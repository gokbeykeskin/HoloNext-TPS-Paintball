using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using Photon;
using Photon.Pun;
public class ThirdPersonShooterController : MonoBehaviour
{

    [SerializeField] int damage=20;
    [Header("Mouse Sensitivity Options")]
    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity = 0.6f;
    
    [Header("Inverse Kinematics")]
    [SerializeField] GameObject aim,bodyAim;

    [Header("Camera Settings")]
    [SerializeField] private float camShakeIntensity = 0.5f;

    [Header("Required Transforms")]
    [SerializeField] LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] Transform aimTransform;
    [SerializeField] Transform bulletPrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] Transform vfxHit;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private PhotonView view;
    float fireRate = 0.3f;
    float nextFire = 0.0f;
    Health playerHealth;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        view = GetComponent<PhotonView>();
        if(view.IsMine){
            playerHealth = GetComponent<Health>();
            
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
        if(transform.position.y<-10) StartCoroutine(playerHealth.Respawn());
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width /2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray,out RaycastHit raycastHit, 999f, aimColliderLayerMask)){
            aimTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        RotateTowardsAim(mouseWorldPosition);
        

        if(starterAssetsInputs.aim){
            thirdPersonController.SetSensitivity(aimSensitivity);

        }
        else{
            thirdPersonController.SetSensitivity(normalSensitivity);
        }
        
        if(starterAssetsInputs.shoot && Time.time > nextFire){
            Shoot(mouseWorldPosition,raycastHit);
            
        }
        else starterAssetsInputs.shoot = false;
    }

    void RotateTowardsAim(Vector3 mouseWorldPosition){
        if(starterAssetsInputs.aim || starterAssetsInputs.shoot){
            thirdPersonController.SetRotateOnMove(false); // Stop character controller rotation
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = aimDirection;
        }
        else thirdPersonController.SetRotateOnMove(true);
    }





    
    void Shoot(Vector3 mouseWorldPos,RaycastHit raycastHit){
        CamShake.Instances[0].Shake(camShakeIntensity,0.1f);
        nextFire = Time.time + fireRate;
        if(CamShake.Instances[1]!=null) // if the player is aiming
            CamShake.Instances[1].Shake(camShakeIntensity,0.1f);
            view.RPC ("ShootRPC",RpcTarget.All,mouseWorldPos,raycastHit.point);

        IHealthManager opponentHealthManager = raycastHit.transform.GetComponentInParent<IHealthManager>(); 
        
        if(opponentHealthManager != null){
            opponentHealthManager.TakeDamage(damage);
        }
    }

    [PunRPC]
    void ShootRPC(Vector3 mouseWorldPos,Vector3 particlePos){
        Vector3 aimDir = (mouseWorldPos-shootPoint.position).normalized;
        Instantiate(bulletPrefab,shootPoint.position,Quaternion.LookRotation(aimDir));
        Instantiate(vfxHit,particlePos,Quaternion.identity);
    }





}
