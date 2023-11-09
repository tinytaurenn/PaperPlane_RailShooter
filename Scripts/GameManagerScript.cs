using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManagerScript : MonoBehaviour
{

    public GameObject m_PauseMenu;

    public static GameManagerScript s_Instance; //to easier to manage


    public List<GameObject> m_IngameEnemyList;
    [Space]
    public int m_PlayerScore = 0;

    public int m_StartLife = 3;
    public int m_MaxLife = 20;
    [Space]

    public List<GameObject> m_EnemyList;

    public Transform m_EnemySpawnTransform;
    public Transform m_PlayerTransform;




    [Header("Enemy Spawn Properies")]


    public GameObject m_EnemyPortalGameObject;


    [System.Serializable]
    public struct PortalPositions
    {
        public Transform m_EnemyTransform;
        public Vector3 m_enemySpawnOffset;

    }

    public List<PortalPositions> m_PortalPositionList = new List<PortalPositions>();

    public float m_TimeBetweenPortalAndSpawn = 3f;
    public float m_PortalDestroyTime = 6f;

    [Range(0f, 200f)]
    public float m_EnemyXSpawnRange = 50f;
    public float m_EnemyYSpawnRange = 30f;


    public float m_DistanceFromPlayerOnZ = 5f;
    public int m_EnemySpawnNumber = 5;
    public float m_EnemySpawnTime = 3f;

    public float m_RandomRange_Z_Position = 10f;


    public int m_WaveCount = 0;
    public float m_Time;
    public float m_IntervalTime = 5f;
    public float m_IntervalForceDelay = 4f;




    //allow to creates waves with number, range between them and types of enemy
    //if no more waves created, the game create new ones based on the parameters
    [System.Serializable]
    public struct WaveList
    {
        public int enemyNumber;
        public List<GameObject> enemyList;
        public float spawnRange;

    }
    public List<WaveList> waveList = new List<WaveList>();




    public enum EGameState
    {
        InGame,
        Paused,
        GameOver,
        count

    }
    public EGameState currentState = EGameState.InGame;
    // Start is called before the first frame update



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

        m_PlayerTransform = PlayerScript.s_Instance.transform;

        m_PauseMenu.SetActive(false);


        Physics.IgnoreLayerCollision(7, 7);// avoid  ally projectil to hit each others

        PlayerScript.s_Instance.SetLife(m_StartLife);

        m_Time = Time.time;






    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();



    }

    private void FixedUpdate()
    {
        //stay at a certain distance forward the player 
        m_EnemySpawnTransform.position = new Vector3(
            m_EnemySpawnTransform.position.x,
            m_EnemySpawnTransform.position.y,
            m_PlayerTransform.position.z + m_DistanceFromPlayerOnZ);
    }
    void SwitchState(EGameState GameState)
    {
        if (currentState == GameState)
        {
            return;//avoiding to switch on the same
        }
        OnExitState(currentState);
        currentState = GameState;
        OnEnterState(currentState);

    }
    void OnEnterState(EGameState GameState)
    {
        switch (GameState)
        {
            case EGameState.InGame:

                Time.timeScale = 1.0f;
                break;
            case EGameState.Paused:
                m_PauseMenu.SetActive(true);
                m_PauseMenu.GetComponentInChildren<Button>().Select();


                Time.timeScale = 0.0f;
                break;
            case EGameState.GameOver:
                Time.timeScale = 0.0f;
                CanvasScript.s_Instance.m_GameOver.gameObject.SetActive(true);

                break;
            default:
                break;
        }

    }
    void UpdateState()//fonction appelée chaque frame
    {
        switch (currentState)
        {
            case EGameState.InGame:
                InGame();
                break;
            case EGameState.Paused:
                Pause();
                break;
            case EGameState.GameOver:
                GameOver();
                break;
            default:
                break;
        }


    }
    void OnExitState(EGameState GameState)
    {
        switch (GameState)
        {
            case EGameState.InGame:

                break;
            case EGameState.Paused:
                m_PauseMenu.SetActive(false);
                Time.timeScale = 1.0f;
                //CanvaManager.instance.pauseMenu.SetActive(false);


                break;
            case EGameState.GameOver:

                //CanvaManager.instance.gameOverMenu.SetActive(false);

                ReloadScene();


                break;
            default:
                break;
        }
    }
    void InGame()
    {
        if (Input.GetKeyDown(KeyCode.I)) //debug 
        {
            WaveSpawner(0);
        }

        if (m_IngameEnemyList.Count <= 0)
        {
            if (Time.time > m_Time + m_IntervalTime)
            {
                if (m_Time == 0) //avoid long wait for the first wave 
                {
                    print("first wave ");
                    WaveSpawner(0);
                }
                else
                {
                    print("interval force delay");
                    WaveSpawner(m_IntervalForceDelay);
                }


                m_Time = Time.time;
            }

        }

        if (Input.GetButtonDown("start"))
        {
            SwitchState(EGameState.Paused);
        }

    }

    void Pause()
    {



        if (Input.GetButtonDown("start"))
        {
            SwitchState(EGameState.InGame);
        }

    }
    void GameOver()
    {
        //press enter to reset at game Over
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CanvasScript.s_Instance.m_GameOver.gameObject.SetActive(false);
            SwitchState(EGameState.InGame);
        }
    }
    public void TriggerGameOver()
    {

        SwitchState(EGameState.GameOver);
    }

    //spawn a number of enemy from an enemy list :   in random positions and  in a certain range
    IEnumerator EnemySpawnList(int SpawnNumber, List<GameObject> EnemyList, float SpawnTime, float StartTime)
    {
        yield return new WaitForSeconds(StartTime);

        m_WaveCount++;
        foreach (GameObject item in m_IngameEnemyList)
        {
            Destroy(item);
        }
        m_IngameEnemyList.Clear();

        // opening the portal here 

        
        int randomPortalPosStruct = Random.Range(0, m_PortalPositionList.Count);

        
        GameObject portal = Instantiate(m_EnemyPortalGameObject, m_PortalPositionList[randomPortalPosStruct].m_EnemyTransform);

        yield return new WaitForSeconds(m_TimeBetweenPortalAndSpawn);



        for (int i = 0; i < SpawnNumber; i++)
        {
            int enemyRandomIndex = Random.Range(0, EnemyList.Count);



            
            GameObject enemyGameObject = Instantiate(EnemyList[enemyRandomIndex], 
                m_PortalPositionList[randomPortalPosStruct].m_EnemyTransform.position + m_PortalPositionList[randomPortalPosStruct].m_enemySpawnOffset,
                m_EnemyList[enemyRandomIndex].transform.rotation);

            m_IngameEnemyList.Add(enemyGameObject);

            yield return new WaitForSeconds(SpawnTime); //little intervals between spawns
        }
        
        portal.GetComponent<Animator>().SetBool("IsClosing", true);
        

    }
    //select the wave to spawn, starting from the created ones to  the automatic ones
    void WaveSpawner(float delay)
    {

        if (m_WaveCount < waveList.Count)
        {
            StartCoroutine(EnemySpawnList(waveList[m_WaveCount].enemyNumber, waveList[m_WaveCount].enemyList, m_EnemySpawnTime, delay));
        }
        else if (m_WaveCount >= waveList.Count)
        {
            StartCoroutine(EnemySpawnList(m_EnemySpawnNumber, m_EnemyList, m_EnemySpawnTime, delay));
        }


    }

    public void ReloadScene()
    {
        PlayerScript.s_Instance.m_Life = m_StartLife;
        Time.timeScale = 1.0f;


        SceneManager.LoadScene(0);
    }
    public void ApplicationQuit()
    {
        Application.Quit();
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_EnemySpawnTransform.position - m_EnemySpawnTransform.right * m_EnemyXSpawnRange, m_EnemySpawnTransform.position + m_EnemySpawnTransform.right * m_EnemyXSpawnRange);

        



    }
}
