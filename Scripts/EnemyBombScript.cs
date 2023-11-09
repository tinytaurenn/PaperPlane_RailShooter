using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBombScript : MonoBehaviour
{

    //there is no use anymore of this script

    public float m_GrowSpeed = 1f;
    public float m_GrowTime = 2f;
    public float m_ExplodingDelay = 0.2f;
    public float m_destroyTime = 3f;
    public GameObject m_InnerBall;
    public ParticleSystem m_ParticleSystem;
    public float m_TotargetSpeed = 0.1f;




    void Start()
    {
        StartCoroutine(Growing(m_GrowTime));
    }

    // Update is called once per frame
    void Update()
    {


    }
    private void FixedUpdate()
    {
        m_ParticleSystem.transform.localScale = transform.localScale;//scale particle effect with parent scale

        if (transform.position.z > PlayerScript.s_Instance.transform.position.z)// move towards player transform
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerScript.s_Instance.transform.position, m_TotargetSpeed);
        }

    }

    //increase scale with time 
    //deprecated
    IEnumerator Growing(float time)
    {


        float i = 0;
        float rate = 1 / time;
        while (i < 1)
        {
            transform.localScale += Vector3.one * Time.fixedDeltaTime * m_GrowSpeed;



            i += Time.fixedDeltaTime * rate;


            yield return 0;



        }
        StartCoroutine(Exploding(m_ExplodingDelay, m_destroyTime));






    }
    //destroy multiples gameObjet with special timings
    IEnumerator Exploding(float delay, float destroyTime)
    {

        m_InnerBall.transform.localScale = m_InnerBall.transform.localScale * 2;//make it grow instantly
        yield return new WaitForSeconds(delay);
        m_InnerBall.gameObject.SetActive(false);
        yield return new WaitForSeconds(delay);
        Destroy(gameObject, destroyTime);
    }


    private void OnTriggerEnter(Collider other)
    {

    }
}
