using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//no use of this script anymore
public class FinalSlashScript : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ParentFalling()
    {
        if (transform.parent == null)
        {
            return; 
        }

        if (transform.parent.TryGetComponent<EnemyScript>(out EnemyScript parent))
        {
            parent.m_FallSmokeTrail.Play(); 
            parent.currentState = EnemyScript.EEnemyState.Falling;
        }

        transform.parent = null; 

        

    }
}
