using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CanvasScript : MonoBehaviour
{

    public static CanvasScript s_Instance;



    public TextMeshProUGUI m_LifeText;
    public TextMeshProUGUI m_PlayerScore;
    public TextMeshProUGUI m_GameOver;
    public GameObject m_GodMode;

    public Image m_GaugeImage;
    public Gradient m_Gradient;
    public Slider m_Slider;

    public Image m_CriticalStateColor;

    public float m_CriticalFadeSpeed;
    public bool m_IsRed = false;

    public float m_FadeDelay;

    private float m_BlinkTime;




    private void Awake()
    {
        if (s_Instance == null)
        {


            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);//not 2 gameManager
        }
    }
    void Start()
    {
        m_LifeText.text = "Life : " + PlayerScript.s_Instance.m_Life;
        m_PlayerScore.text = "Score : " + GameManagerScript.s_Instance.m_PlayerScore;

        m_BlinkTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {



    }
    private void FixedUpdate()
    {




        //updating Score and life UI
        m_LifeText.text = "Life : " + PlayerScript.s_Instance.m_Life;
        m_PlayerScore.text = "Score : " + GameManagerScript.s_Instance.m_PlayerScore;

        /**
         * the gauge follow the public gradient color and change to red in a certain delay to make it "blink"
         */


        if (PlayerScript.s_Instance.m_ShootOverload)
        {
            BlinkyGaugeBar();
        }
        else
        {
            m_GaugeImage.color = m_Gradient.Evaluate(m_Slider.normalizedValue);
        }



        m_Slider.value = PlayerScript.s_Instance.m_GaugeValue;


    }
    // switch between two color with a time Interval
    void BlinkyGaugeBar()
    {
        if (Time.time > m_BlinkTime + m_FadeDelay)
        {
            m_IsRed = !m_IsRed;
            if (m_IsRed)
            {

                m_GaugeImage.color = Color.red;


            }
            else
            {
                m_GaugeImage.color = m_Gradient.Evaluate(m_Slider.normalizedValue);

            }

            m_BlinkTime = Time.time;

        }
    }


}
