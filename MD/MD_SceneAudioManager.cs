using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DumortierMatthieu
{
    public class MD_SceneAudioManager : MonoBehaviour
    {
        

        


        AudioSource[] m_AudioSources;

        [SerializeField]

        AudioMixerGroup m_AudioMixerGroup;

        void Start()
        {

            m_AudioSources = FindObjectsOfType<AudioSource>();

            for (int i = 0; i < m_AudioSources.Length; i++)
            {
                m_AudioSources[i].outputAudioMixerGroup = m_AudioMixerGroup;
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
        // Update is called once per frame

    }
}
