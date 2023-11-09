using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorVFX_Script : MonoBehaviour
{

    public GameObject m_Enemy;
    public ParticleSystem m_PS;
    public Vector3 m_StartPosOffset;

    public float m_DestroyTime = 3f; 

    void Start()
    {
        transform.parent.parent = m_Enemy.transform;
        transform.parent.localPosition = Vector3.zero+ m_StartPosOffset; 
    }

    
    void Update()
    {
        
    }
    void PS_Activate()//animation event 
    {
        transform.parent.parent = null; 
        m_PS.Play();
        Destroy(m_Enemy);
        Destroy(transform.parent.gameObject, m_DestroyTime); 
    }
}
