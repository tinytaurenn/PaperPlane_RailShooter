using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_DEBUG_Orbiting : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        enum EAxis
        {
            Y,
            X,
            Z
        }

        [SerializeField]
        EAxis m_Axis = EAxis.Y;


        [SerializeField]
        float m_TimeOffSet = 0f;
        [SerializeField]
        float m_RoamingSpeed = 5f;
        [SerializeField]
        float m_DistanceFromFocus = 3f;
        [SerializeField]
        Transform m_Focus;
        [SerializeField]
        float m_LerpSpeed = 1f;
        [SerializeField]
        bool m_IsLookAt = false;
        [SerializeField]
        Transform m_LookAtFocus; 


        float m_XValue = 0f;
        float m_YValue = 0f;

        void Update()
        {

            if (m_IsLookAt && m_LookAtFocus !=null)
            {
                transform.LookAt(m_LookAtFocus); 
            }

            Orbiting(); 
        
        }


        void Orbiting()
        {
            float x = Mathf.Cos(Time.time * m_RoamingSpeed + m_TimeOffSet ) * m_DistanceFromFocus;
            float y = Mathf.Sin(Time.time * m_RoamingSpeed + m_TimeOffSet ) * m_DistanceFromFocus;

            Vector3 targetPosition = Vector3.zero; 

            switch (m_Axis)
            {
                case EAxis.Y:

                    targetPosition = new Vector3(x, y, 0) + m_Focus.position;
                    break;
                case EAxis.X:

                     targetPosition = new Vector3(x, 0, y) + m_Focus.position;
                    break;
                case EAxis.Z:

                     targetPosition = new Vector3(0, y, x) + m_Focus.position;
                    break;
                default:
                    break;
            }
            

            //transform.position = targetPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition, m_LerpSpeed * Time.deltaTime);
        }
    }
}
