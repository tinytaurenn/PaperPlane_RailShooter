using UnityEngine;



public class BorderlWall_FX_Script : MonoBehaviour
{
    public Transform m_Ship;

    Renderer m_Render;

    MaterialPropertyBlock m_Block; 


    private void Awake()
    {
        m_Render = GetComponent<Renderer>();
       
    }

    void Start()
    {
        m_Block = new MaterialPropertyBlock(); 
    }



    private void FixedUpdate()
    {

        Vector3 wallPos = new Vector3(transform.position.x, transform.position.y, m_Ship.position.z);
        transform.position = wallPos; 

    }
    void Update()
    {

        //evaluate opacity with distance from player 
        m_Render.GetPropertyBlock(m_Block);

        Vector3 posWall = new Vector3(transform.position.x, 0, 0); 
        Vector3 posFocus = new Vector3(m_Ship.position.x, 0, 0); 

        float dist = Vector3.Distance(posWall, posFocus); 

        m_Block.SetFloat("_Opacity", dist);
        m_Block.SetVector("_pos", m_Ship.position);

       
        

        m_Render.SetPropertyBlock(m_Block); 
    }
}
