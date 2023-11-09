using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningEffectScript : MonoBehaviour
{
    public Renderer m_PlayerRenderer; 
    public GameObject m_PaperToCenter_PS;
    public GameObject m_Explosion_PS;
    public float m_PaperToCenterTime = 5f;

    public float m_Destroytime = 6f;

    
    



    private void Awake()
    {
        
    }
    void Start()
    {
        Destroy(transform.gameObject, m_Destroytime); 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RendererEnable()
    {
        m_PlayerRenderer.enabled = true; 
    }

    //avoid player to move or shoot while spawning
    void MovementEnable()
    {
        transform.parent.GetComponent<PlayerScript>().m_CanMove = true; 
        transform.parent.GetComponent<PlayerScript>().m_CanShoot = true; 
    }
}
