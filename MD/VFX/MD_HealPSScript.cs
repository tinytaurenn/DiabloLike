using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_HealPSScript : MonoBehaviour
    {
        public SkinnedMeshRenderer MeshRenderer { get; set; }
        ParticleSystem m_PS;
        [SerializeField]
        ParticleSystem m_ChildHealDropPs;

        string m_ShaderHealValue = "_HealValue"; 
        


        private void Awake()
        {
            m_PS = GetComponent<ParticleSystem>();
            
            
        }
        void Start()
        {

            var PS_SkinnedMesh = m_PS.shape; 

            PS_SkinnedMesh.skinnedMeshRenderer = MeshRenderer;

            try
            {
                MeshRenderer.material.SetFloat(m_ShaderHealValue, 1f); //only works in solo
            }
            catch (NullReferenceException)
            {

                print("sad ");
            }
            

            if (m_ChildHealDropPs != null)
            {
                var childPSRenderer = m_ChildHealDropPs.shape;
                childPSRenderer.skinnedMeshRenderer = MeshRenderer;

            }
            else
            {
                Debug.LogWarning("Child ParticleSystem m_ChildHealDropPs is not assigned or not found.");
            }


            


        }

        

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
