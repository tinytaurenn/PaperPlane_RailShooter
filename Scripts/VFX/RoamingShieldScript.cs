using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingShieldScript : MonoBehaviour
{

    [SerializeField]
    GameObject[] m_Models;

    [SerializeField]
    Animator m_RotateAnimator;

    [SerializeField]
    float m_RotateAnimatorMaxSpeed = 1f; 

    public GameObject m_Spark_PS;

    public LayerMask m_ProjectilMask;

    public Transform m_Focus;
    public Transform m_PointToDefend;
    public float m_RoamingSpeed = 4f;
    public float m_DistanceFromFocus = 5f;
    public float m_LerpSpeed = 0.5f;
    public float m_TimeOffSet = 0.5f;

    public float m_DetectionDistance;

    public float m_ExplosionDetectionRadius = 5f;
    public float m_ExplosionRadius = 30f;

    public bool m_IsDefending = false; // remplacer par detectiond de projectile enemi 

    public enum EState
    {
        Roaming,
        Defending
    }

    public EState currentState = EState.Roaming;


    private void Awake()
    {
       
        
        int randomInt = Random.Range(0,m_Models.Length);
        print ("randomint" + randomInt);

        m_Models[randomInt].SetActive(true);
    }
    void Start()
    {
        m_RotateAnimator.SetFloat("SpeedMultiplier", Random.Range(-m_RotateAnimatorMaxSpeed, m_RotateAnimatorMaxSpeed)); 
       


    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();

    }

    private void FixedUpdate()
    {
        FixedUpdateState();
        //transform.position = Vector3.Slerp(transform.position, m_PointsToLerpList[(int)Mathf.Repeat(m_PointsToLerpList.Count, 1)], m_SlerpSpeed); 

    }

    #region StateMachine

    void OnEnterState()
    {
        switch (currentState)
        {
            case EState.Roaming:
                break;
            case EState.Defending:


                break;
            default:
                break;
        }

    }

    void OnExitState()
    {
        switch (currentState)
        {
            case EState.Roaming:
                break;
            case EState.Defending:
                break;
            default:
                break;
        }
    }

    public void SwitchState(EState state)
    {
        if (state == currentState)
        {
            return;
        }

        OnExitState();
        currentState = state;
        OnEnterState();
    }

    void UpdateState()
    {

        ProjectilInterception(m_ExplosionDetectionRadius, m_ExplosionRadius);

        switch (currentState)
        {
            case EState.Roaming:
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, m_LerpSpeed * Time.deltaTime);

                break;
            case EState.Defending:


                transform.localScale = Vector3.Lerp(transform.localScale, 2 * Vector3.one, m_LerpSpeed * Time.deltaTime);

                break;
            default:
                break;
        }
    }


    void FixedUpdateState()
    {
        switch (currentState)
        {
            case EState.Roaming:

                Orbit();

                //if (m_IsDefending)
                //{
                //    SwitchState(State.Defending);
                //}

                break;
            case EState.Defending:

                transform.position = Vector3.Slerp(transform.position, m_PointToDefend.position, m_LerpSpeed * Time.deltaTime);

                //if (!m_IsDefending)
                //{
                //    SwitchState(State.Roaming); 
                //}

                break;
            default:
                break;
        }
    }
    /*/
     * If there is a projectil in the detection radius, explodes and destroy all the projectils in the explosion radius and the shield itself
     *
     */

    void ProjectilInterception(float detectionRadius, float explosionRadius)
    {

        if (Physics.CheckSphere(transform.position, detectionRadius, m_ProjectilMask))
        {

            Collider[] projectils = Physics.OverlapSphere(transform.position, explosionRadius, m_ProjectilMask);
            foreach (var item in projectils)
            {

                GameObject SparkPS = Instantiate(m_Spark_PS, item.transform.position, m_Spark_PS.transform.rotation);
                Destroy(item.gameObject);


            }

            m_Focus.GetComponent<Enemy1Script>().m_RoamingShieldList.Remove(this.gameObject);
            Instantiate(m_Spark_PS, transform.position, m_Spark_PS.transform.rotation);
            Destroy(gameObject);

        }

    }

    //void AutoDestroy()
    //{
    //    Instantiate(m_Spark_PS, transform.position, m_Spark_PS.transform.rotation);
    //    Destroy(gameObject);

    //}



    /*/
     * Circle roaming around the focus
     */

    void Orbit()
    {
        float x = Mathf.Cos(Time.time + m_TimeOffSet * m_RoamingSpeed) * m_DistanceFromFocus;
        float y = Mathf.Sin(Time.time + m_TimeOffSet * m_RoamingSpeed) * m_DistanceFromFocus;
        Vector3 targetPosition = new Vector3(x, y, 0) + m_Focus.position;

        //transform.position = targetPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, m_LerpSpeed * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionDetectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }

    #endregion
}
