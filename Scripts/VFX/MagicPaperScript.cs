using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPaperScript : MonoBehaviour


{

    public GameObject m_ExplosionParticleSystem;
    Animator m_Animator;
    public Transform m_PlayerTransform;
    public Transform m_MagicPaperPosOnPlayer;

    public float m_enemyDestroyTimer = 2f;
    public float m_MagicPaperDestroyTime = 6f;

    public float m_LerpSpeed = 0.8f;
    public float m_ActivationDistanceFromPlayer = 10f;

    public float m_SpaceForwardEnemy = 3f;


    public enum EState
    {
        Idle,
        Escort
    }

    public EState currentState = EState.Idle;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

    }
    void Start()
    {
        if (PlayerScript.s_Instance == null)
        {
            return;
        }
        m_PlayerTransform = PlayerScript.s_Instance.transform;
        m_MagicPaperPosOnPlayer = PlayerScript.s_Instance.m_MagicPaperPosOn;
    }


    void Update()
    {
        if (m_PlayerTransform == null)
        {
            return;
        }

        switch (currentState)
        {
            case EState.Idle:
                float distanceFromPlayer = Vector3.Distance(transform.position, m_PlayerTransform.position);

                if (distanceFromPlayer <= m_ActivationDistanceFromPlayer)
                {
                    //switchstate to trigger animation and effect
                    Activation();
                }



                break;
            case EState.Escort:



                break;
            default:
                break;
        }




    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case EState.Idle:
                break;
            case EState.Escort:

                transform.position = Vector3.Lerp(transform.position, m_MagicPaperPosOnPlayer.position, m_LerpSpeed * Time.deltaTime);


                break;
            default:
                break;
        }
    }

    void Activation()
    {
        currentState = EState.Escort;
        m_Animator.enabled = true;

        Destroy(this.gameObject, m_MagicPaperDestroyTime);
    }
    //select all alive enemies and destroy them just after the effect
    void Explosion()
    {
        const float delayBeforeEnemyDestroy = 0.1f; 
        print("StarExplosion");

        foreach (var enemy in GameManagerScript.s_Instance.m_IngameEnemyList)
        {
            print (enemy.name);
            //Vector3 pos = enemy.transform.position + -Vector3.forward * m_SpaceForwardEnemy;
            //GameObject PS = Instantiate(m_ExplosionParticleSystem, pos, m_ExplosionParticleSystem.transform.rotation);
            //PS.transform.parent = enemy.transform;
            //PS.transform.localPosition = Vector3.forward * m_SpaceForwardEnemy;

            //StartCoroutine(UnParenting(PS, m_enemyDestroyTimer - delayBeforeEnemyDestroy));

            StartCoroutine( enemy.GetComponent<EnemyScript>().Nuked(1)); 

        }


        DestroyEnemies();


    }

    void DestroyEnemies()
    {
        foreach (var enemy in GameManagerScript.s_Instance.m_IngameEnemyList)
        {

            Destroy(enemy, m_enemyDestroyTimer);

        }

        GameManagerScript.s_Instance.m_IngameEnemyList.Clear(); //brutal but efficient
    }

    IEnumerator UnParenting(GameObject PS, float time)
    {
        yield return new WaitForSeconds(time);
        PS.transform.parent = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_ActivationDistanceFromPlayer);
    }
}
