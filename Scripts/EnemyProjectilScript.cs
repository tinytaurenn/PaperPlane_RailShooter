using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyProjectilScript : MonoBehaviour
{
    [Header("ShowCase mode")]
    public bool m_IsShowCaseMode = false;
    public List<Transform> m_ShowCase_RoamingPoints_List;

    public int m_NextPosIndex = 0;

    public float m_RotationSlerpForce = 0.9f;
    public float m_DistanceToChangeTrajectory = 6f; 


    public Rigidbody m_Rigidbody;
    public float m_speed;
    public bool deviation;
    [Range(3, 15)]
    public float bulletDuration;

    public float bulletHitDestroyTime = 0.5f;

    public GameObject Hit;
    public GameObject Flash;

    [SerializeField]
    GameObject[] m_PencilsList; 


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        int chosenPencil = Random.Range(0, m_PencilsList.Length);

        m_PencilsList[chosenPencil].SetActive(true); 

    }





    void Start()
    {
        if (m_IsShowCaseMode)
        {

            transform.LookAt(m_ShowCase_RoamingPoints_List[0].position);
            m_Rigidbody.AddForce(transform.forward * m_speed, ForceMode.VelocityChange); 


            return; 
        }



        //simple push forward force at spawn

        m_Rigidbody.AddForce(transform.forward * m_speed, ForceMode.VelocityChange);

        if (Flash != null)
        {
            GameObject flashObj = Instantiate(Flash, transform.position, Quaternion.identity);
            ParticleSystem flashPs = flashObj.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashObj, flashPs.main.duration);
            }
        }


        Destroy(gameObject, bulletDuration);//destroy projectile after X s duration

    }



    void Update()
    {
        m_Rigidbody.velocity = transform.forward * m_speed; 

        if (m_IsShowCaseMode)
        {

            ShowCaseModeFunction(); 



        }
    }

    void ShowCaseModeFunction()
    {
        if (Vector3.Distance(transform.position, m_ShowCase_RoamingPoints_List[m_NextPosIndex].position) < m_DistanceToChangeTrajectory)
        {


            m_NextPosIndex = (int)Mathf.Repeat(m_NextPosIndex + 1, m_ShowCase_RoamingPoints_List.Count);





        }

        Vector3 direction = m_ShowCase_RoamingPoints_List[m_NextPosIndex].position - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), m_RotationSlerpForce * Time.deltaTime);
        //transform.LookAt(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {



        if (!collision.collider.CompareTag("Enemy"))
        {
            //Hit.Play();

            GameObject.Instantiate(Hit, transform.position, Hit.transform.rotation);
            //Destroy(this.gameObject, bulletHitDestroyTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name + " nom de l'objet touché ");
        if (!other.CompareTag("Enemy")
            &&!other.CompareTag("Collectable"))
        {
            GameObject.Instantiate(Hit, other.ClosestPoint(transform.position), Hit.transform.rotation, other.transform) ;

            //Destroy(this.gameObject, bulletHitDestroyTime);

            //marche moins bien 
        }

    }



}
