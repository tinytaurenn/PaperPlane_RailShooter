using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingScript : MonoBehaviour
{
    PlayerScript player;


    private void Awake()
    {
        player = GetComponentInParent<PlayerScript>(); 

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //call the healing function in the player
    void HealingPlayer()
    {
        player.m_Life++; 
        player.DestroyLevel(player.m_Life);
    }
}
