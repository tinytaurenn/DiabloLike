using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DumortierMatthieu
{
    public class MD_PlayerAudioListener : MonoBehaviour
    {
        [SerializeField]
        AudioMixerGroup m_AudioMixerGroup;

        public static MD_PlayerAudioListener Instance { get; private set; }

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
        }
        void Start()
        {


        }

        public AudioMixerGroup GetAudio()
        {
            return m_AudioMixerGroup;
        }
    }
}
