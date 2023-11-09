using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//there is no use of this script anymore
public class ProjetilTrailScript : MonoBehaviour
{
    public float m_RoamingSpeed;
    public float m_DistanceFromFocus;
    public float m_LerpSpeed; 
    public Transform m_Focus; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Orbit()
    {
        float x = Mathf.Cos(Time.time +  m_RoamingSpeed) * m_DistanceFromFocus;
        float y = Mathf.Sin(Time.time +   m_RoamingSpeed) * m_DistanceFromFocus;
        Vector3 targetPosition = new Vector3(x, y, 0) + m_Focus.position;

        //transform.position = targetPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, m_LerpSpeed * Time.deltaTime);
    }
}
