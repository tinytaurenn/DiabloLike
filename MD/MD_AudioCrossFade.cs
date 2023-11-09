using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_AudioCrossFade : MonoBehaviour
    {

        float m_timer = 0f;
        [Range(0f, 2f)]
        [SerializeField]
        float m_GlobalVolumeModifier = 1f;  

        [Serializable]
        struct AudioSourceStruct
        {
            
            public AudioSource audioSource;

            
            public AnimationCurve curve;
        }

        [SerializeField]
        AudioSourceStruct[] m_Struct = new AudioSourceStruct[2]; 

        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {

            


            m_timer += Time.deltaTime;

            float deltaValue = Time.time / m_Struct[0].audioSource.clip.length * m_Struct[0].audioSource.pitch; 

            
            

            foreach (var audio in m_Struct)
            {
                audio.audioSource.volume = audio.curve.Evaluate(Mathf.Repeat(deltaValue,1));
                audio.audioSource.volume = Mathf.Clamp01( audio.audioSource.volume * m_GlobalVolumeModifier);
                
            }
        
        }
    }
}
