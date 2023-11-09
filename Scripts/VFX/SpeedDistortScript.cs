using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDistortScript : MonoBehaviour
{
    Renderer m_Renderer;
    public GameObject m_Camera;
    public float m_CameraShakingMultiplier = 50f;
    public Transform m_PlayerTransform;
    [Range(-1f, 1f)]
    public float m_OpacityOffset = -0.5f;


    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();

    }
    void Start()
    {
        if (m_PlayerTransform == null)
        {
            if (PlayerScript.s_Instance == null)
            {
                return;
            }
            m_PlayerTransform = PlayerScript.s_Instance.transform;
        }
    }


    void Update()//set coordination between cameraShaking and speed material properties
    {
        m_Renderer.material.SetFloat("_GlobalOpacity", Mathf.Clamp01(m_Renderer.material.GetFloat("_speed_distort") + m_OpacityOffset));

        if (m_Camera == null)
        {
            return;
        }

        m_Camera.GetComponent<CameraShakingScript>().m_LerpSpeed = (m_Renderer.material.GetFloat("_speed_distort")) * m_CameraShakingMultiplier;
    }

    private void FixedUpdate()
    {
        if (m_PlayerTransform == null)
        {
            return;
        }
        transform.position = m_PlayerTransform.position;
    }
}
