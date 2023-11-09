using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Script : EnemyScript
{
    //classic shooter enemy

    [Header("Shoot Properties")]
    public float m_TimeBeforeAttack = 2f;
    [Range(1f, 10f)]
    public float m_RushToTargetSpeed = 2f;

    [SerializeField]
    GameObject m_Hit; 

    [SerializeField]
    float m_DistanceToDestroy = 1f; 

    [Space]
    public GameObject m_ProjectilPrefab;
    public Transform m_ShootAnchor;
    public float m_MissileNumber = 2;

    [Space]
    public int m_ShootNumber;
    public float m_ShootDelay;

    [SerializeField]
    Transform m_PlayerTransform;

    int m_AttackCoroutineCount = 0;

    Coroutine m_AttackRoutine; 

    [SerializeField]
    TrailRenderer m_WindTrailRenderer;
    [SerializeField]
    AnimationCurve m_WindTrailCurve;
    [SerializeField]
    AnimationCurve m_WindTrailSpeedCurve;

     

    MaterialPropertyBlock m_MaterialPropertyBlock; 
    MaterialPropertyBlock m_OpacityMaterialPropertyBlock; 




    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();

        StartCoroutine(SpawnRoutine(m_TimeBeforeAttack)); 

        m_PlayerTransform = PlayerScript.s_Instance.transform; 
    }

    
    public override void Update()
    {
        base.Update();

        if (m_EnemyLife <= 0)
        {
            if (m_AttackRoutine != null)
            {
                StopCoroutine(m_AttackRoutine);
            }
            currentState = EEnemyState.Normal; 
            
        }
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //rotate towards player
        Quaternion newRotation = Quaternion.LookRotation(PlayerScript.s_Instance.transform.position - transform.position);


        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.1f);
    }


    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {

            Instantiate(m_Hit, other.ClosestPoint(transform.position),m_Hit.transform.rotation) ; 

            Destroy(this.gameObject,m_DistanceToDestroy); 
        }

        if (m_EnemyLife <= 0)
        {
            m_WindTrailRenderer.enabled = false; 
        }
    }

    protected override void AttackingFunction()
    {
        //base.AttackingFunction();

        if (m_AttackCoroutineCount<1)
        {
            m_AttackRoutine =  StartCoroutine(AttackRoutine(m_DistanceToDestroy));
        }

        
        
    }

    IEnumerator AttackRoutine(float time)
    {
        print("attackroutine launched"); 

        m_AttackCoroutineCount++;
        float i = 0;
        float rate = 1 / time;

        StartCoroutine(AggroColor(m_TimeBeforeAttack)); 

        yield return new WaitForSeconds(m_TimeBeforeAttack); 



        Vector3 pos = m_PlayerTransform.position;


        while (i < 1)
        {

            transform.position = Vector3.Lerp(transform.position, pos, m_RushToTargetSpeed * Time.deltaTime);

            i += rate * Time.deltaTime; 
            yield return 0; 
        }

        Destroy(gameObject, m_DistanceToDestroy);
       

        

       
    }

    IEnumerator SpawnRoutine(float time)
    {
        float i = 0f;
        float rate = 1f / time;


        m_OpacityMaterialPropertyBlock = new MaterialPropertyBlock();

        while (i < 1)
        {
            

            m_OpacityMaterialPropertyBlock.SetFloat("_GlobalOpacity", i);

            m_WindTrailRenderer.SetPropertyBlock(m_OpacityMaterialPropertyBlock);
            i += rate * Time.deltaTime;

            yield return 0f;
        }

    }

    IEnumerator AggroColor(float time)
    {
        float i = 0f; 
        float rate = 1f / time;

        m_MaterialPropertyBlock = new MaterialPropertyBlock();

        while (i < 1)
        {
            m_MaterialPropertyBlock.SetFloat("_DangerSlider", m_WindTrailCurve.Evaluate(i)); 
            //m_Animator.speed = m_WindTrailSpeedCurve.Evaluate(i);
            m_Animator.SetFloat("SpeedMultiplier", m_WindTrailSpeedCurve.Evaluate(i)); 
            

            m_WindTrailRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
            i += rate * Time.deltaTime;



            yield return 0f; 
        }
    }

    protected override void Shooting()
    {


        StartCoroutine(ShootCoroutine(m_ShootDelay, m_ShootNumber));
    }
    // shoot a number of projectil and align them
    void Shoot()
    {
        for (int i = 0; i < m_MissileNumber; i++)
        {          
            Vector3 pos = m_ShootAnchor.position + new Vector3(i - (m_MissileNumber / 2) + 0.5f, 0, 0);
            Instantiate(m_ProjectilPrefab, pos, m_ShootAnchor.rotation);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_ShootAnchor.position, 1f);
    }
    //shoot multiples time the same amount of projectil
    IEnumerator ShootCoroutine(float time, float shootNumber)
    {
        for (int i = 0; i < shootNumber; i++)
        {
            Shoot();
            yield return new WaitForSeconds(time);
        }
    }

}
