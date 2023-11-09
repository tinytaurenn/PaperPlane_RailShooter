using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassUnderCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("trigger enter");
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("collision enter");
    }
}
