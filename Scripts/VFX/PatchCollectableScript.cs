using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PatchCollectableScript : MonoBehaviour
{
    Transform m_PlayerTransform;

    [SerializeField]
    float m_DistanceFromPlayerToActivate = 50f;

    [SerializeField]
    AnimationCurve m_FadeInAnimationCurve; 
    [SerializeField]
    AnimationCurve m_FadeOutAnimationCurve;
    [SerializeField]
    float m_FadeTime = 3f; 

    bool m_IsEnabled = false; 



    [SerializeField]
    float m_Speed = 5f;

    [SerializeField]
    enum EDirection
    {
        right,
        left
    }

    [SerializeField]
    EDirection m_Direction;

    MaterialPropertyBlock m_MaterialPropertyBlock;

    Renderer m_Renderer;


    private void Awake()
    {
            m_Renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_PlayerTransform = PlayerScript.s_Instance.transform;



        if (m_Direction == EDirection.right)
        {

            m_MaterialPropertyBlock.SetFloat("_Speed", -m_Renderer.material.GetFloat("_Speed"));
            m_Renderer.SetPropertyBlock(m_MaterialPropertyBlock);

        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, m_PlayerTransform.position) < m_DistanceFromPlayerToActivate)
        {
            Movement();


            if (m_IsEnabled == false)
            {
                m_IsEnabled = true;
                StartCoroutine(Fade(m_FadeTime, m_FadeInAnimationCurve)); 
            }
        }

    }

    void Movement()
    {
        switch (m_Direction)
        {
            case EDirection.right:

                transform.position += m_Speed * Time.deltaTime * transform.right;
                

                break;
            case EDirection.left:

                transform.position += m_Speed * Time.deltaTime * (-transform.right);
                break;
            default:
                break;
        }
    }

    IEnumerator Fade(float time, AnimationCurve curve)
    {
        float i = 0;
        float rate = 1 / time;

        while (i < 1)
        {

            m_MaterialPropertyBlock.SetFloat("_GlobalOpacity", curve.Evaluate(i));

            m_Renderer.SetPropertyBlock(m_MaterialPropertyBlock);

            i += rate * Time.deltaTime; 

            yield return 0; 
        }
        m_MaterialPropertyBlock.SetFloat("_GlobalOpacity", curve.Evaluate(1));

        m_Renderer.SetPropertyBlock(m_MaterialPropertyBlock);

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") || other.CompareTag("Projectil"))
        {
            print("PatchHeal Trigger"); 

            var player = PlayerScript.s_Instance;

            if (player.m_Life < GameManagerScript.s_Instance.m_MaxLife)
            {
                player.m_Life++;
                player.HealLevel(player.m_Life, m_Renderer.material.GetColor("_Color")); ;

                //player.m_HealVFXAnimator.Play(player.m_Healing);

            }


            StartCoroutine(Fade(m_FadeTime,m_FadeOutAnimationCurve));
            Destroy(gameObject, m_FadeTime);

        }









    }
}
