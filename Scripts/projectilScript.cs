using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectilScript : MonoBehaviour
{
    public Rigidbody m_Rigidbody;
    public Animator m_Animator;

    public int m_Damage = 1;
    public float m_speed;
    public bool m_Deviation;
    [Range(0.5f, 15f)]
    public float bulletDuration;

    public float bulletHitDestroyTime = 0.5f;

    public GameObject m_Hit;
    public GameObject m_Flash;
    public float m_GrowingSpeed = 1f;
    public bool m_IsAlone;

    public float m_FlipMaxSpeed = 2f;
    float m_FlipSpeed;

    [Header("Detect Enemy properties")]
    public LayerMask m_EnemyMask;
    public bool m_IsLerping = false;
    public float m_LerpToEnemySpeed = 2f;
    public float m_DetectRadius = 20f;
    public Transform m_TargetToLerp;
    public bool m_IsTargetFound = false;
    public float m_DistanceOverToDestroy = 3f;



    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();



    }

    void Start()
    {

        m_FlipSpeed = Random.Range(-m_FlipMaxSpeed, m_FlipMaxSpeed);

        if (m_FlipSpeed > 0)
        {
            m_FlipSpeed += m_FlipMaxSpeed;
        }
        else
        {
            m_FlipSpeed -= m_FlipMaxSpeed;
        }
        //m_Animator.speed = m_FlipSpeed;
        if (m_Animator != null)
        {
            //m_Animator.SetFloat("SpeedMultiplier", m_FlipSpeed);
        }


        m_Rigidbody.AddForce(transform.forward * m_speed, ForceMode.Impulse);

        if (m_Flash != null)
        {
            GameObject flashObj = Instantiate(m_Flash, transform.position, Quaternion.identity);
            ParticleSystem flashPs = flashObj.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashObj, flashPs.main.duration);
            }
        }


        Destroy(gameObject, bulletDuration);

    }


    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (m_IsLerping)
        {
            //FindingEnemyToLerp();
            LerpToEnemy(m_TargetToLerp);

        }


        
    }
    //search for an enemy in a chosen radius, and lock a transform
    void FindingEnemyToLerp()
    {
        if (m_IsTargetFound)
        {
            return;
        }


        if (!Physics.CheckSphere(transform.position, m_DetectRadius, m_EnemyMask))
        {
            return;
        }
        const int maxEnemyDetected = 3;
        Collider[] enemies = new Collider[maxEnemyDetected];
        enemies = Physics.OverlapSphere(transform.position, m_DetectRadius, m_EnemyMask);
        if (enemies.Length <= 0)
        {
            return;
        }
        float maxDistance = 999f;
        int targetIndex = 0;

        print("Enemies Lenghts is " + enemies.Length);
        for (int i = 0; i < enemies.Length; i++)
        {

            
            float distance2 = enemies[i].gameObject.transform.position.x - transform.position.x;
            float distance = Vector3.Distance(transform.position, enemies[i].gameObject.transform.position);
            if (distance2 < maxDistance)
            {
                if (enemies[i].transform.position.z > PlayerScript.s_Instance.transform.position.z)
                {

                    targetIndex = i;

                }

                
            }
        }


        m_TargetToLerp = enemies[targetIndex].gameObject.transform;
        m_IsTargetFound = true;




    }
    //Lerp to the nearest enemy transform if detected 
    void LerpToEnemy(Transform target)
    {
        if (m_TargetToLerp != null && transform.position.z > m_TargetToLerp.transform.position.z + m_DistanceOverToDestroy)
        {
            Destroy(this.gameObject);
        }

        if (m_IsTargetFound && m_TargetToLerp != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, m_LerpToEnemySpeed * Time.deltaTime);
        }
        else
        {
            FindingEnemyToLerp();
        }




    }




    private void OnCollisionEnter(Collision collision)
    {



        if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Projectil"))
        {
            //Hit.Play();

            GameObject.Instantiate(m_Hit, collision.transform.position, collision.transform.rotation, collision.transform);
            Destroy(this.gameObject, bulletHitDestroyTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name + " nom de l'objet touché ");
        if (!other.CompareTag("Player")
            && !other.CompareTag("Projectil")
            && !other.CompareTag("Collectable"))
        {
            GameObject.Instantiate(m_Hit, transform.position, transform.rotation);//start Hit VFX
            Destroy(this.gameObject, bulletHitDestroyTime);


        }

        if (other.CompareTag("Enemy"))
        {
            print("hured an enemy");
        }

    }
    private void OnDrawGizmos()
    {
        if (m_IsLerping)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_DetectRadius);
        }
    }




}
