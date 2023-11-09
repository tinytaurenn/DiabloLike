using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;
using static Shared.S_GameManager;
using System;

using UnityEngine.SceneManagement;

namespace Shared
{
    public class S_InGameMenuManager : MonoBehaviour
    {


        [SerializeField] private EventSystem m_eventSystem;
        [SerializeField] private AudioSource m_audioSource;
        [SerializeField] private AudioClip m_selectSound;
        private static S_InGameMenuManager s_instance;
        [SerializeField] private int m_mainMenuID = 1;


        [Header("Pause Menu")]
        [SerializeField] private S_Menu m_pauseMenu;
        //[SerializeField] private GameObject m_startFocus;

        [Space(10)]

        [Header("Parameters Menu")]
        [SerializeField] private CanvasGroup m_parametersMenu;
        private S_Menu m_parametersMenuSMenu;

        [Space(10)]

        [Header("Revive Menu")]
        [SerializeField] private S_Menu m_reviveMenu;

        [Space(10)]

        [Header("GameOver Menu")]
        [SerializeField] private S_Menu m_gameOverMenu;


        private void Awake()
        {
            m_parametersMenuSMenu = m_parametersMenu.GetComponent<S_Menu>();
            if (s_instance != null) return;
            // OnPauseEnter += Open;
            s_instance = this;
            
        }

        private void OnEnable()
        {
            OnSwitchState += SwitchState;

        }

        private void OnDisable()
        {
            OnSwitchState -= SwitchState;
        }

        public void ShowParameters()
        {
            m_pauseMenu.gameObject.SetActive(false);
            m_parametersMenu.alpha = 1.0f;
            m_parametersMenu.interactable = true;
            m_eventSystem.SetSelectedGameObject(m_parametersMenuSMenu.FirstSelectableButton);
        }

        public void HideParameters()
        {
            m_parametersMenu.alpha = 0.0f;
            m_parametersMenu.interactable = false;
            m_pauseMenu.gameObject.SetActive(true);
            m_eventSystem.SetSelectedGameObject(m_pauseMenu.FirstSelectableButton);
        }

        public void ChangeBrightness(float value)
        {
            S_ImageCorrectionPP correctionPP = Camera.main.GetComponent<S_ImageCorrectionPP>();
            if (correctionPP)
            {
                correctionPP.ChangeBrightness(value);
            }
        }

        public void ChangeGamma(float value)
        {
            S_ImageCorrectionPP correctionPP = Camera.main.GetComponent<S_ImageCorrectionPP>();
            if (correctionPP)
            {
                correctionPP.ChangeGamma(value);
            }
        }

        public static void Push() => s_instance.OnPauseTrigger();

        public static void ClosePause() => s_instance.ClosePauseMenu(); 
        
        private void SwitchState(EGameState state)
        {
            if (state == EGameState.Death) OnDeathTrigger();
            else if (state == EGameState.Pause) OnPauseTrigger();
            else if (state == EGameState.GameOver) OnGameOverTrigger();
        }

        private void OnDeathTrigger()
        {

            if (S_GameManager.Instance.Hearts == 0)
            {
                return; 
            }
            m_reviveMenu.gameObject.SetActive(true);
            m_eventSystem.SetSelectedGameObject(m_reviveMenu.FirstSelectableButton);
            //Time.timeScale = 0f;
            S_GameManager.Instance.GameState = EGameState.Death;
        }

        private void OnPauseTrigger()
        {
            m_pauseMenu.gameObject.SetActive(true);
            m_eventSystem.SetSelectedGameObject(m_pauseMenu.FirstSelectableButton);
            //Time.timeScale = 0f;
            S_GameManager.Instance.GameState = EGameState.Pause;
        }

        private void OnGameOverTrigger()
        {
            m_gameOverMenu.gameObject.SetActive(true);
            m_eventSystem.SetSelectedGameObject(m_gameOverMenu.FirstSelectableButton);
            S_GameManager.Instance.GameState = EGameState.Death;
        }

        public void ClosePauseMenu()
        {
            //Time.timeScale = 1.0f;
            m_pauseMenu.gameObject.SetActive(false);
            S_GameManager.Instance.GameState = EGameState.Ingame;
        }
        public void CloseReviveMenu()
        {
            //Time.timeScale = 1.0f;
            m_reviveMenu.gameObject.SetActive(false);
            //S_GameManager.Instance.GameState = EGameState.Ingame;
        }

        public void CloseGameOverMenu()
        {
            m_gameOverMenu.gameObject.SetActive(false) ;
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(m_mainMenuID);
            m_pauseMenu.gameObject.SetActive(false); 
            S_GameManager.Instance.GameState = EGameState.Menu; 
        }

        public void NextCheckPoint()
        {
            S_GameManager.Instance.NextCheckPoint();
            ClosePause(); 
        }

        public void QuitGame()
        {
            #if UNITY_STANDALONE //quit build 

            Application.Quit();
            #endif

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif



        }


        public void PlaySelectSound()
        {
            m_audioSource.clip = s_instance.m_selectSound;
            m_audioSource.Play();
        }

        public void PlaySubmitSound()
        {
            m_audioSource.clip = s_instance.m_selectSound;
            m_audioSource.Play();
        }
    }
}
