using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBombScript_Model_2 : MonoBehaviour
{
    
    
    public float m_ExplodingDelay = 0.2f;
    public float m_destroyTime = 3f;

    public float m_Sphere_Min_Size = 0.6f;
    public float m_sphere_Bounce_Speed = 2f;

    public AnimationCurve m_GrowingCurve;
    public float m_MaxSize = 10f; 
    public float m_TimeToGrow = 2.5f; 
    public float m_Timer = 0f; 

    public GameObject m_Sphere; 
    public ParticleSystem m_FireVortex_PS;
    public ParticleSystem m_BubbleBounce_PS; 
    public float m_TotargetSpeed = 0.1f;
    void Start()
    {
        Destroy(gameObject, m_destroyTime); 
        
    }

    // Update is called once per frame
    void Update()


    {
        m_Timer += Time.deltaTime;

        float scale = m_GrowingCurve.Evaluate(m_Timer/m_TimeToGrow);

        transform.localScale =  (Vector3.one * scale) * m_MaxSize;
        




        
        //moving
        m_Sphere.transform.localScale = Vector3.one *  ( (Mathf.Sin(Time.time * m_sphere_Bounce_Speed) /10 ) + m_Sphere_Min_Size) ;
        

    }

    private void FixedUpdate()
    {


        m_FireVortex_PS.transform.localScale = transform.localScale;
        m_BubbleBounce_PS.transform.localScale = transform.localScale; 

        if (!PlayerScript.s_Instance)
        {
            return; 
        }

        if (transform.position.z > PlayerScript.s_Instance.transform.position.z)// move towards player transform
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerScript.s_Instance.transform.position, m_TotargetSpeed);
        }
    }


    
}
