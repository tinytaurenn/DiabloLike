using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;
using UnityEditor;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Xml;

namespace Shared
{

    
    public class S_GameManager : MonoBehaviour
    {
        public static S_GameManager Instance { get; private set; }

        [SerializeField]
        private Material m_imageCorrectionPostProcessMaterial;
        [SerializeField]
        private AudioMixer m_masterMixer;

        public static event Action<EGameState> OnSwitchState;

        public static event Action OnPlayerSwitch; 
        public static event Action OnPlayerAfterRevive; 
        public static event Action<bool> OnPause; 

        public static event Action<bool> OnResume;

        public static event Action<int> OnLifeUpdate; 
        public static event Action<int> OnManaUpdate;
        public static event Action<int> OnHeartsUpdate;

        public static event Action<bool> OnPlayerControllerInput;

        [SerializeField]
        bool m_IsLevelLoaded = false;

        public bool IsLevelLoad => m_IsLevelLoaded; 

        [Header("Sliders")]
        [SerializeField]
        private Slider m_volumeSlider;
        [SerializeField]
        private Slider m_brightnessSlider;
        [SerializeField]
        private Slider m_gammaSlider;

        [Space(10)]
        [Header("Instances")]
        [Space(10)]


        [SerializeField]
        CheckPointManager m_CheckPointManager;

       
        [Space(5)]

        [SerializeField]
        GameObject m_UI_Manager;

        [SerializeField]
        float m_PauseFadeSpeed = 1f; 

        public  GameObject UI_Manager  => m_UI_Manager;
        
        
        int m_charSelected;
        public int CharSelected
        {
            set { m_charSelected = value; }
        }

        [Space(10)]
        [Header("Players")]
        [Space(10)]

        [SerializeField]
        GameObject[] m_PlayerAssetList = new GameObject[5];

        [Space(10)]
        [Header("Loading Screenshots")]
        [Space(10)]

        [SerializeField]
        Sprite[] m_LoadingScreenList = new Sprite[5];

        [Space(10)]
        [Header("Loading Parameters")]
        [Space(10)]

        [SerializeField]
        Image m_LoadingScreen;

        [SerializeField]
        Slider m_ProgressionSlider; 

        AsyncOperation m_LoadingAsync;

        [Space(5)]
        [SerializeField]
        float m_LoadingForcedTime = 3f; 

        bool m_IsLevelLoading = false;

        [Space(10)]
        [Header("Game Info")]
        [Space(10)]

        [SerializeField]
        private int m_CurrentPlayerIndex  = 0;

        private int  m_CurrentPlayerIndexFlag = 0; 

        public int CurrentPlayerIndex
        {
            get
            {
                return m_CurrentPlayerIndex; 
            }
            set
            {


                m_CurrentPlayerIndex = value; 
            }
        }


        [SerializeField]
        GameObject m_CurrentPlayer;

        [SerializeField]
        Vector3 m_LastCheckPointPos; 

        public GameObject CurrentPlayer
        {
            get
            {
                return m_CurrentPlayer; 
            }
            set
            { 
                m_CurrentPlayer = value;
            }
        }

        [SerializeField]
        public int MaxHearts { get; private set; } = 3;
        [SerializeField]
        int m_hearts = 0;
        public  int Hearts => m_hearts;


        [SerializeField]
        int m_Life = 100;

        int m_LifeFlag = 100; 

        public int  MaxLife { get; private set; } = 100; 

        public int Life
        {
            get
            {

                return m_Life;

            }
            set
            {
                m_Life = value;

               
            }
        }

        [SerializeField]
        int m_Mana = 100;

        int m_ManaFlag = 100;

        

        public int Mana {

            get
            {
                return m_Mana; 
            } 
            set 
            {
                m_Mana = Mathf.Clamp(value, 0, MaxMana); 

                OnManaUpdate?.Invoke(m_Mana);
            } 
        }

        public int MaxMana { get; private set;  } = 100;

        [Space(10)]
        [Header("God Mode")]
        [Space(10)]

        [SerializeField]
        bool m_IsGodmode = false;

        public bool IsGodMode
        {
            get
            {
                return m_IsGodmode;
            }
            set
            {
                m_IsGodmode = value;
            }
        }


        public enum EGameState
        {
            Menu, 
            Ingame,
            Puzzle,
            Pause,
            Death,
            GameOver
        }

        [Space(10)]
        [Header("GameState")]
        [Space(10)]

        [SerializeField]
        EGameState m_GameState = EGameState.Ingame;
       

        EGameState m_FlagGameState = EGameState.Ingame; 

        public EGameState GameState {

            get { return m_GameState; } 
            set {  m_GameState = value; } 
        }

        Coroutine m_PauseCoroutine; 


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject); 
            }
            

            DontDestroyOnLoad(this.gameObject); 
        }
        // Start is called before the first frame update
        void Start()
        {
            //ChangeHearts(MaxHearts); 

            SetSliders();
            
            m_CurrentPlayerIndexFlag = m_CurrentPlayerIndex; 

            m_FlagGameState = m_GameState; 

            MaxLife = m_Life;

            MaxMana = m_Mana; 
        }

        public void SetSliders()
        {
            try
            {
                m_volumeSlider.value = PlayerPrefs.GetFloat("Volume");
                m_brightnessSlider.value = PlayerPrefs.GetFloat("Brightness");
                m_gammaSlider.value = PlayerPrefs.GetFloat("Gamma");
            }
            catch (NullReferenceException)
            {

                UnityEngine.Debug.LogWarning("Set Sliders null ref"); 
            }

            
        }

        // Update is called once per frame
        void Update()
        {
            //print("TimeScale is : " + Time.timeScale); 

            VerifThatCouldGoInPropertieButWeCannot();

            


            LoadingLevel(); 
            
            
        }

        

        void VerifThatCouldGoInPropertieButWeCannot()
        {
            if (m_IsGodmode)
            {
                GodMode();
            }
            else
            {
                if (m_ManaFlag != m_Mana)
                {
                    
                    m_ManaFlag = m_Mana;
                    OnManaUpdate?.Invoke(m_Mana);

                }

                if (m_LifeFlag != m_Life)
                {
                    m_LifeFlag = m_Life;
                    OnLifeUpdate?.Invoke(m_Life);

                    CheckDeath();

                }

            }

            

            


            if (m_FlagGameState != m_GameState)
            {
                print("switching gamestate"); 
                SwitchStateUpdate();
                

            }

            if (m_CurrentPlayerIndexFlag != m_CurrentPlayerIndex)
            {
                if (m_GameState == EGameState.Ingame && m_LoadingAsync ==null)
                {
                    SwitchPlayer(m_CurrentPlayerIndex);
                }

                
                m_CurrentPlayerIndexFlag = m_CurrentPlayerIndex; 

            }
        }

        #region StateMachine

        void OnEnterState(EGameState state)
        {

            switch (state)
            {
                case EGameState.Menu:



                    


                    OnPlayerControllerInput?.Invoke(false);

                    OnSwitchState?.Invoke(EGameState.Menu);

                    m_Mana = MaxMana;
                    m_Life = MaxLife;
                    m_hearts = MaxHearts;

                    OnLifeUpdate?.Invoke(MaxLife);
                    OnManaUpdate?.Invoke(MaxMana); 
                    OnHeartsUpdate?.Invoke(MaxHearts);

                    print("ui manager desactivate");


                    m_UI_Manager.SetActive(false);


                    SceneManager.LoadScene(1);

                    m_IsLevelLoaded =   false; 

                    


                    OnSwitchState?.Invoke(EGameState.Menu);
                    break;
                case EGameState.Ingame:

                    



                    OnSwitchState?.Invoke(EGameState.Ingame);

                    break;
                case EGameState.Puzzle:

                    OnPlayerControllerInput?.Invoke(false);
                    OnSwitchState?.Invoke(EGameState.Puzzle);
                    break;
                case EGameState.Pause:


                    //Time.timeScale = 0f; 
                    m_PauseCoroutine =  StartCoroutine(FadePauseRoutine(m_PauseFadeSpeed));
                    
                    OnSwitchState?.Invoke(EGameState.Pause);

                    break;
                case EGameState.Death:

                    ChangeHearts(m_hearts--); 

                    OnPlayerControllerInput?.Invoke(false); 

                    OnSwitchState?.Invoke(EGameState.Death); 

                    break;
                default:
                    break;
            }


        }



        void OnExitState(EGameState state)
        {

            switch (state)
            {
                case EGameState.Menu:
                    //print("exit menu"); 

                    



                    

                    break;
                case EGameState.Ingame:
                    break;
                case EGameState.Puzzle:
                    OnPlayerControllerInput?.Invoke(true);
                    break;
                case EGameState.Pause:



                    if (m_PauseCoroutine != null)
                    {
                        StopCoroutine(m_PauseCoroutine);
                    }

                    OnPause?.Invoke(false);
                    if (GameState != EGameState.Menu)
                    {
                        S_InGameMenuManager.ClosePause();
                    }

                    

                    Time.timeScale = 1f;
                    break;
                case EGameState.Death:
                    break;
                default:
                    break;
            }


        }

        void SwitchStateUpdate()
        {
            OnExitState(m_FlagGameState);
            //print ("flag gamestate is " +  m_FlagGameState);
            //print(" gamestate is " + m_GameState);
            m_FlagGameState = m_GameState;
            OnEnterState(m_GameState);
            OnSwitchState?.Invoke(m_GameState);
        }

         void SwitchState(EGameState state)
        {

            if (state == m_GameState)
            {
                if (state == EGameState.Pause)
                {
                    SwitchState(EGameState.Ingame);
                    return; 
                    
                }

                if (state == EGameState.Puzzle)
                {
                    SwitchState( EGameState.Ingame);
                    return;
                }
                else
                {
                    return;
                }
                 
            }
            //print("switch not even ");

            OnSwitchState?.Invoke(state); // Allow playersscripts to listen game state changing 

            OnExitState(m_GameState);
            m_GameState = state;    
            OnEnterState(m_GameState);    

        }

        void UpdateState()
        {
            switch (m_GameState)
            {
                case EGameState.Menu:
                    break;
                case EGameState.Ingame:
                    break;
                case EGameState.Puzzle:
                    break;
                case EGameState.Pause:
                    break;
                case EGameState.Death:
                    break;
                default:
                    break;
            }
        }
        #endregion


        IEnumerator FadePauseRoutine(float time)
        {
            float i = 1;

            float rate = 1 / time; 

            while (i>=0)
            {
                Time.timeScale = i;

                i -= rate * Time.fixedDeltaTime;

                

                yield return 0; 


            }

            OnPause?.Invoke(true);



        }

        void CheckDeath()
        {
            if (m_Life <= 0)
            {
                m_hearts--;
                SwitchState(m_hearts <= 0 ? EGameState.GameOver : EGameState.Death);
            }
        }

        public void LoadLevel(int level)
        {

            m_IsLevelLoaded = false; 

            m_LoadingScreen.sprite = m_LoadingScreenList[level-2];
            m_LoadingScreen.gameObject.SetActive(true);

            m_LoadingAsync = SceneManager.LoadSceneAsync(level);
            m_LoadingAsync.allowSceneActivation = true;

           m_IsLevelLoading = true;
           //OnPlayerControllerInput?.Invoke(false);
           m_GameState = EGameState.Ingame;

            //S_SkillManager.SetCharacterSprites(m_CurrentPlayerIndex);

            //m_GameState = EGameState.Ingame;

            //SceneManager.LoadScene(level);

            //StartCoroutine(LoadingScreenRoutine(level, 6f)); 
        }

        void LoadingLevel()
        {
            if (m_LoadingAsync == null) return;


            if (m_IsLevelLoading == false) return; 

            //print("progress is : " + m_LoadingAsync.progress);

            //m_ProgressionSlider.value = m_LoadingAsync.progress;

            m_ProgressionSlider.value = Mathf.Lerp(m_ProgressionSlider.value, m_LoadingAsync.progress, m_LoadingForcedTime * Time.deltaTime); 






            if (m_LoadingAsync.isDone && m_ProgressionSlider.value >= 0.95f)
            {
                //m_LoadingScreen.gameObject.SetActive(false);

                
                m_IsLevelLoading = false;

                m_ProgressionSlider.value = 0f; 

                StartCoroutine(LoadingScreenRoutine(0));
            }


            
        }

        IEnumerator LoadingScreenRoutine( float time)
        {
            

           

            yield return new WaitForSeconds(time);


            

            m_LoadingScreen.gameObject.SetActive(false);

            m_UI_Manager.SetActive(true);

            m_LoadingAsync = null;

            OnPlayerControllerInput?.Invoke(true);
            //m_CurrentPlayerIndex = m_charSelected;

            m_IsLevelLoaded = true;




        }



        void Ondeath()
        {
            //DeathMenu

        }

        public void Spawn( Vector3 startPos)
        {
            m_CurrentPlayer = Instantiate(m_PlayerAssetList[m_CurrentPlayerIndex], startPos, m_PlayerAssetList[m_CurrentPlayerIndex].transform.rotation);
        }

        public void Spawn(int currentPlayerIndex, Vector3 startPos)
        {
            m_CurrentPlayer = Instantiate(m_PlayerAssetList[currentPlayerIndex], startPos , m_PlayerAssetList[currentPlayerIndex].transform.rotation);
        }

        public void OnRevive()
        {
            const int MANA_ON_REVIVE = 50;
            const int LIFE_ON_REVIVE = 50; 

            //m_CurrentPlayer.transform.position = m_CheckPointManager.LastCheckPoint();

            if (m_GameState != EGameState.Death)
            {
                OnPlayerSwitch?.Invoke(); //should not be used out of debuging
            }

            Destroy(m_CurrentPlayer.gameObject);
            m_CurrentPlayer = Instantiate(m_PlayerAssetList[m_CurrentPlayerIndex], m_CheckPointManager.LastCheckPoint(), m_PlayerAssetList[m_CurrentPlayerIndex].transform.rotation);
            m_Life = LIFE_ON_REVIVE;
            m_Mana = MANA_ON_REVIVE; 
            SwitchState(EGameState.Ingame);
            OnPlayerAfterRevive.Invoke();

        }

        public void SetCheckPointManager(CheckPointManager manager)
        {
            m_CheckPointManager = manager;
        }

        public void NextCheckPoint()
        {
            if (m_CheckPointManager == null)
            {
                print("there is no  checkpoint manager attached to this script");
                return; 
            }

            Vector3 pos =  m_CheckPointManager.NextCheckPointPos(); 


            if (CurrentPlayer.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
            {
                agent.enabled = false;
                m_CurrentPlayer.transform.position = pos;
                agent.enabled = true;

            }
            else
            {
                m_CurrentPlayer.transform.position = pos;
            }

            
        }


         void SwitchPlayer(int playerIndex)
        {

            Vector3 pos = m_CurrentPlayer.transform.position; 

            Quaternion currentRotation = m_CurrentPlayer.transform.rotation;

            if (m_CurrentPlayer != null)
            {
                Destroy(m_CurrentPlayer);
            }
            

            
            m_CurrentPlayer = Instantiate(
                m_PlayerAssetList[playerIndex],
                pos,
               currentRotation);

            //m_CurrentPlayerIndex = playerIndex; 

            OnPlayerSwitch?.Invoke(); 

        }

        

        public void SetGodMod()
        {
            m_IsGodmode = !m_IsGodmode; 
        }

        void GodMode()
        {

            if (m_Mana < MaxMana)
            {
                m_Mana = MaxMana; 
            }

            if (m_Life < MaxLife)
            {
                m_Life = MaxLife; 
            }

            if (m_ManaFlag != m_Mana)
            {
                m_Mana = MaxMana; 
                
                m_ManaFlag = m_Mana;
                OnManaUpdate?.Invoke(m_Mana);

            }

            if (m_LifeFlag != m_Life)
            {
                m_Life = MaxLife; 
                m_LifeFlag = m_Life;
                OnLifeUpdate?.Invoke(m_Life);

                CheckDeath();

            }
        }

        void ChangeHearts(int value)
        {
            value = Mathf.Clamp(value, 0, MaxHearts); 
            m_hearts = value;
            OnHeartsUpdate?.Invoke(m_hearts);
        }

        public void EnableObject(GameObject obj )
        {
            obj.SetActive(!obj.activeSelf);
        }

        

        


        public void ChangeVolume(float value) // code snippet borrowed from https://answers.unity.com/questions/1174589/changing-game-volume.html & https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
        {
            float newvolume = Mathf.Log(value) * 20;
            PlayerPrefs.SetFloat("Volume", value);
            m_masterMixer.SetFloat("MasterVolume", newvolume);
        }

        

        
    }
}
