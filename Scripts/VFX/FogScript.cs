using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogScript : MonoBehaviour
{
    Renderer m_renderer; 
    public Transform m_Anchor;
    public Transform m_playerTransform;
    float m_DistancefromPlayer;
    public float m_MaxOpacity = 1.77f;
    public float m_OpacityLerpSpeed = 0.5f; 



    private void Awake()
    {
        m_renderer = GetComponent<Renderer>(); 
    }
    void Start()
    {
        m_DistancefromPlayer = transform.position.z - m_Anchor.position.z;
        m_renderer.material.SetFloat("_Opacity", 0); 
    }

    
    void Update()
    {
        //wait player to pass the transform then lerp in opacity and start keeping same distance with the player 
        
        if (m_playerTransform.position.z > m_Anchor.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y,m_playerTransform.position.z)
                + (Vector3.forward * m_DistancefromPlayer);


            m_renderer.material.SetFloat("_Opacity", 
                Mathf.Lerp(m_renderer.material.GetFloat("_Opacity"), m_MaxOpacity, m_OpacityLerpSpeed * Time.deltaTime));
        }
        
    }
}
