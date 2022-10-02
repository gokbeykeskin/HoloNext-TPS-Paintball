using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamShake : MonoBehaviour
{
    public static CamShake[] Instances={null,null}; //aim cam and follow cam to access from shooter controller
    CinemachineVirtualCamera cinemachineVirtualCamera;
    float shakeTimer=0;
    

    // Start is called before the first frame update
    void Awake()
    {
        if(Instances[0]==null) Instances[0]=this;
        else Instances[1]=this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void Shake(float instensity,float time){
        var multiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = instensity;
        shakeTimer = time;
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeTimer>0){
            shakeTimer -= Time.deltaTime;
            if(shakeTimer<=0f){
                var multiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                multiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }

    }
}
