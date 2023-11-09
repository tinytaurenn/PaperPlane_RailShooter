using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManagerScript : MonoBehaviour


{
    public static CameraManagerScript s_Instance;

    public enum playerState
    {
        Standart,
        Zoom,
        Count
    }
    public playerState state = playerState.Standart;

    public Transform focus;

    public float speed;
    public float speedRotation;
    public bool m_CameraRotationFollow = false;


    public float minimumDistance = 0.5f;

    [System.Serializable]
    public struct ECamList
    {
        public string name;
        public Transform anchor;
        public float speed;
        public float speedRotation;
    }

    public List<ECamList> m_CamList = new List<ECamList>();

    private void Awake()
    {

        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        MoveCameraPos();

    }
    private void FixedUpdate()
    {

        Quaternion cameraAnchorRotation = m_CamList[(int)state].anchor.rotation;


        if (!m_CameraRotationFollow)
        {
            Vector3 eulerRotation = cameraAnchorRotation.eulerAngles;

            cameraAnchorRotation = Quaternion.Euler(new Vector3(eulerRotation.x, eulerRotation.y, 0));
        }


        transform.rotation = Quaternion.Slerp(transform.rotation, cameraAnchorRotation, Time.deltaTime * speedRotation);

        RaycastHit hit;
        //if (Physics.Linecast(focus.position, m_CamList[(int)state].anchor.position, out hit))//if touching the wall
        //{


        //    if (hit.distance < minimumDistance)//if inbetween distance smaller than the minimum distance
        //    {
        //        print("test 1");
        //        if (!hit.collider.CompareTag("Player"))
        //        {
        //            Vector3 dest = (hit.point - focus.position).normalized * minimumDistance;

        //            transform.position = Vector3.Lerp(transform.position, focus.position + dest, speed * Time.deltaTime);
        //        }


        //    }
        //    else
        //    {

        //        transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime * speed);//doesnt hit the wall

        //    }
        //}
        //else
        //{



        //    transform.position = Vector3.Slerp(transform.position, m_CamList[(int)state].anchor.position, Time.deltaTime * speed);//smooth follow

        //}

        transform.position = Vector3.Slerp(transform.position, m_CamList[(int)state].anchor.position, Time.deltaTime * speed);//smooth follow



    }
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(focus.position, 1.0f);

        RaycastHit hit;
        if (Physics.Linecast(focus.position, m_CamList[(int)state].anchor.position, out hit))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(focus.position, hit.point);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hit.point, 1.0f);

        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(focus.position, m_CamList[(int)state].anchor.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(m_CamList[(int)state].anchor.position, 1.0f);


        }
    }

    //change camera state with right click 
    private void MoveCameraPos()
    {
        //switch to POV View
        if (Input.GetButtonDown("Fire2"))
        {

            int newState = (int)Mathf.Repeat((int)state + 1, (int)playerState.Count); //seems that mathf.repeat is slightly better than %

            state = (playerState)newState;
        }
    }
}
