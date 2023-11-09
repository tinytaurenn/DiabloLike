using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace DumortierMatthieu
{
    public class MD_PuzzleScript : MonoBehaviour
    {
        PlayerInput m_PlayerInput;

        [SerializeField]

        GameObject m_Player;


        private void Awake()
        {
            m_PlayerInput = new PlayerInput(); 
        }

        void Update()
        {
        
        }

        private void OnEnable()
        {
            
            m_PlayerInput.Puzzle.SelectAction.performed += selection => PuzzleSelect(); 

            
            
        }

        private void OnDisable()
        {
            m_PlayerInput.Disable();
            m_PlayerInput.Puzzle.SelectAction.performed -= selection => PuzzleSelect();
        }

        void PuzzleSelect()
        {
            print("Puzzle is responding"); 
        }

        private void OnTriggerEnter(Collider other)
        {

            //checkPlayer
            print("enabling puzzle input");

            
            m_PlayerInput.Enable();
        }

        private void OnTriggerExit(Collider other)
        {
            //checkPlayer
            print("disable puzzle input");
            m_PlayerInput.Disable();
        }
    }
}
