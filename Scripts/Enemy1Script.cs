using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;



public class Enemy1Script : EnemyScript
{
    //enemy droping "bombs"

    [Header("Enemy Properties")]

    [SerializeField]
    Material m_WingsBaseMat;
    [SerializeField]
    Material m_PaperWingsMat;

    [SerializeField]
    SkinnedMeshRenderer m_WingsRenderer;

    [SerializeField]
    GameObject m_InnerPortalGO; 


    [Header("Shoot properties")]

    public Transform m_EnemyShootAnchor;
    public GameObject m_Bomb;
    public ParticleSystem m_BombSpawn_PS;


    [Space]
    public GameObject m_ProjectilPrefab;
    public Transform m_ShootAnchor;
    public float m_MissileNumber = 2;

    [Space]
    public int m_ShootNumber;
    public float m_ShootDelay;

    [Header("RoamingShield properties")]
    public GameObject m_RoamingShieldGameObject;
    public int m_RoamingShieldSpawnNumber = 3;
    public Transform m_RoamingShieldSpawnPoint;
    public Transform m_RoamingShieldDefendPoint;

    public List<GameObject> m_RoamingShieldList;

    public float m_ShieldDetectionDistance = 20f;
    public float m_DetectionAngle = 25f;
    public LayerMask m_ProjectilMask;

    public Collider[] projectils;




    [Header("Shield Properties")]

    //public int m_ShieldLife = 3;
    public float m_TimeBeforeShieldActivation = 2f;

    private float m_TimeOnSpawn;
    [Space]
    public Renderer m_ShieldRenderer;
    public Collider m_ShieldCollider;
    public GameObject m_ShieldGameObject;
    [Space]
    Color m_initialColor;
    public Color m_DamagedColor;

    [Range(0.1f, 1f)]
    public float m_DamagedColorTime = 0.1f;




    public override void Awake()
    {


        base.Awake();
    }
    public override void Start()
    {
        base.Start();
        m_TimeOnSpawn = Time.time;

        //m_initialColor = m_ShieldRenderer.material.GetColor("_Fresnel_Color");

        StartCoroutine(SpawningShieldWithTime(m_TimeBeforeShieldActivation));

        //SpawningShieldProgram(m_RoamingShieldSpawnNumber); 
    }

    // Update is called once per frame
    public override void Update()
    {

        base.Update();
        //if(currentState == EnemyState.Attacking) base.KeepPosition(); 

        ProjectilDetection(m_ShieldDetectionDistance);

        m_ShootAnchor.transform.LookAt(PlayerScript.s_Instance.transform.position);




    }

    protected override void SwitchMaterialToPaper()
    {
        base.SwitchMaterialToPaper();

        m_WingsRenderer.material = m_PaperWingsMat; 


    }

    protected override IEnumerator PaperScretch(float time)
    {

        foreach (var item in m_RoamingShieldList)
        {
            Destroy(item);
        }

        m_Animator.speed = 1f;
        m_Animator.SetTrigger("Scruntch"); 

        m_InnerPortalGO.SetActive(false);

        float i = 0;
        float rate = 1 / time;

        while (i < 1)
        {
            m_Renderer.SetBlendShapeWeight(0, m_PaperAnimationCurve.Evaluate(i));
            m_WingsRenderer.SetBlendShapeWeight(0, m_PaperAnimationCurve.Evaluate(i));
            i += Time.deltaTime * rate;

            yield return 0;
        }

        
    }

    protected override IEnumerator PaperDesintegrate(float time)
    {
        float i = 0;
        float rate = 1 / time;

        while (i < 1)
        {
            m_Renderer.material.SetFloat("_Value", m_PaperDesintegrateAnimationCurve.Evaluate(i));
            m_WingsRenderer.material.SetFloat("_Value", m_PaperDesintegrateAnimationCurve.Evaluate(i));
            i += Time.deltaTime * rate;
            yield return 0;
        }

        yield return 0;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //if (Time.time > m_TimeBeforeShieldActivation + m_TimeOnSpawn)
        //{
        //    m_ShieldRenderer.gameObject.SetActive(true);
        //}


        //if (m_ShieldLife <= 0)
        //{
        //    m_ShieldRenderer.gameObject.SetActive(false);
        //}




    }

    void SpawningShieldProgram(int spawnNumber)
    {

        for (int i = 0; i < spawnNumber; i++)
        {



            GameObject shield = Instantiate(m_RoamingShieldGameObject, m_RoamingShieldSpawnPoint.position, m_RoamingShieldGameObject.transform.rotation);
            m_RoamingShieldList.Add(shield);
            var ShieldObject = shield.GetComponent<RoamingShieldScript>();
            ShieldObject.m_Focus = transform;
            ShieldObject.m_PointToDefend = m_RoamingShieldDefendPoint;
            ShieldObject.m_TimeOffSet = i;

        }


    }

    //detect incoming projectils and switch state if closer enough to the focus to protect
    void ProjectilDetection(float detectDistance)
    {
        if (m_RoamingShieldList.Count <= 0)
        {
            //if no shield, no function
            return;
        }

        if (!Physics.CheckSphere(transform.position, detectDistance, m_ProjectilMask))
        {

            //stop the function and reset all shields state to "roaming" if there is no projectils around the detectDistance
            //print("Return roaming");

            foreach (var item in m_RoamingShieldList)
            {
                item.GetComponent<RoamingShieldScript>().SwitchState(RoamingShieldScript.EState.Roaming);
            }

            return;
        }

        //create a list with every projectils in the detectDistance
        projectils = Physics.OverlapSphere(transform.position, detectDistance, m_ProjectilMask);


        if (projectils.Length > 0)
        {

            foreach (var item in m_RoamingShieldList)
            {
                //stop the function here if there is already a shield in a defending state 

                if (item.TryGetComponent<RoamingShieldScript>(out RoamingShieldScript roamingShield))
                {
                    if (roamingShield.currentState == RoamingShieldScript.EState.Defending)
                    {
                        return;
                    }
                }
                



            }
            //stop the function here if there is no projectil in the field of view with the detectionAngle
            if (!IsThereFOVInList(projectils, m_DetectionAngle))
            {
                return;
            }





            //randomly select a shield around the ship and put it in defending state 
            // in defending state, the shield get bigger and put itself on the defending point transform

            int randShieldNumber = UnityEngine.Random.Range(0, m_RoamingShieldList.Count - 1);

            m_RoamingShieldList[randShieldNumber].GetComponent<RoamingShieldScript>().SwitchState(RoamingShieldScript.EState.Defending);

            print("defending");



        }




    }
    /*/
     * check if the target is in the fielw of view using the backward direction of the transform
     */

    bool FOV(float angle, Vector3 targetPos)
    {
        float dot = Vector3.Dot(-transform.forward.normalized, (targetPos - transform.position).normalized);
        float dotInDeg = Mathf.Acos(dot) * Mathf.Rad2Deg;


        if (dotInDeg <= angle)
        {
            return true;
        }


        return false;

    }

    //return true if one of the list's colliders is in the field of view
    bool IsThereFOVInList(Collider[] list, float angle)
    {
        foreach (var item in list)
        {
            if (FOV(angle, item.transform.position))
            {
                print("there is proj in fov");
                return true;
            }


        }

        return false;
    }

    IEnumerator SpawningShieldWithTime(float time)
    {
        yield return new WaitForSeconds(time);

        SpawningShieldProgram(m_RoamingShieldSpawnNumber);



    }


    //when damage is taken, the shield will glow a little more
    //deprecated
    IEnumerator ShieldTakingDamage(float damageAmount, float time)
    {

        m_ShieldRenderer.material.SetColor("_Global_Color", m_DamagedColor);
        m_ShieldRenderer.material.SetColor("_Fresnel_Color", m_DamagedColor);

        yield return new WaitForSeconds(time);

        m_ShieldRenderer.material.SetColor("_Global_Color", m_initialColor);
        m_ShieldRenderer.material.SetColor("_Fresnel_Color", m_initialColor);
    }
    protected override void Shooting()
    {
        //Instantiate(m_Bomb, m_EnemyShootAnchor.position, m_Bomb.transform.rotation);

        //if (m_BombSpawn_PS != null)
        //{
        //    m_BombSpawn_PS.Play();
        //}


        //StartCoroutine(ShootCoroutine(m_ShootDelay, m_ShootNumber));

        ShootAnim(); 


    }

    void ShootAnim()
    {
        m_Animator.SetTrigger("Shoot"); 
    }

    void ShootEvent()
    {

        Shoot(); 
    }

    IEnumerator ShootCoroutine(float time, float shootNumber)
    {
        for (int i = 0; i < shootNumber; i++)
        {
            Shoot();
            yield return new WaitForSeconds(time);
        }
    }

    void Shoot()
    {
        if (m_EnemyLife <= 0) return; 

        for (int i = 0; i < m_MissileNumber; i++)
        {
            Vector3 pos = m_ShootAnchor.position + new Vector3(i - (m_MissileNumber / 2) + 0.5f, 0, 0);
            Instantiate(m_ProjectilPrefab, pos, m_ShootAnchor.rotation);
            m_BombSpawn_PS.Play();

        }
    }

    //the shield is taking over the damages then destroy itself. 
    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectil"))
        {
            base.OnTriggerEnter(other);//projectils start to hit the enemy life

            //if (m_ShieldLife <= 0)
            //{


            //}
            //else
            //{
            //    StartCoroutine(ShieldTakingDamage(1f, m_DamagedColorTime));
            //    m_ShieldLife--;
            //}
        }






    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_EnemyShootAnchor.position, 1f);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, m_ShieldDetectionDistance);


        Gizmos.color = Color.green;


    }

    private void OnDestroy()
    {
        

        foreach (var item in m_RoamingShieldList)
        {
            Destroy(item);
        }
    }

}
