using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{

    //code globally provided by FXenHancer by Gil Damoiseaux 
    public class MD_CameraShake : MonoBehaviour
    {
        [SerializeField]
        Transform m_CameraTransform; 


        [SerializeField]
        bool m_IsCameraShaking = false;
        [SerializeField]
        [Range(0f, 3f)]
        float m_Amplitude = 0.4f;
        [SerializeField]
        [Range(0.05f, 2f)]
        float m_Spring = 0.5f;
        [SerializeField]
        [Range(0f, 2f)]
        float m_Recoil = 0.1f;
        [SerializeField]
        [Range(0f, 2f)]
        float m_Damping = 0.5f;


        [SerializeField]
        bool m_IsCameraItself = false; 

       

        /// ///////////////////////////////////// 
        /// /////////////////////////////////////
        /// 


        Vector2 m_CameraSpeed = Vector2.zero;
        Vector2 m_CameraPos = Vector2.zero;

        float m_CameraSpring; 

        Vector3 m_CameraRecoildSpeed  = Vector3.zero;
        Vector3 m_CameraRecoilPos  = Vector3.zero;

        float m_CameraDamping; 




        void Start()
        {

            if (m_IsCameraItself)
            {
                m_CameraTransform = transform;
            }
            else
            {
                m_CameraTransform = CameraManager.Instance.transform;

                if (m_CameraTransform == null)
                {
                    m_CameraTransform = Camera.current.transform;
                }
            }

            

            
        
        }

        // Update is called once per frame
        void Update()
        {



            if (m_CameraTransform == null)
            {
                return; 
            }


            if (m_IsCameraShaking) Shake();




            Vector3 delta = Vector3.zero;

            m_CameraSpeed = m_CameraSpeed * Mathf.Max(0f, 1f - m_CameraDamping * Time.deltaTime)- m_CameraPos * m_CameraSpring * Time.deltaTime;
            m_CameraPos += m_CameraSpeed * Time.deltaTime; 
            m_CameraRecoildSpeed = m_CameraRecoildSpeed * Mathf.Max(0f,1f - m_CameraDamping * Time.deltaTime)- m_CameraRecoilPos * m_CameraSpring * Time.deltaTime;
            m_CameraRecoilPos += m_CameraRecoildSpeed * Time.deltaTime;
            delta += m_CameraRecoilPos;
            delta += m_CameraTransform.right * m_CameraPos.x + m_CameraTransform.up * m_CameraPos.y;

            m_CameraTransform.position += delta; 


        }

        public void SimpleShake()
        {
            m_IsCameraShaking = true;
        }

        public void Shake()
        {
            const float SCALING_VALUE = 20f; 

            

            m_CameraRecoildSpeed += SCALING_VALUE * m_Recoil * Vector3.one;
            m_CameraSpring = m_Spring * 10000f;
            m_CameraDamping = m_Damping * SCALING_VALUE;
            m_CameraSpeed += UnityEngine.Random.insideUnitCircle.normalized * m_Amplitude * SCALING_VALUE;

            m_IsCameraShaking = false;

        }

        public void Shake(float amplitude)
        {
            const float SCALING_VALUE = 20f;



            m_CameraRecoildSpeed += SCALING_VALUE * m_Recoil * Vector3.one;
            m_CameraSpring = m_Spring * 10000f;
            m_CameraDamping = m_Damping * SCALING_VALUE;
            m_CameraSpeed += UnityEngine.Random.insideUnitCircle.normalized * amplitude * SCALING_VALUE;

            m_IsCameraShaking = false;

        }

        public void Shake(float amplitude, float spring,float damping,  float recoil)
        {
            const float SCALING_VALUE = 20f;



            m_CameraRecoildSpeed += SCALING_VALUE * recoil * Vector3.one;
            m_CameraSpring = spring * 10000f;
            m_CameraDamping = damping * SCALING_VALUE;
            m_CameraSpeed += UnityEngine.Random.insideUnitCircle.normalized * amplitude * SCALING_VALUE;

            m_IsCameraShaking = false;

        }


    }
}
