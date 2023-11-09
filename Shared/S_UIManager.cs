using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;
using UnityEngine.UIElements;
using System.IO;
using TMPro;
using static Shared.S_GameManager;

namespace Shared
{
    public class S_UIManager : MonoBehaviour
    {
        public static S_UIManager s_instance;
        private AudioSource m_audioSource;
        [SerializeField] private AudioClip m_selectSound;
        [SerializeField] private AudioClip m_clickedSound;

        [SerializeField]
        private EventSystem m_eventSystem;

        [Header("Menus and Canvas's")]
        [SerializeField]
         CanvasGroup m_mainMenu,
          m_characterSelectionMenu,
          m_levelSelectionMenu,
          m_optionsMenu,
          m_PauseMenu,
          m_GameOverMenu,
          m_EndScreen,
          m_HUD,
          m_creditsMenu;
        Vector3 m_cameraBasePos;
        [SerializeField]Vector3 m_posOffset;
        Quaternion m_cameraBaseRot;
        [SerializeField] Quaternion m_rotOffset;
        Camera m_cam;
        [SerializeField] float m_ppCrementalIndex = 0.01f;
        [SerializeField] Transform m_characters;
        int m_characterCount;
        int m_currentCharacter = 4;
        public int CurrentCharacter { get { return m_currentCharacter; } }

        //[Header("Menus First buttons")]
        //[SerializeField] GameObject m_characterSelectionFirstButton;
        //[SerializeField] GameObject m_optionsFirstButton;
        //[SerializeField] GameObject m_levelSelectionFirstButton;
        //[SerializeField] GameObject m_creditsFirstButton;
        [SerializeField]
        private List<Vector3> m_characterOffSetPosition = new List<Vector3>();
        [SerializeField]
        private List<Quaternion> m_characterOffSetRotation = new List<Quaternion>();
        private CanvasGroup m_currentMenu;
        private GameObject m_lastSelectedGameObject;
        private int m_currentLevelToPlayID = 2;
        Coroutine m_camInCourse;
        Coroutine m_blurCourse;

        [SerializeField] int m_creditAugmentSpeed = 4;
        int m_actualSpeed = 1;
        [SerializeField] List<TextAsset> m_credits = new List<TextAsset>();
        [SerializeField] TextMeshProUGUI m_textComponent;
        [SerializeField] int m_creditSpeed = 5;
        [SerializeField] float m_cameraRotationSpeed = 1;
        [SerializeField] float m_cameraPositionSpeed = 1.5f;
        [SerializeField] float m_cameraBetweenCharPositionSpeed = 1;
        float m_cameraActualPositionSpeed;
        Vector3 m_creditsBasePos;
        Stack<Coroutine> m_CreditDisplacement = new();
        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
            }
            m_audioSource = GetComponent<AudioSource>();
            s_instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            //DontDestroyOnLoad(this.gameObject); 
            if (m_mainMenu)
            {
                print("main menu"); 
                m_currentMenu = m_mainMenu;
                m_eventSystem.SetSelectedGameObject(m_mainMenu.gameObject.GetComponent<S_Menu>().FirstSelectableButton);
            }
            
            m_cam = Camera.main;
            m_cameraBasePos = m_cam.transform.position;
            m_cameraBaseRot = m_cam.transform.rotation;
            if (m_characters)
                m_characterCount = m_characters.childCount;
        }

        // Update is called once per frame
        void Update()
        {

            // Get always something selected :
            if (m_eventSystem.currentSelectedGameObject == null && m_lastSelectedGameObject != null)
            {
                m_eventSystem.SetSelectedGameObject(m_lastSelectedGameObject);
            }
            else if (m_eventSystem.currentSelectedGameObject != m_lastSelectedGameObject)
            {
                m_lastSelectedGameObject = m_eventSystem.currentSelectedGameObject;
            }
        }

        public int CurrentLevelToPlayID
        {
            get{
                return m_currentLevelToPlayID;
            }

            set{
                m_currentLevelToPlayID = value;
            }
        }
        
        public void LoadLevel(int level)
        {
            //print("loading level : " + level);
            m_currentLevelToPlayID = level;
            //CharacterSelectionMenu();
            LoadLevel(); 
           // print("gamestate level : " + level + " state : " + S_GameManager.Instance.GameState);

            //S_GameManager.Instance.GameState = S_GameManager.EGameState.Ingame; 

            // S_GameManager.Instance.LoadLevel(level);

        }
        public void LoadLevel()
        {
            HideCanvas(m_currentMenu);
            S_GameManager.Instance.LoadLevel(m_currentLevelToPlayID);
            S_GameManager.Instance.CurrentPlayerIndex = m_currentCharacter;

            //S_GameManager.Instance.CharSelected = m_currentCharacter;
        }

        #region CharacterSelectionField

        public void CharacterSelectionMenu()
        {
            m_cameraActualPositionSpeed = m_cameraPositionSpeed;
            CamCourse(m_characterOffSetPosition[m_currentCharacter], m_characterOffSetRotation[m_currentCharacter]);
            BlurCourse(true);
            m_characters.GetChild(m_currentCharacter).gameObject.SetActive(true);

        }
        public void CharacterSelect(int selectionType)
        {
            //m_characters.GetChild(m_currentCharacter).gameObject.SetActive(false);
            m_currentCharacter += selectionType;
            if (m_currentCharacter < 0)
                m_currentCharacter = m_characterCount - 1;
            else
                m_currentCharacter = m_currentCharacter % m_characterCount;
            //m_characters.GetChild(m_currentCharacter).gameObject.SetActive(true);
            m_cameraActualPositionSpeed = m_cameraBetweenCharPositionSpeed;
            CamCourse(m_characterOffSetPosition[m_currentCharacter], m_characterOffSetRotation[m_currentCharacter]);
        }
        void BlurCourse(bool blurType)
        {
            if (m_blurCourse != null)
                StopCoroutine(m_blurCourse);
            m_blurCourse = StartCoroutine(BlurChange(blurType));
        }
        void CamCourse(Vector3 position, Quaternion rotation)
        {
            if (m_camInCourse != null)
                StopCoroutine(m_camInCourse);
            m_camInCourse = StartCoroutine(CameraMovement(position, rotation));
        }
        IEnumerator CameraMovement(Vector3 endPosition,Quaternion endRotation)
        {
            while (m_cam.transform.position != endPosition)
            {

                m_cam.transform.position = Vector3.Lerp(m_cam.transform.position, endPosition, Time.deltaTime * m_cameraActualPositionSpeed);
                m_cam.transform.rotation = Quaternion.Lerp(m_cam.transform.rotation, endRotation, Time.deltaTime * m_cameraRotationSpeed);
                yield return new WaitForFixedUpdate();
            }


        }
        IEnumerator BlurChange(bool selection)
        {
            PostProcessVolume ppvolume = m_cam.GetComponent<PostProcessVolume>();
            if (selection)
                m_ppCrementalIndex = -m_ppCrementalIndex;
            else
                m_ppCrementalIndex = Mathf.Abs(m_ppCrementalIndex);
            do
            {
                ppvolume.weight += m_ppCrementalIndex;
                yield return new WaitForFixedUpdate();
            }while(ppvolume.weight < 1 && ppvolume.weight > 0.01f);

        }
        #endregion


        public void ChangeCurrentInteractible(GameObject interactible)
        {
            //EventSystem.current.SetSelectedGameObject(null);
            m_lastSelectedGameObject = interactible;
            
            m_eventSystem.SetSelectedGameObject(interactible);
        }

        public void ChangeCanvas(CanvasGroup newMenu)
        {
            if (m_currentMenu == m_characterSelectionMenu)
            {
                CamCourse(m_cameraBasePos, m_cameraBaseRot);
                BlurCourse(false);
                //m_characters.GetChild(m_currentCharacter).gameObject.SetActive(false);
            }
            if (m_currentMenu == m_creditsMenu)
            {
                foreach (var item in m_CreditDisplacement)
                {
                    StopCoroutine(item);
                }
                m_CreditDisplacement.Clear();
            }


            HideCanvas(m_currentMenu);
            //ChangeCurrentInteractible(newMenu.gameObject.GetComponent<S_Menu>().FirstSelectableButton);
            m_currentMenu = newMenu;
            ShowCanvas(m_currentMenu);
            ChangeCurrentInteractible(m_currentMenu.gameObject.GetComponent<S_Menu>().FirstSelectableButton);
        }

        private void HideCanvas(CanvasGroup menu)
        {

            menu.interactable = false;
            menu.alpha = 0;
            menu.blocksRaycasts = false;
            menu.gameObject.SetActive(false);
            
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(1);
        }

        private void ShowCanvas(CanvasGroup menu)
        {
            menu.gameObject.SetActive(true); 

            menu.interactable = true;
            menu.alpha = 1;
            menu.blocksRaycasts = true;
        }

        public void LaunchLevel()
        {
            
            SceneManager.LoadScene(m_currentLevelToPlayID);
        }

        public void QuitGame()
        {
            //Code snippet provided by https://gamedevbeginner.com/how-to-quit-the-game-in-unity/
            #if UNITY_STANDALONE //quit build 

            Application.Quit();
            #endif

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        public static void BtnWasSelected(GameObject selectedBtn)
        {
            if (selectedBtn == s_instance.m_lastSelectedGameObject) return;
            s_instance.m_audioSource.clip = s_instance.m_selectSound;
            s_instance.m_audioSource.Play();
        }

        public static void PlaySelectSound()
        {
            s_instance.m_audioSource.clip = s_instance.m_selectSound;
            s_instance.m_audioSource.Play();
        }

        public static void BtnWasClicked()
        {
            s_instance.m_audioSource.clip = s_instance.m_clickedSound;
            s_instance.m_audioSource.Play();
        }

        public static void BtnWasCancel()
        {
            s_instance.m_audioSource.clip = s_instance.m_clickedSound;
            s_instance.m_audioSource.Play();
        }

        public void CreditReading()
        {
            if (m_textComponent.text.Length < m_credits[0].text.Length)
            {
                m_creditsBasePos = m_textComponent.transform.position;
                foreach (var text in m_credits)
                    m_textComponent.text += text.text;
            }
            m_textComponent.transform.position = m_creditsBasePos;
            m_CreditDisplacement.Push(StartCoroutine(Displacement()));
        }
        public IEnumerator Displacement()
        {
            while (true)
            {
                m_textComponent.transform.position = new(m_textComponent.transform.position.x, m_textComponent.transform.position.y + Time.deltaTime * m_creditSpeed, m_textComponent.transform.position.z);
                yield return new WaitForEndOfFrame();
            }
        }
        public void CreditSpeedModifier()
        {
            if (m_actualSpeed < m_creditAugmentSpeed)
            {
                m_CreditDisplacement.Push(StartCoroutine(Displacement()));
                m_actualSpeed++;
            }
            else
            {
                for (; m_actualSpeed > 1; m_actualSpeed--)
                {
                    StopCoroutine(m_CreditDisplacement.Peek());
                    m_CreditDisplacement.Pop();
                }
            }
        }
    }
}
