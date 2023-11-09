using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRenderSwitchScript : MonoBehaviour
{
    //public Material m_Material;
    public ParticleSystem m_ParticleSystem;
    public float[] m_OffSetList = { .54f, .46f }; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var customData = m_ParticleSystem.customData;
        customData.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
        float randomInList = m_OffSetList[Random.Range(0, m_OffSetList.Length)];

        customData.SetVector(ParticleSystemCustomData.Custom1, 1, randomInList);
    }
}
