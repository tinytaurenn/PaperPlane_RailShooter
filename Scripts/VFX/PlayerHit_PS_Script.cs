using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit_PS_Script : MonoBehaviour
{
    Vector3 m_EulerRotation;



    public float m_DestroyTime = 2f; 
    private void Awake()
    {
        
    }

    void Start()
    {

        //change item rotation
        m_EulerRotation = transform.rotation.eulerAngles;

        m_EulerRotation = new Vector3(
            Random.Range(0, 360),
            m_EulerRotation.y,
            m_EulerRotation.z);

        transform.rotation = Quaternion.Euler(m_EulerRotation);

        Destroy(this.gameObject, m_DestroyTime);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
