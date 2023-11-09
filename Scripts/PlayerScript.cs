using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody m_Rigidbody;
    public Animator m_Animator;
    public Renderer m_Renderer;
    //public ParticleSystem m_ParticleSystem;
    public ParticleSystem m_ExplosionParticleSystem;
    //public LineRenderer m_ReactorLine;

    public LineRenderer[] m_ReactorLineList;


    [Header("Animator Properties")]

    private int m_Exploding = Animator.StringToHash("PlayerDestructionAnimation");

    public int m_Healing = Animator.StringToHash("Heal_VFX");

    private int m_OverHeatingAnimation = Animator.StringToHash("OverHeating");

    private int m_RefillAnimation = Animator.StringToHash("RefillAnimation");



    [Header("Player properties")]
    public float m_RotationForce = 35f;
    public float m_RotationSlerpSpeed = 0.5f;

    public float m_Speed = 10;

    public float m_SideStopForceMultiplier = 2f;
    public float m_SideForceMultiplier = 8f;


    public float m_MinSpeed = 20.0f;
    public float m_MaxSpeed = 90.0f;
    [Space]

    int m_MaxLife = 3; 
    public int m_Life = 1;

    [Space]
    public bool m_IsGodMode = false;
    public bool m_CanMove = false;
    public float m_GodModeDelay = 0.2f;
    public int m_GodModeBlinkNumber = 10;

    [SerializeField]
    AnimationCurve m_GodModeAnimationCurve; 




    [Header("Shoot properties")]
    public bool m_CanShoot = false; 

    public Transform m_ShootAnchor;
    public Transform m_ShootVFXSpawnAnchor;
    public GameObject m_ProjectilPrefab;
    public GameObject m_ProjectilPrefab_2;
    public GameObject m_ProjectilSpawnPrefab;
    [Space]
    public bool m_IsAngle;
    public bool m_IsRandomAngle;
    public float m_AngleConeSize = 5f;
    [Space]
    public float m_MissileNumber = 3f;

    public float m_SpaceBetweenMissile = 10f;
    [Space]
    public float m_AngleMissileNumber = 3f;

    public float m_AngleSpaceBetweenMissile = 10f;

    [Header("Gauge Properties")]
    [Range(0f, 100f)]
    public float m_GaugeValue = 1f;

    public float m_GaugeFullfillSpeed = 1f;
    public float m_ShootGaugeUseValue = 5f;

    [SerializeField] float m_RandomShootUseValue = 20f; 

    public bool m_ShootOverload = false;
    public float m_ShootOverloadTime = 2f;
    public float m_shootOverloadGaugeFullfillMultiplier = 3f;

    [Header("Shader Properties")]
    public float m_SpawnShadeTime = 2f;

    [Range(1f, 10f)]
    public float m_ExplosionPower = 1f;
    public float m_ExplosionTime = 3f;

    [Header("Heal properties")]
    public Animator m_HealVFXAnimator;

    [SerializeField]
    GameObject m_HealLeft;
    [SerializeField]
    GameObject m_HealRight;

    public float m_HealTime = 5f;

    [Header("Magic Paper properties")]
    public Transform m_MagicPaperPosOn;

    [Header("Speed properties")]
    public Renderer m_SpeedConeRenderer;






    public static PlayerScript s_Instance;

    public enum EShootStyle
    {
        Normal,
        RandomAngle,
        Count
    }
    public EShootStyle shootStyle = EShootStyle.Normal;




    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        //m_Renderer = GetComponent<Renderer>();



        if (s_Instance == null)
        {


            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);//not 2 gameManager
        }
    }
    void Start()
    {
        m_Animator.Play("PlayerSpawn");

        //StartCoroutine(SpawningShader(m_SpawnShadeTime));
        m_MaxLife = GameManagerScript.s_Instance.m_MaxLife; 
        foreach (var item in m_ReactorLineList)
        {
            item.material.SetFloat("_Opacity", 0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (m_CanShoot)
        {
            InputControl();
        }
        

        



    }

    void FixedUpdate()
    {
        Movement();
        ReactorLineUpdate();
        StartCoroutine(GaugeFullfill(3));
    }
    private void LateUpdate()
    {

    }
    //simple method to pack PlayerInputs 
    private void InputControl()
    {
        if (Input.GetButtonDown("Jump")) ShootIfGauge();
        if (Input.GetButtonDown("Fire3")) SwitchShootStyle();



    }

    void MovementEnable()
    {
        m_CanMove = true;
        m_CanShoot = true;
    }
    /**
     * add force in the input directions
     * has a minimum force to forward
     * stop the X axis force to recenter the player 
     */
    private void Movement()

    {




        //forces


        if (m_Rigidbody.velocity.z < m_MinSpeed)
        {
            m_Rigidbody.AddForce(Vector3.forward * m_Speed);
        }
        else if (m_Rigidbody.velocity.z > m_MaxSpeed)
        {
            m_Rigidbody.AddForce(-Vector3.forward * m_Speed);
        }

        if (m_Rigidbody.velocity.x > 0)
        {
            m_Rigidbody.AddForce(-Vector3.right * m_Speed * m_SideStopForceMultiplier);




        }
        else if (m_Rigidbody.velocity.x < 0)
        {
            m_Rigidbody.AddForce(Vector3.right * m_Speed * m_SideStopForceMultiplier);


        }


        if (!m_CanMove)
        {
            return;
        }

        //visual rotations 

        Quaternion rotation = Quaternion.identity;

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f)
        {
            rotation = Quaternion.Euler(0, 0, Input.GetAxisRaw("Horizontal") * -m_RotationForce);

        }

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, m_RotationSlerpSpeed * Time.fixedDeltaTime);

        //input movement

        Vector3 Dir = new Vector3(Input.GetAxisRaw("Horizontal") * m_SideForceMultiplier, 0.0f, Input.GetAxisRaw("Vertical"));//ZQSD control
        m_Rigidbody.AddForce(Dir * (m_Speed - 0.5f) * Time.fixedDeltaTime, ForceMode.VelocityChange);//force and speed



    }
    // 3 methods to shoot : simple row, angle, and random (in a certain angle ) 
    private void Shoot()
    {


        if (GameManagerScript.s_Instance.currentState != GameManagerScript.EGameState.InGame)
        {
            return;
        }
        else
        {

            float halfAngle = (m_AngleMissileNumber - 1) * m_AngleSpaceBetweenMissile * 0.5f;

            switch (shootStyle)
            {
                case EShootStyle.Normal:


                    for (int i = 0; i < m_MissileNumber; i++)
                    {
                        //Debug.Log("NormalShoot");
                        Vector3 pos = m_ShootAnchor.position + new Vector3(i - (m_MissileNumber / 2) + 0.5f, 0, 0);


                        GameObject paperProjectil = Instantiate(m_ProjectilPrefab_2, pos, m_ProjectilPrefab.transform.rotation);
                        GameObject spawnProjectil = Instantiate(m_ProjectilSpawnPrefab, m_ShootVFXSpawnAnchor.position, m_ProjectilSpawnPrefab.transform.rotation, m_ShootAnchor);
                    }
                    break;

                case EShootStyle.RandomAngle:


                    for (int i = 0; i < m_AngleMissileNumber; i++)
                    {
                        //Debug.Log("RandomShoot");

                        float xSpread = Random.Range(-1f, 1f);
                        float ySpread = Random.Range(-1f, 1f);

                        //normalize the spread vector to keep it conical
                        Vector3 spread = new Vector3(xSpread, ySpread, 0.0f).normalized * m_AngleConeSize;
                        Quaternion rotation = Quaternion.Euler(spread) * m_ShootAnchor.rotation;
                        Instantiate(m_ProjectilPrefab, m_ShootAnchor.position, rotation);
                    }
                    Instantiate(m_ProjectilSpawnPrefab, m_ShootVFXSpawnAnchor.position, m_ProjectilSpawnPrefab.transform.rotation, m_ShootAnchor);
                    break;
                case EShootStyle.Count:
                    break;
                default:
                    break;
            }









        }
    }


    //simple SetInt method
    public void SetLife(int life)
    {
        m_Life = life;
    }



    private void OnCollisionEnter(Collision collision)
    {

        if (!m_IsGodMode)//godmode still makes things colliding unless for the player itself
        {
            if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Environnement "))
            {

                m_Life--;
                m_Life = Mathf.Clamp(m_Life,0, m_MaxLife);

                if (m_Life <= 0)
                {
                    GameManagerScript.s_Instance.TriggerGameOver();

                }

                StartCoroutine(GodMode(m_GodModeDelay));

            }
            else if (collision.collider.CompareTag("Limit"))
            {

            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_IsGodMode)//godmode still makes things colliding unless for the player itself
        {
            if (other.CompareTag("Enemy"))
            {
                m_Life--;
                DestroyLevel(m_Life);

                if (m_Life > 0)
                {

                    StartCoroutine(GodMode(m_GodModeDelay));
                }

                if (m_Life <= 0)
                {
                    m_CanMove = false;
                    StartCoroutine(Exploding(m_ExplosionTime));
                }
            }
        }

        if (other.CompareTag("Collectable"))
        {

        }
    }

    public void DestroyLevel(int life)
    {
        const float leftDestroyLevel = 0.652f;
        const float rightDestroyLevel = 0.471f;
        switch (life)
        {
            case (1):

                m_Renderer.material.SetFloat("_Step", rightDestroyLevel);

                if (m_HealLeft.activeSelf)
                {
                    m_HealLeft.SetActive(false);
                }
                if (m_HealRight.activeSelf)
                {
                    m_HealRight.SetActive(false);
                }

                break;
            case (2):

                if (m_HealLeft.activeSelf)
                {
                    m_HealLeft.SetActive(false);
                }
                
                //StartCoroutine(HealingCoroutine(m_HealTime, rightDestroyLevel, "_Step"));//1 right 
                

                m_Renderer.material.SetFloat("_Step_L", leftDestroyLevel);

                break;
            case (3):

                if (m_HealRight.activeSelf)
                {
                    m_HealRight.SetActive(false);
                }
                

                //StartCoroutine(HealingCoroutine(m_HealTime, leftDestroyLevel, "_Step_L"));
                //StartCoroutine(HealingCoroutine(m_HealTime, rightDestroyLevel, "_Step"));


                break;


            default:

                break;

        }
    }

    public void HealLevel(int life, Color color)
    {
        switch (life)
        {
            case (1):

                //

                break;
            case (2):

                HealFunction(1,color);
                //StartCoroutine(HealingCoroutine(m_HealTime, rightDestroyLevel, "_Step"));//1 right 




                break;
            case (3):

                HealFunction(2,color);

                //StartCoroutine(HealingCoroutine(m_HealTime, leftDestroyLevel, "_Step_L"));
                //StartCoroutine(HealingCoroutine(m_HealTime, rightDestroyLevel, "_Step"));


                break;


            default:

                break;

        }

    }

    IEnumerator HealingCoroutine(float time, float state, string step)
    {
        float i = state;
        float rate = 1 / time;

        while (i < 1)
        {
            m_Renderer.material.SetFloat(step, i);

            yield return 0;
            i += rate * Time.deltaTime;
        }


    }

    void HealFunction(int Index, Color color )
    {
        MaterialPropertyBlock block; 
        block = new MaterialPropertyBlock();

        block.SetColor("_Color", color); 

        switch (Index)
        {
            case 1:

                m_HealRight.SetActive(true);
                m_Animator.SetTrigger("Heal_1");
                m_HealRight.GetComponent<Renderer>().SetPropertyBlock(block);


                break; 
            case 2:

                m_HealLeft.SetActive(true);
                m_Animator.SetTrigger("Heal_2");
                m_HealLeft.GetComponent<Renderer>().SetPropertyBlock(block);
                break;


            default:
                break;
        }
    }

    //the player shader contains two switch that makes the shader transparent (unless the emission map ) 
    //the godmode shader 
    void SwitchShader()
    {
        m_Renderer.material.SetInt("_Global_Opacity", (m_Renderer.material.GetInt("_Global_Opacity") + 1) % 2);




    }
    //the player enter in godmode and blink a certain amout of time with the shader effect
    IEnumerator GodMode(float time)
    {


        float i = 0f;

        float rate = 1 / time;
        m_IsGodMode = true;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        while (i < 1)
        {
            propertyBlock.SetFloat("_GodModStep", m_GodModeAnimationCurve.Evaluate(i)); 
            m_Renderer.SetPropertyBlock(propertyBlock);

            i+= rate * Time.deltaTime;  
            yield return 0; 

        }
        



        
        m_IsGodMode = false;



    }

    //the player gauge never stops to refill
    IEnumerator GaugeFullfill(float time)
    {
        if (m_GaugeValue < CanvasScript.s_Instance.m_Slider.maxValue)
        {
            m_GaugeValue += Time.fixedDeltaTime * m_GaugeFullfillSpeed;
            yield return new WaitForSeconds(time);
        }
    }
    //detect of the player is allowed to shoot depending of the gauge
    void ShootIfGauge()
    {
        switch (shootStyle)
        {
            case EShootStyle.Normal:

                if (m_GaugeValue > m_ShootGaugeUseValue && !m_ShootOverload)
                {
                    m_GaugeValue -= m_ShootGaugeUseValue;

                    Shoot();
                }
                else if (m_GaugeValue < m_ShootGaugeUseValue && !m_ShootOverload)
                {
                    m_GaugeValue = 0;
                    Shoot();
                    StartCoroutine(ShootOverloadTime(m_ShootOverloadTime));
                }
                break;
            case EShootStyle.RandomAngle:

                if (m_GaugeValue > m_RandomShootUseValue && !m_ShootOverload)
                {
                    m_GaugeValue -= m_RandomShootUseValue;

                    Shoot();
                }
                else if (m_GaugeValue < m_RandomShootUseValue && !m_ShootOverload)
                {
                    m_GaugeValue = 0;
                    Shoot();
                    StartCoroutine(ShootOverloadTime(m_ShootOverloadTime));
                }


                break;
            case EShootStyle.Count:
                break;
            default:
                break;
        }

        
    }

    //for the button onclick
    public void GodModeEnable()
    {
        m_IsGodMode = !m_IsGodMode;
        CanvasScript.s_Instance.m_GodMode.SetActive(m_IsGodMode);

    }
    //navigate through Shootmode with shift 
    public void SwitchShootStyle()
    {
        int nextShootStyle = ((int)shootStyle + 1) % (int)EShootStyle.Count;
        shootStyle = (EShootStyle)nextShootStyle;
    }
    //opacity gradient between max speed and min speed  for the line renderer
    void ReactorLineUpdate()
    {

        float value = m_MaxSpeed - m_MinSpeed;
        float diff = m_Rigidbody.velocity.z - m_MinSpeed;
        value = diff / value;
        value = Mathf.Clamp01(value);
        //print("linerenderes value" + value);

        foreach (var item in m_ReactorLineList)
        {



            item.material.SetFloat("_Opacity", value);
        }
        // changer aussi ici le distort du cone d'acceleration ( 1- Speeddistort ) avec cette value
        if (m_SpeedConeRenderer == true)
        {
            m_SpeedConeRenderer.material.SetFloat("_speed_distort", value);
        }//reste a placer le cone dans la scene 


    }


    // the player cant shoot and the gauge is refilling with a certain speed
    IEnumerator ShootOverloadTime(float time)
    {
        float speedFlag = m_GaugeFullfillSpeed;
        m_ShootOverload = true;
        m_GaugeFullfillSpeed *= m_shootOverloadGaugeFullfillMultiplier;

        m_Animator.Play(m_OverHeatingAnimation);

        yield return new WaitForSeconds(time);
        m_GaugeFullfillSpeed = speedFlag;

        m_ShootOverload = false;
    }

    //lerp a treshold between 1 and -1 to activate a pixelate clipping effect

    // exploding coroutine, set values on the exploding vfx and trigger gameover for the player 
    IEnumerator Exploding(float time)
    {
        float i = 0;
        float rate = 1 / time;


        //m_Animator.enabled = true;
        m_Animator.Play(m_Exploding);



        yield return 0;







    }

    public void TriggerRefillAnimation()
    {
        m_Animator.Play(m_RefillAnimation);
    }

    void TriggerGameOver()
    {
        GameManagerScript.s_Instance.TriggerGameOver();
    }


}
