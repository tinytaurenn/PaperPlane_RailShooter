using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakingScript : MonoBehaviour
{
    
    public AnimationCurve m_Curve;
    public bool m_IsCameraShaking = false;

    public Vector3 m_StartPos;

    public float m_LerpSpeed = 2f;
    public float m_Delay = 0.1f; 
    Vector3 m_NewPos;
    float m_Time = 0f; 

    void Start()
    {
        m_StartPos = transform.position; 
        m_Time = Time.time;
        m_NewPos = m_StartPos + new Vector3(Random.insideUnitSphere.x, Random.insideUnitSphere.y, 0);
    }

    
    void Update()
    {
       
    }
    private void FixedUpdate()
    {

        if (!m_IsCameraShaking)
        {
            return;
        }
        float power = m_Curve.Evaluate(Mathf.Abs(Mathf.Sin(Time.time)));
        //print(Mathf.Sin(Time.time));

        if (m_Time + m_Delay <= Time.time)
        {
            Vector3 startPos = new Vector3(transform.position.x, m_StartPos.y, transform.position.z);
            m_NewPos = startPos + new Vector3(0, Random.insideUnitSphere.y, Random.insideUnitSphere.z) * power;
            //transform.position = m_NewPos; 
            m_Time = Time.time;
        }

       

        transform.position = Vector3.Lerp(transform.position, m_NewPos, m_LerpSpeed * Time.deltaTime);
        

    }
}
