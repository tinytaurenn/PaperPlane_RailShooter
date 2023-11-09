using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPackScript : MonoBehaviour
{
   
    public ParticleSystem m_Collectable;
    Animator m_Animator; 


    private void Awake()
    {
        //m_Renderer = GetComponent<Renderer>();
        m_Animator =  GetComponent<Animator>();
    }

    
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") || other.CompareTag("Projectil"))
        {
            var player = PlayerScript.s_Instance;

            if (player.m_Life < GameManagerScript.s_Instance.m_MaxLife)
            {
                player.m_Life++;
                //player.HealLevel(player.m_Life, m);

                //player.m_HealVFXAnimator.Play(player.m_Healing);

            }



            Destroy(gameObject);

        }

        

        


        


    }

    

}
