using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//showReel Script for projectils

public class DebugShooterScript : MonoBehaviour
{
    public GameObject m_Projectil;
    public float m_AngleConeSize = 25f;
    public int m_AngleMissileNumber = 2; 

    public GameObject m_Projectil_2;
    public Transform m_ShootAnchor;

    public float m_ShootForce = 10f;

    public GameObject m_SpawnParticleSystem;
    public Transform m_SpawnParticleSystemTransform; 

    public bool m_ShootTrigger = false; 
    public bool m_ShootTrigger_2 = false; 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {



        if (m_ShootTrigger)
        {
            for (int i = 0; i < m_AngleMissileNumber; i++)
            {
                //Debug.Log("RandomShoot");

                float xSpread = Random.Range(-1f, 1f);
                float ySpread = Random.Range(-1f, 1f);

                //normalize the spread vector to keep it conical
                Vector3 spread = new Vector3(xSpread, ySpread, 0.0f).normalized * m_AngleConeSize;
                Quaternion rotation = Quaternion.Euler(spread) * m_ShootAnchor.rotation;
                Instantiate(m_Projectil, m_ShootAnchor.position, rotation);
            }
            
            Instantiate(m_SpawnParticleSystem, m_SpawnParticleSystemTransform.position, m_SpawnParticleSystem.transform.rotation, m_SpawnParticleSystemTransform); 
            m_ShootTrigger = false; 
        }

        if (m_ShootTrigger_2)
        {
            Shoot(m_Projectil_2);
            Instantiate(m_SpawnParticleSystem, m_SpawnParticleSystemTransform.position, m_SpawnParticleSystem.transform.rotation, m_SpawnParticleSystemTransform);
            m_ShootTrigger_2 = false;
        }
    }

    void Shoot(GameObject projectil)
    {
        Instantiate(projectil, m_ShootAnchor.position, m_Projectil.transform.rotation);
        //projectil.GetComponent<Rigidbody>().AddForce(Vector3.forward * m_ShootForce, ForceMode.Impulse); 

    }

    
}

