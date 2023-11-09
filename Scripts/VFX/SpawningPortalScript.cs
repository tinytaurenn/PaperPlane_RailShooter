using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPortalScript : MonoBehaviour
{
    Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyMe()
    {
        Destroy(this.gameObject); 
    }


}
