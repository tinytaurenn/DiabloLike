using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class InstancingOnStart : MonoBehaviour
    {
        [SerializeField]
        GameObject m_DebugPlayerPrefab;
        [SerializeField]
        Transform m_SpawnPos;
        [SerializeField]
        float m_GizmosSize = 0.1f;

        void Awake()
        {
            Instantiate(m_DebugPlayerPrefab, m_SpawnPos.position, m_DebugPlayerPrefab.transform.rotation); 
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue; 
            Gizmos.DrawCube(transform.position + Vector3.up, Vector3.one * m_GizmosSize); 
        }
    }
    
}
