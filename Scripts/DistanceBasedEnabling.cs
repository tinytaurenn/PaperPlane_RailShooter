using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]


public class DistanceBasedEnabling : MonoBehaviour
{
    [SerializeField]
    float m_Distance = 50f;
    [SerializeField]
    Transform m_ViewTransform;
    [SerializeField]
    MeshRenderer m_Renderer; 
    
    void Update()
    {

        if (m_ViewTransform == null) return;

        if (Vector3.Distance(transform.position, m_ViewTransform.position) < m_Distance)
        {
            m_Renderer.enabled = true;
        }
        else
        {
            m_Renderer.enabled = false; 
        }
        
    }
}
