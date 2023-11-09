using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//there is no use of this script anymore 
public class RefillPackScript : MonoBehaviour
{
    public int m_RefillAmount = 20;


    Animator m_Animator;
    public Animator m_BoxAnimator;

    private int m_BoxBreakingAnimation = Animator.StringToHash("Boxbreaking");


    Renderer m_Renderer;
    public ParticleSystem m_ParticleSystem;

    private bool m_IsDestroyed = false;



    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();

        m_Animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Projectil"))
        {
            if (m_IsDestroyed)
            {
                return;
            }

            var player = PlayerScript.s_Instance;

            player.TriggerRefillAnimation();

            m_IsDestroyed = true;

            if (player.m_GaugeValue < CanvasScript.s_Instance.m_Slider.maxValue)
            {
                player.m_GaugeValue += m_RefillAmount;
            }


            m_BoxAnimator.enabled = true;
            m_Animator.Play(m_BoxBreakingAnimation);

            Destroy(gameObject, 1f);

        }



    }
    private void FixedUpdate()
    {



    }
}
