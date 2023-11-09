using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_CannonBall : MonoBehaviour
    {
        Rigidbody m_RigidBody;

        [SerializeField]
        float m_SpeedForce = 15f;

        [SerializeField]
        float m_SmokeTrailDeathTime = 1f;

        [SerializeField]
        ParticleSystem m_SmokeTrail; 



        private void Awake()
        {
            m_RigidBody = GetComponent<Rigidbody>();
        }

        // Start is called before the first frame update
        void Start()
        {

            Shoot();

            Destroy(m_SmokeTrail.gameObject, m_SmokeTrailDeathTime); 
        
        }

        void Shoot()
        {
            m_RigidBody.AddForce(transform.forward * m_SpeedForce, ForceMode.Impulse);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, transform.forward * 100f); 
        }
    }
}
