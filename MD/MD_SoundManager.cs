using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using System.Linq;
using Shared;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.Audio;


namespace DumortierMatthieu
{
    [Serializable]
    public class MD_SoundManager
    {

        //code Inspired by FXEnhancer by Gil Damoiseaux

        [SerializeField]

        AudioClip[] m_AudioClips = new AudioClip[3]; 

        [SerializeField]
        [Range(0f, 10f)]
        float m_SoundDelay = 0f;

        [MinMaxSlider(-3, 3f)]
        [SerializeField]
        Vector2 m_PitchRange = new (0.8f, 1.2f);

        [MinMaxSlider(0, 1f)]
        [SerializeField]
        Vector2 m_VolumeRange = new (0.5f, 0.9f);

        [SerializeField]
        [Range(1, 250)]
        int m_Priority = 120; 

        [SerializeField]
        float m_MaxDistance = 50f; 

        AudioSource m_OneaudioSource;

        [SerializeField]
        AudioMixerGroup m_AudioMixerGroup;

           


        public void Activation(Transform pos)
        {

            
           
           

            GameObject gameObject = new GameObject("AudioTemp"); 
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = MD_PlayerAudioListener.Instance.GetAudio();

            int clipIndex = UnityEngine.Random.Range(0, m_AudioClips.Length);

            gameObject.transform.position = pos.position;

            audioSource.priority = m_Priority; 
            audioSource.spatialBlend = 1f; 
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 1;
            audioSource.maxDistance = m_MaxDistance; 
               

            audioSource.clip = m_AudioClips[clipIndex];
            audioSource.pitch = UnityEngine.Random.Range(m_PitchRange.x, m_PitchRange.y);
            audioSource.volume = UnityEngine.Random.Range(m_VolumeRange.x, m_VolumeRange.y);

            audioSource.PlayDelayed(m_SoundDelay);

            UnityEngine.Object.Destroy(gameObject, m_AudioClips[clipIndex].length *  audioSource.pitch);


            

            ///


            

            

        }

        public void Activation(Transform pos, bool isOneAudioSource)
        {
           
            
            

            if (isOneAudioSource)
            {
                
                int clipIndex = UnityEngine.Random.Range(0, m_AudioClips.Length);

                m_OneaudioSource = pos.gameObject.GetComponent<AudioSource>();

                m_OneaudioSource.outputAudioMixerGroup = MD_PlayerAudioListener.Instance.GetAudio();

                m_OneaudioSource.priority = m_Priority;
                m_OneaudioSource.clip = m_AudioClips[clipIndex];
                m_OneaudioSource.pitch = UnityEngine.Random.Range(m_PitchRange.x, m_PitchRange.y);
                m_OneaudioSource.volume = UnityEngine.Random.Range(m_VolumeRange.x, m_VolumeRange.y);

                m_OneaudioSource.PlayDelayed(m_SoundDelay);


                


            }
            else
            {

                GameObject gameObject = new GameObject("AudioTemp");
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                int clipIndex = UnityEngine.Random.Range(0, m_AudioClips.Length);

                audioSource.outputAudioMixerGroup = MD_PlayerAudioListener.Instance.GetAudio();

                gameObject.transform.position = pos.position;


                audioSource.priority = m_Priority;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.minDistance = 1;
                audioSource.maxDistance = m_MaxDistance;

                audioSource.clip = m_AudioClips[clipIndex];
                audioSource.pitch = UnityEngine.Random.Range(m_PitchRange.x, m_PitchRange.y);
                audioSource.volume = UnityEngine.Random.Range(m_VolumeRange.x, m_VolumeRange.y);

                audioSource.PlayDelayed(m_SoundDelay);

                UnityEngine.Object.Destroy(gameObject, m_AudioClips[clipIndex].length * audioSource.pitch);
            }
            




            ///




        }


        public AudioSource AddAndActivate(Transform pos)
        {
           
            
            m_OneaudioSource = pos.gameObject.AddComponent<AudioSource>();

            int clipIndex = UnityEngine.Random.Range(0, m_AudioClips.Length);

            m_OneaudioSource.outputAudioMixerGroup = MD_PlayerAudioListener.Instance.GetAudio();


            m_OneaudioSource.priority = m_Priority;
            m_OneaudioSource.spatialBlend = 1f;
            m_OneaudioSource.rolloffMode = AudioRolloffMode.Linear;
            m_OneaudioSource.minDistance = 1;
            m_OneaudioSource.maxDistance = m_MaxDistance; 

            m_OneaudioSource.clip = m_AudioClips[clipIndex];
            m_OneaudioSource.pitch = UnityEngine.Random.Range(m_PitchRange.x, m_PitchRange.y);
            m_OneaudioSource.volume = UnityEngine.Random.Range(m_VolumeRange.x, m_VolumeRange.y);

            m_OneaudioSource.PlayDelayed(m_SoundDelay);

            UnityEngine.Object.Destroy(m_OneaudioSource, m_AudioClips[clipIndex].length * m_OneaudioSource.pitch);


            return m_OneaudioSource;

            ///




            

        }

        public AudioSource AddAndActivate(Transform pos, int clipChoice)
        {


            m_OneaudioSource = pos.gameObject.AddComponent<AudioSource>();

            int clipIndex = Math.Clamp(clipChoice, 0, m_AudioClips.Length-1);

            m_OneaudioSource.outputAudioMixerGroup = MD_PlayerAudioListener.Instance.GetAudio();


            m_OneaudioSource.priority = m_Priority;
            m_OneaudioSource.spatialBlend = 1f;
            m_OneaudioSource.rolloffMode = AudioRolloffMode.Linear;
            m_OneaudioSource.minDistance = 1;
            m_OneaudioSource.maxDistance = m_MaxDistance;

            m_OneaudioSource.clip = m_AudioClips[clipIndex];
            m_OneaudioSource.pitch = UnityEngine.Random.Range(m_PitchRange.x, m_PitchRange.y);
            m_OneaudioSource.volume = UnityEngine.Random.Range(m_VolumeRange.x, m_VolumeRange.y);

            m_OneaudioSource.PlayDelayed(m_SoundDelay);

            UnityEngine.Object.Destroy(m_OneaudioSource, m_AudioClips[clipIndex].length * m_OneaudioSource.pitch);


            return m_OneaudioSource;

            ///






        }

        public IEnumerator FadeAudioSource(float time)
        {

            const float VOLUME_THRESHOLD = 0.01f; 

            //Debug.Log("audiosource fade start");
            if (m_OneaudioSource == null)
            {
                //Debug.Log("audiosource is null");
                yield break; 
            }

            //float volume = audioSource.volume;

            //float i = 0f;
            float  rate = 1 / time;

            while (m_OneaudioSource.volume > VOLUME_THRESHOLD)
            {

                //Debug.Log("audiosource fading " + m_OneaudioSource.name);

                m_OneaudioSource.volume -= rate * Time.deltaTime;

                yield return 0;
            }


        }

        public IEnumerator UnFadeAudioSource(float time,float volume )
        {

            float newVolume = Mathf.Clamp01(volume);

            if (m_OneaudioSource == null)
            {
                yield break;
            }
            m_OneaudioSource.volume = 0; 
            
            float rate = 1 / time;

            while (m_OneaudioSource.volume < newVolume)
            {



                m_OneaudioSource.volume += rate * Time.deltaTime;

                yield return 0;
            }

            

            
        }


        public IEnumerator UnFadeAudioSource(float time)
        {

            float newVolume = Mathf.Clamp01(UnityEngine.Random.Range(m_VolumeRange.x, m_VolumeRange.y));

            if (m_OneaudioSource == null)
            {
                yield break;
            }
            m_OneaudioSource.volume = 0;

            float rate = 1 / time;

            while (m_OneaudioSource.volume < newVolume)
            {



                m_OneaudioSource.volume += rate * Time.deltaTime;

                yield return 0;
            }

            
        }

        public void DestroyAudio(float time)
        {
            UnityEngine.Object.Destroy(m_OneaudioSource, time);
        }
        public void DestroyAudio( )
        {
            
            UnityEngine.Object.Destroy(m_OneaudioSource);
           
        }

       














    }
}
