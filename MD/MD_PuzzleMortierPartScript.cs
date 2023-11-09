using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_PuzzleMortierPartScript : MonoBehaviour
    {

        //public bool IsRepaired => m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Construction");

        
        public bool IsRepaired
        {
            get;
            private set;
        }

        

        [SerializeField]
        int m_OrderNumber = 0; 


        public int OrderNumber => m_OrderNumber; 


        Animator m_Animator;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            

        }
        void Start()
        {

            //foreach (Transform item in GetComponentsInChildren<Transform>())
            //{
            //    if (item == this.transform)
            //    {
            //        continue;
            //    }
            //    m_PiecesList.Add(item.gameObject);
            //}





        }

        void RepairSound()
        {
            if (IsRepaired)
            {
                transform.parent.GetComponent<MD_PuzzleScriptMortier>().RepairSound();
            }

           
        }

        public void Repair()
        {
            m_Animator.SetTrigger("Construction"); 
            print("repairing myself");
            IsRepaired = true; 
        }

        public void BreakPart()
        {
            m_Animator.SetTrigger("Destruction"); 
            IsRepaired = false; 
        }

        

        // Update is called once per frame
        void Update()
        {

        }


    }
}
