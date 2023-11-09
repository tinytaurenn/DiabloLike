using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DumortierMatthieu
{
    public class MD_PuzzleBridge : MonoBehaviour


    {

        Rigidbody m_RigidBody;

        [SerializeField]
        Animator m_Animator;

        

        [SerializeField]
        float m_BlockingPathDestroyTime = 6f;

        [SerializeField]
        GameObject m_BlockingPathGameObject;

        [SerializeField]
        MeshCollider m_PuzzleBridgeCollider;

        [SerializeField]
        GameObject m_BridgeFallSmokeEmbergen;

        [SerializeField]
        GameObject m_BridgeFallPS; 

        [Space(10)]
        [Header("SOUNDS")]
        [Space(10)]
        [SerializeField]
        MD_SoundManager m_BridgeFallSound;
        [SerializeField]
        MD_SoundManager m_DestructionSound;


        const float CAMERASHAKE_AMPLITUDE = 1f;
        const float CAMERASHAKE_SPRING = 0.35f;
        const float CAMERASHAKE_RECOIL = 0.35f;
        const float CAMERASHAKE_DAMPING = 0.4f;



        private void Awake()
        {
            m_RigidBody = GetComponentInChildren<Rigidbody>();
        }

        void Start()
        {

            //PuzzleValidationTrigger();
        }

        public void PuzzleValidationTrigger(float delay)
        {
           
            Invoke(nameof(PuzzleValidation), delay);

        }

        void PuzzleValidation()
        {
            m_Animator.enabled = true;



            Destroy(m_BlockingPathGameObject, m_BlockingPathDestroyTime);

            m_DestructionSound.Activation(transform);
            CameraShake(); 

            Invoke(nameof(PuzzleBridgeColliderEnable), m_BlockingPathDestroyTime); 
        }

        void CameraShake()
        {
            CameraManager.Instance.GetComponent<MD_CameraShake>().Shake(CAMERASHAKE_AMPLITUDE, CAMERASHAKE_SPRING, CAMERASHAKE_DAMPING, CAMERASHAKE_RECOIL);
        }

        void BridgeFallSmoke()
        {
            m_BridgeFallSmokeEmbergen.SetActive(true);
            m_BridgeFallPS.SetActive(true);
        }

        void BridgeFallSound()
        {
            m_BridgeFallSound.Activation(transform); 
        }

        void PuzzleBridgeColliderEnable()
        {

            m_PuzzleBridgeCollider.enabled = true; 
            

        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
