using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_RepeatSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject m_GameObjectToSpawn;

        [SerializeField]
        float m_RepeatTime = 5f;

        float m_Timer = 0f;

        [SerializeField]
        bool m_IsForcedDestroyed = true;

        [SerializeField]
        float m_DestructionTime = 5f; 

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (m_GameObjectToSpawn == null) return;

            if (Time.time > m_Timer + m_RepeatTime)
            {

                GameObject instantiatedObject  =  Instantiate(m_GameObjectToSpawn, transform.position, m_GameObjectToSpawn.transform.rotation);

                m_Timer = Time.time;

                if (m_IsForcedDestroyed)
                {
                    Destroy(instantiatedObject, m_DestructionTime); 
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(transform.position, Vector3.one); 
        }
    }
}
