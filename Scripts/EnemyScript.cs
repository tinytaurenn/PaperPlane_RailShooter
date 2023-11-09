using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public abstract class EnemyScript : MonoBehaviour
{


    public enum EEnemyState
    {
        Normal,
        Dodging,
        Attacking,
        Falling,
        Count
    }

    public EEnemyState currentState = EEnemyState.Normal;

    [Header("Enemy Properties")]

    public int m_EnemyLife = 3;

    Rigidbody m_rigidBody;
    [SerializeField]
    protected Animator m_Animator;
    protected Collider m_Collider;

    [SerializeField]
    Material m_BaseMaterial;
    [SerializeField]
    Material m_PaperMaterial;

    [SerializeField]
     protected AnimationCurve m_PaperAnimationCurve;

    [SerializeField]
    protected AnimationCurve m_PaperDesintegrateAnimationCurve;

    [SerializeField]
    protected SkinnedMeshRenderer m_Renderer; 

    public float m_FallingForce = 20f;
    public float m_X_AxisFallingMultiplier = 3f; 

    public ParticleSystem m_FallSmokeTrail;

    float randomXPos;

    int m_CoroutineCount = 0;
    int m_MaxCoroutineCount = 1; 



    [Header("Speed properties")]
    public float m_MinFollowSpeed = 1f;
    public float m_MaxFollowSpeed = 0.5f;


    public float m_RandomLerpTime;


    [Header("Shoot Properties")]

    [Range(0f, 3f)]
    public float m_DodgeSpeed = 0.3f;
    public float m_DodgeTime = 1f;
    public float m_DetectionRadius;

    [Space]
    [Range(0f, 30f)]
    public float m_EnemyShootCooldown = 5f;

    [Range(0f, 15f)]
    public float m_EnemyMinShootCooldown = 5f;
    public bool m_IsRandomEnemyShoot = false;


    [SerializeField] private float m_EnemyShootTime = 0f;




    [Header("Position Properties")]

    public float m_TimeBeforeMoving = 3f;
    [Space]

    [SerializeField] private float m_Formation_X_Axis_Position;
    [Range(0f, 20f)]
    private float m_RandomRange_Z_Position;

    [Header("Shader properties")]
    
    public float m_SpawnShadeTime = 2f;

    [Space]

    public GameObject m_ExplosionPrefab; 
    public ParticleSystem m_ExplosionParticleSystem;
    public float m_ExplosionPower = 2f;
    public float m_ExplodingTime = 3f;

    public virtual void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        //m_Renderer = GetComponent<Renderer>();
        //m_Animator = GetComponent<Animator>(); 
        m_Collider = GetComponent<Collider>();

        


    }
    public virtual void Start()
    {

        randomXPos = (Random.Range(-m_FallingForce, m_FallingForce))*m_X_AxisFallingMultiplier;

        //StartCoroutine(SpawningShader(m_SpawnShadeTime));

        m_EnemyShootTime += Time.time;

        //at enemy spawn, "chose" a random place in the Z axis and a sorted one in the X axis
        m_Formation_X_Axis_Position = SortedXAxisPosition();
        m_RandomRange_Z_Position = GameManagerScript.s_Instance.m_RandomRange_Z_Position;


        m_RandomRange_Z_Position = Random.Range(0, m_RandomRange_Z_Position);

        //start to move after a certain time
        Invoke("StartMoving", m_TimeBeforeMoving);

        //get a random Shoot Interval time in a certain range 
        if (m_IsRandomEnemyShoot) m_EnemyShootCooldown = Random.Range(m_EnemyMinShootCooldown, m_EnemyShootCooldown);



    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Movement();

    }
    public virtual void FixedUpdate()
    {
        Movement();




    }

    protected virtual void SwitchMaterialToPaper()
    {
        m_Renderer.material = m_PaperMaterial; 
    }




    void Movement()
    {
        switch (currentState)
        {
            case EEnemyState.Normal:
                KeepPosition();
                //ProjectilDetect();
                break;
            case EEnemyState.Dodging:
                Dodge();
                break;

            case EEnemyState.Attacking:

               
                //KeepPosition();

                AttackingFunction(); 



                break;


            case EEnemyState.Falling:

                FallingLerpPosition(randomXPos);

                break; 

            case EEnemyState.Count:

                
                break;
            default:
                break;
        }

        //KeepPosition();

    }
    //put simple force in forward and get a random lerp value


    protected virtual void  AttackingFunction()
    {
        KeepPosition();
        ShootOnCoolDown(m_EnemyShootCooldown);

    }

    void StartMoving()
    {
        Vector3 dir = new Vector3(0, 0, 1);
        m_rigidBody.AddForce(dir * PlayerScript.s_Instance.m_Rigidbody.velocity.z, ForceMode.VelocityChange);
        m_RandomLerpTime = Random.Range(m_MinFollowSpeed, m_MaxFollowSpeed);
    }
    //keeps the enemy on a certain range from the player with its transform 
    public void KeepPosition()
    {

        ProjectilDetect();
        //change state to attack if on cooldown
        if (currentState == EEnemyState.Normal)
        {
            if (Time.time > m_EnemyShootTime + m_EnemyShootCooldown)
            {
                currentState = EEnemyState.Attacking;
            }
        }


        Vector3 nextPos = new Vector3(
            m_Formation_X_Axis_Position,
            PlayerScript.s_Instance.transform.position.y,
            GameManagerScript.s_Instance.m_EnemySpawnTransform.position.z + m_RandomRange_Z_Position);

        



        transform.position = Vector3.Lerp(transform.position, nextPos, m_RandomLerpTime * Time.deltaTime);


    }
    //create a new pos to lerp when destroyed by the player
    //deprecated
    void FallingLerpPosition(float randomXPos)
    {
        


        Vector3 nextPos;

        nextPos = new Vector3(
                m_Formation_X_Axis_Position + randomXPos,
                PlayerScript.s_Instance.transform.position.y - m_FallingForce,
                GameManagerScript.s_Instance.m_EnemySpawnTransform.position.z + m_RandomRange_Z_Position);

        transform.position = Vector3.Lerp(transform.position, nextPos, m_RandomLerpTime * Time.deltaTime);

    }
    // sort X position of enemies depending of the X spawn range and the number of enemy in the wave 

    float SortedXAxisPosition()
    {
        int formationNumber;
        float formationRange;
        if (GameManagerScript.s_Instance.m_WaveCount < GameManagerScript.s_Instance.waveList.Count)
        {
            formationNumber = GameManagerScript.s_Instance.waveList[GameManagerScript.s_Instance.m_WaveCount - 1].enemyNumber;
            formationRange = GameManagerScript.s_Instance.m_EnemyXSpawnRange
           * GameManagerScript.s_Instance.waveList[GameManagerScript.s_Instance.m_WaveCount - 1].spawnRange;
        }
        else
        {
            formationNumber = GameManagerScript.s_Instance.m_EnemySpawnNumber;
            formationRange = GameManagerScript.s_Instance.m_EnemyXSpawnRange;

        }


        int inGameNumber = GameManagerScript.s_Instance.m_IngameEnemyList.Count;



        if (inGameNumber >= formationNumber && formationNumber % 2 == 1)
        {
            return GameManagerScript.s_Instance.m_EnemySpawnTransform.position.x;  ; //if wave number is uneven , the last enemy to spawn  will be the center of the wave
        }

        int halfNumber = (int)formationNumber / 2;
        float xPos =  formationRange / halfNumber;
        if (inGameNumber > halfNumber)
        {
            xPos *= halfNumber - inGameNumber;
            xPos += GameManagerScript.s_Instance.m_EnemySpawnTransform.position.x; 
            return xPos;
        }
        else
        {
            xPos *= inGameNumber;
            xPos += GameManagerScript.s_Instance.m_EnemySpawnTransform.position.x;
            return xPos;
        }










    }
    //detect an ally projectil incoming in a certain radius 
    public virtual void ProjectilDetect()
    {
        if (currentState == EEnemyState.Falling)
        {
            return; 
        }

        LayerMask mask = LayerMask.GetMask("Projectil");

        if (Physics.CheckSphere(transform.position, m_DetectionRadius, mask))
        {
            Collider[] DetectedObjetList = Physics.OverlapSphere(transform.position, m_DetectionRadius, mask);



            currentState = EEnemyState.Dodging;

        }



    }
    //if the projectil is the player projectil, run the dodge coroutine depending on the first detected projectil
    private void Dodge()
    {
        LayerMask mask = LayerMask.GetMask("Projectil");
        Collider[] DetectedObjetList = Physics.OverlapSphere(transform.position, m_DetectionRadius, mask);


        if (DetectedObjetList.Length > 0)
        {
            Vector3 projectilPosition = DetectedObjetList[0].GetComponent<Transform>().position;
            StartCoroutine(Dodge(mask, projectilPosition));
        }





    }

    // dodge by reverse towarding the projetil then return to normal state after a certain time 
    IEnumerator Dodge(LayerMask mask, Vector3 projectilPosition)
    {




        projectilPosition = new Vector3(projectilPosition.x, transform.position.y, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, projectilPosition, -m_DodgeSpeed);

        yield return new WaitForSeconds(m_DodgeTime);

        if(currentState != EEnemyState.Falling) currentState = EEnemyState.Normal;
    }

    protected abstract void Shooting();//depend on the enemy type 


    //shoot only if cooldown is respected
    void ShootOnCoolDown(float cooldown)
    {


        if (Time.time > m_EnemyShootTime + cooldown)
        {

            Shooting();
            m_EnemyShootTime = Time.time;

        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        print("enemy trigger enter");
        if (other.CompareTag("Projectil"))
        {

            int damage = other.gameObject.GetComponent<projectilScript>().m_Damage; 
            m_EnemyLife -= damage;
            if (m_EnemyLife <= 0)
            {
                
                
                GameManagerScript.s_Instance.m_IngameEnemyList.Remove(gameObject);
                //currentState = EnemyState.Falling;

                if (m_CoroutineCount >= m_MaxCoroutineCount)
                {
                    print("The exploding coroutine tried to be called multiples times");
                    return; 
                }
                StartCoroutine(Exploding(m_ExplodingTime));

            }
        }
    }

    IEnumerator SpawningShader(float time)
    {
        float i = 1;
        float rate = 1 / time;
        while (i > -1)
        {
            m_Renderer.material.SetFloat("_Clipping_Threshold", i);



            i -= Time.fixedDeltaTime * rate;


            yield return 0;



        }
    }
    //exploding coroutine, end with the gameobject destroy, trigger the VFX and set their values 

    public IEnumerator Nuked(float time)
    {

        print("run nuked");
        m_CoroutineCount++;
        if (!PlayerScript.s_Instance.m_IsGodMode)
        {
            GameManagerScript.s_Instance.m_PlayerScore++;
        }

        Vector3 randomRot = new Vector3(
            m_ExplosionPrefab.transform.rotation.x,
            m_ExplosionPrefab.transform.rotation.y,
            Random.Range(0, 360));



        float i = 0;
        float rate = 1 / time;








       
        GameObject FinalSlash = Instantiate(m_ExplosionPrefab, transform.position, m_ExplosionPrefab.transform.rotation);
        FinalSlash.GetComponentInChildren<ScissorVFX_Script>().m_Enemy = this.gameObject;
        //Destroy (FinalSlash, time);
        yield return 0;



        if (i >= 1)
        {
            m_Renderer.enabled = false;
            yield return new WaitForSeconds(1);
            m_ExplosionParticleSystem.gameObject.SetActive(false);
            Destroy(gameObject);
        }


        //m_FallSmokeTrail.Play(); 
        yield return new WaitForSeconds(3);

        //currentState = EnemyState.Falling;




        yield return new WaitForSeconds(2);

        Destroy(gameObject);


        m_CoroutineCount--;
    }
    IEnumerator Exploding(float time)
    {

        m_Collider.enabled = false;
        
        m_CoroutineCount++;
        if (!PlayerScript.s_Instance.m_IsGodMode)
        {
            GameManagerScript.s_Instance.m_PlayerScore++;
        }

        Vector3 randomRot = new Vector3(
            m_ExplosionPrefab.transform.rotation.x,
            m_ExplosionPrefab.transform.rotation.y,
            Random.Range(0, 360));



        //float i = 0;
        //float rate = 1 / time;






        StartCoroutine(ExplodingPaper()); 

        //GameObject FinalSlash = Instantiate(m_ExplosionPrefab, transform.position, Quaternion.Euler(randomRot), transform);
        //GameObject FinalSlash = Instantiate(m_ExplosionPrefab, transform.position, m_ExplosionPrefab.transform.rotation);
        //FinalSlash.GetComponentInChildren<ScissorVFX_Script>().m_Enemy = this.gameObject; 
        ////Destroy (FinalSlash, time);
        //yield return 0; 


        
        //if (i >= 1)
        //{
        //    m_Renderer.enabled = false;
        //    yield return new WaitForSeconds(1);
        //    m_ExplosionParticleSystem.gameObject.SetActive(false);
        //    Destroy(gameObject);
        //}

        
        //m_FallSmokeTrail.Play(); 
        yield return new WaitForSeconds(3);

        //currentState = EnemyState.Falling;




        yield return new WaitForSeconds(2);

        Destroy(gameObject);


        m_CoroutineCount--; 

    }

    protected virtual IEnumerator PaperDesintegrate(float time)
    {
        float i = 0;
        float rate = 1 / time;

        while (i < 1)
        {
            m_Renderer.material.SetFloat("_Value", m_PaperDesintegrateAnimationCurve.Evaluate(i));
            i += Time.deltaTime * rate;
            yield return 0;
        }

        yield return 0;
    }

    IEnumerator ExplodingPaper()
    {

        

        m_CoroutineCount++;
        if (!PlayerScript.s_Instance.m_IsGodMode)
        {
            GameManagerScript.s_Instance.m_PlayerScore++;
        }

        SwitchMaterialToPaper();
        m_Animator.speed = 0; 
        StartCoroutine(PaperScretch(1));

       

        currentState = EEnemyState.Falling;

        StartCoroutine(PaperDesintegrate(3)); 

        
        m_rigidBody.useGravity = true;
        m_rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        m_MinFollowSpeed = 0f; 
        m_MaxFollowSpeed = 0f;
        m_RandomLerpTime = 0f;
        



        yield return 0; 
    }


    protected virtual IEnumerator PaperScretch(float time)
    {

        float i = 0;
        float rate = 1 / time;

        while (i < 1)
        {
            m_Renderer.SetBlendShapeWeight(0, m_PaperAnimationCurve.Evaluate(i));
            i += Time.deltaTime * rate;
            yield return 0;
        }

        yield return 0;



    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRadius);





    }
    private void OnDestroy()
    {

        if (GameManagerScript.s_Instance == null)
        {
            return; 
        }

        if (GameManagerScript.s_Instance.m_IngameEnemyList.Count == 0)
        {

            //GameManagerScript.s_Instance.m_Time = Time.time;
        }

        GameManagerScript.s_Instance.m_IngameEnemyList.Remove(this.gameObject);
    }



}
