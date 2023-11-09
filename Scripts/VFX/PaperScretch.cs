using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperScretch : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer m_Renderer;
    [SerializeField]
    AnimationCurve m_Curve;
    [SerializeField]
    float m_Time;

    [SerializeField]
    float m_Timer = 0;
    [SerializeField]
    float m_ResetTime = 5f; 

    void Start()
    {

        
       
    }

    IEnumerator PaperScretching(float time)
    {
        float i = 0;
        float rate = 1 / time;

        while (i < 1)
        {
            print(i + " = i ");
            m_Renderer.SetBlendShapeWeight(0, m_Curve.Evaluate(i));
            i += Time.deltaTime * rate;
            yield return 0;
        }

        yield return 0; 

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >  m_Timer + m_ResetTime )
        {
            m_Timer = Time.time;
            m_Renderer.SetBlendShapeWeight(0, 0);
            StartCoroutine(PaperScretching(m_Time));

        }
        
    }
}
