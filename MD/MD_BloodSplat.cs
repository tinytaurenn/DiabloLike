using DumortierMatthieu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_BloodSplat : MonoBehaviour
    {
        [SerializeField]
        MD_SoundManager m_BloodSplatSound; 
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        
        private void OnParticleCollision(GameObject other)
        {
            m_BloodSplatSound.Activation(transform); 
        }

        
    }
}
