using Shared;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace DumortierMatthieu
{
    public class MD_PuzzleScriptMortier : MonoBehaviour
    {

        enum EPuzzleState
        {
            Enabled,
            Disabled
        }
        [SerializeField]
        EPuzzleState m_State = EPuzzleState.Disabled; 

        PlayerInput m_PlayerInput; 

        [SerializeField]
        bool m_IsSelected = false;

        [SerializeField]
        bool m_IsFinished = false; 

        Renderer m_Renderer;

        LocalKeyword m_KeyWord;

        Animator m_Animator;

        [SerializeField]
        float m_MinDistanceFromPlayer = 10f; 

        [SerializeField]
        List<MD_PuzzleMortierPartScript> m_PiecesList = new List<MD_PuzzleMortierPartScript>();

        [SerializeField]
        GameObject m_CurrentPlayer;

        MaterialPropertyBlock m_MaterialPropertyBlock;

        [SerializeField]
        int m_ItemIndex = 0;

        [SerializeField]
        int m_OrderNumber = 0;


        [SerializeField]
        GameObject m_CannonBall;
        [SerializeField]
        GameObject m_CannonBallExplosionEmbergenGameObject; 

        [SerializeField]
        MD_PuzzleBridge m_BridgeScript;

        [SerializeField]
        float m_BridgeDestructionDelay = 0;

        [SerializeField]
        float m_DestructionWatchTime = 6f;

        [SerializeField]
        Transform m_PuzzleCamera;
        [SerializeField]
        Transform m_DestructionViewCamera;

        [SerializeField]
        GameObject m_ButtonTextObject; 

        [Space(10)]
        [Header("SOUNDS")]
        [Space(10)]

        [SerializeField]
        MD_SoundManager m_PuzzleFailSound;
        [SerializeField]
        MD_SoundManager m_PuzzleWinSetupSound;
        [SerializeField]
        MD_SoundManager m_PuzzleWinShootSound; 
        [SerializeField]
        MD_SoundManager m_PuzzleSelectSound;
        [SerializeField]
        MD_SoundManager m_PuzzleValidationSound;


        const float CAMERASHAKE_AMPLITUDE = 1f;
        const float CAMERASHAKE_SPRING = 0.35f;
        const float CAMERASHAKE_RECOIL = 0.35f;
        const float CAMERASHAKE_DAMPING = 0.4f; 



        private void Awake()
        {
            
            m_Animator = GetComponent<Animator>();
            m_PlayerInput = new PlayerInput();
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
        }
        void Start()
        {
            m_CurrentPlayer = S_GameManager.Instance.CurrentPlayer; 

            foreach (var item in GetComponentsInChildren<MD_PuzzleMortierPartScript>())
            {
                if (item == this)
                {
                    continue; 
                }
                m_PiecesList.Add(item); 
            }
            
            m_KeyWord = new LocalKeyword(m_PiecesList[0].GetComponent<Renderer>().material.shader, "_SELECTED_ON");


            DestroyAllParts();
            



        }

        private void OnEnable()
        {
            S_GameManager.OnPlayerSwitch += NewCurrentPlayer;

            S_GameManager.OnSwitchState += OnGameManagerSwitchState; 

            m_PlayerInput.Puzzle.SelectAction.performed += selection => ToPuzzleState();

            m_PlayerInput.Puzzle.DRightAction.performed += selectionNext => SelectNext(1);
            m_PlayerInput.Puzzle.DLeftAction.performed += selectionPrevious => SelectNext(-1);

            m_PlayerInput.Puzzle.LTAction.performed += validation => ValidatePuzzle();






        }

        private void OnDisable()
        {
            S_GameManager.OnPlayerSwitch -= NewCurrentPlayer;

            S_GameManager.OnSwitchState -= OnGameManagerSwitchState;

            m_PlayerInput.Disable();
            m_PlayerInput.Puzzle.SelectAction.performed -= selection => ToPuzzleState();

            m_PlayerInput.Puzzle.DRightAction.performed -= selectionNext => SelectNext(1);
            m_PlayerInput.Puzzle.DLeftAction.performed -= selectionPrevious => SelectNext(-1);

            m_PlayerInput.Puzzle.LTAction.performed -= validation => ValidatePuzzle();
        }

        // Update is called once per frame
        void Update()
        {
            if (Physics.CheckSphere(transform.position, m_MinDistanceFromPlayer) && m_CurrentPlayer == null)
            {
                m_CurrentPlayer = S_GameManager.Instance.CurrentPlayer;
                
            }
            OnUpdateState(); 
           
        }

        void OnUpdateState()
        {
            switch (m_State)
            {
                case EPuzzleState.Enabled:
                    break;
                case EPuzzleState.Disabled:
                    break;
                default:
                    break;
            }
        }

        void OnEnterState(EPuzzleState state)
        {
            switch (state)
            {
                case EPuzzleState.Enabled:
                    m_PlayerInput.Puzzle.SelectAction.Enable();
                    
                    break;
                case EPuzzleState.Disabled:
                    m_PlayerInput.Puzzle.SelectAction.Disable();

                    break;
                default:
                    break;
            }
        }

        void OnExitState(EPuzzleState state)
        {
            switch (state)
            {
                case EPuzzleState.Enabled:
                    

                    break;
                case EPuzzleState.Disabled:
                    break;
                default:
                    break;
            }
        }

        void SwitchState(EPuzzleState state)
        {
            if (m_State == state)
            {
                return; 
            }

            OnExitState(m_State); 
            m_State = state;
            OnEnterState(m_State);
        }



        void DestroyAllParts()
        {
            foreach (var item in m_PiecesList)
            {
                item.BreakPart();

            }
        }

        void BreakRepairedParts()
        {

            m_PuzzleFailSound.Activation(transform); 

            foreach (var item in m_PiecesList)
            {
                if (item.IsRepaired)
                {
                    item.BreakPart();
                }
                

            }

        }

        void ToPuzzleState()
        {

            if (m_IsFinished)

            {
                return; 
            }
            

            
            //S_GameManager.Instance.SwitchState(S_GameManager.EGameState.Puzzle);
            //S_GameManager.Instance.GameState = S_GameManager.EGameState.Puzzle;

            if (S_GameManager.Instance.GameState == S_GameManager.EGameState.Puzzle)
            {
                S_GameManager.Instance.GameState = S_GameManager.EGameState.Ingame;
                CameraManager.Instance.CameraState = CameraManager.ECameraState.Follow;
                m_ButtonTextObject.SetActive(true);
            }
            else
            {
                m_ButtonTextObject.SetActive(false); 

                print("waouuuw");
                CameraManager.Instance.LockActiveTransform = m_PuzzleCamera.transform; 
                S_GameManager.Instance.GameState = S_GameManager.EGameState.Puzzle;
                CameraManager.Instance.CameraState = CameraManager.ECameraState.Puzzle;
            }

        }

        void Select(int index, bool selected)
        {
            m_PuzzleSelectSound.Activation(m_PiecesList[index].transform);

            m_PiecesList[index].GetComponent<Renderer>().material.SetKeyword(m_KeyWord, selected);

            m_IsSelected = selected;
        }

        

        void NewCurrentPlayer()
        {
            m_CurrentPlayer = S_GameManager.Instance.CurrentPlayer;
        }

        void SelectNext(int selection)
        {
            

            Select(m_ItemIndex, false);
            int nextSelect = CheckNext(selection);
            

            Select(nextSelect,true);
            m_ItemIndex = nextSelect;
        }

        private void ValidatePuzzle()
        {
            if (m_PiecesList[m_ItemIndex].OrderNumber != m_OrderNumber)
            {

               
                BreakRepairedParts();
                m_OrderNumber = 0; 
                return; 

               
            }

            m_PiecesList[m_ItemIndex].Repair();
            //RepairSound(); 


            if (!SameOrderNumberVerif(m_OrderNumber))
            {
                m_OrderNumber++;
            }
            
            

            if (CheckWinning())
            {
                EndPuzzle();

                // canon script activation
                //Destroy(this); 
                return; 
            }
            SelectNext(1); 
        }

        public void RepairSound()
        {
            m_PuzzleValidationSound.Activation(m_PiecesList[m_ItemIndex].transform);
        }

        void ShootSound()
        {
            print("shooting sound  "); 
            m_PuzzleWinShootSound.Activation(transform);
            
        }

        void CameraShake()
        {


            CameraManager.Instance.GetComponent<MD_CameraShake>().Shake(CAMERASHAKE_AMPLITUDE,CAMERASHAKE_SPRING,CAMERASHAKE_DAMPING,CAMERASHAKE_RECOIL);
        }

        void SetupSound()
        {
            m_PuzzleWinSetupSound.Activation(transform);
        }

        bool SameOrderNumberVerif(int number)
        {
            foreach (var piece in m_PiecesList)
            {
                if (piece.OrderNumber == m_OrderNumber && piece.IsRepaired == false)
                {
                    return true;
                }
            }

            return false;

        }

        void EndPuzzle()
        {

            print("puzzle is finished");
            
            ToPuzzleState();
            Select(m_ItemIndex, false);
            m_PlayerInput.Disable();


            m_ButtonTextObject.SetActive(false);

            m_Animator.enabled = true; 
            SetupSound();


            this.enabled = false;

            m_IsFinished = true;

            CameraManager.Instance.LockTimee = m_DestructionWatchTime; 
            CameraManager.Instance.LockActiveTransform = m_DestructionViewCamera;
            CameraManager.Instance.CameraState = CameraManager.ECameraState.Lock;

            

            //StartCoroutine( CameraManager.Instance.ToLockPos(m_DestructionWatchTime)); 



        }


        int CheckNext(int selection)
        {
            int nextSelect;

            
            nextSelect = (m_ItemIndex + selection);
            
            nextSelect = (int)Mathf.Repeat(nextSelect, m_PiecesList.Count);
            print("nextselect 1  is " + nextSelect);

            if (m_PiecesList[nextSelect].IsRepaired)
            {
                if (selection > 0 )
                {
                    return CheckNext(selection + 1);
                }
                else if (selection < 0 )
                {
                    return CheckNext(selection - 1);
                }
            }
            print("actual selection is " + selection);
            print("nextselect is " + nextSelect); 
            return nextSelect;

        }

        bool CheckWinning()
        {
            foreach (var item in m_PiecesList)
            {
                if (!item.IsRepaired)
                {
                    return false;
                }
            }

            return true; 
        }

        void OnGameManagerSwitchState(S_GameManager.EGameState state)
        {
            if (state == S_GameManager.EGameState.Puzzle)
            {
                Select(m_ItemIndex, true);
                m_PlayerInput.Puzzle.DRightAction.Enable();
                m_PlayerInput.Puzzle.DLeftAction.Enable();

                m_PlayerInput.Puzzle.LTAction.Enable();

                //m_PlayerInput.Puzzle.DRightAction.performed += selectionNext => SelectNext(1);
                //m_PlayerInput.Puzzle.DLeftAction.performed += selectionPrevious => SelectNext(-1);

                //m_PlayerInput.Puzzle.LTAction.performed += validation => ValidatePuzzle();
            }
            else
            {


                Select(m_ItemIndex, false);

                m_PlayerInput.Puzzle.DRightAction.Disable();
                m_PlayerInput.Puzzle.DLeftAction.Disable();

                m_PlayerInput.Puzzle.LTAction.Disable();


                //m_PlayerInput.Puzzle.DRightAction.performed -= selectionNext => SelectNext(1);
                //m_PlayerInput.Puzzle.DLeftAction.performed -= selectionPrevious => SelectNext(-1);

                //m_PlayerInput.Puzzle.LTAction.performed -= validation => ValidatePuzzle();

            }
        }

        void ShootCannon()
        {
            m_CannonBall.SetActive(true);
            
            
            m_BridgeScript.PuzzleValidationTrigger(m_BridgeDestructionDelay); 
        }
        void ShootCannonEmbergenEffect()
        {
            m_CannonBallExplosionEmbergenGameObject.SetActive(true);
            m_CannonBallExplosionEmbergenGameObject.transform.parent = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return; 
            }
            //checkPlayer

            print("enabling puzzle input");

            SwitchState(EPuzzleState.Enabled);
            
        }

        private void OnTriggerExit(Collider other)
        {

            if (!other.CompareTag("Player"))
            {
                return;
            }
            //checkPlayer
            print("disable puzzle input");

            SwitchState(EPuzzleState.Disabled);
            
        }
    }
}
