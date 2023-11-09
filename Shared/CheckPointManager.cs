using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{

    
    public class CheckPointManager : MonoBehaviour
    {
        

        [SerializeField]
        Vector3 m_LastCheckPointPosition; 

        [SerializeField]
        bool IsShowGizmos = true; 
        
        public List<Transform> m_CheckPointList = new List<Transform>();

        public int m_ActualIndex = 0; 

        private void Awake()
        {

            

            int i = 0; 
            foreach (var item in GetComponentsInChildren<Transform>())
            {
                if (item != this.transform)
                {
                    m_CheckPointList.Add(item);
                    item.GetComponent<CheckPoint>().m_IndexInParentList = i;
                    i++;                }

                
            }
        }
        private void OnEnable()
        {
            
        }
        private void OnDisable()
        {

            
            m_CheckPointList.Clear(); 
        }
        void Start()
        {
            S_GameManager.Instance.SetCheckPointManager(this);
            m_LastCheckPointPosition = m_CheckPointList[0].position;

            Spawn(); 
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void Spawn()
        {
            if (m_CheckPointList.Count <= 0 )
            {
                Debug.LogError(" pas de checkpoints config dans le manager "); 
            }

            S_GameManager.Instance.Spawn(m_CheckPointList[0].position);
            

            
        }

        public void SetLastPos(Vector3 pos)
        {
            m_LastCheckPointPosition = pos; 
        }

        public Vector3 NextCheckPointPos()
        {

            if (m_CheckPointList.Count <= 0 )
            {
                return m_LastCheckPointPosition; 
            }

            int nextIndex = Mathf.Clamp(m_ActualIndex -1, 0, m_CheckPointList.Count -1);

            return m_CheckPointList[nextIndex].transform.position; 
            //return Vector3.zero; 
        }

        public Vector3 LastCheckPoint()
        {
            return m_LastCheckPointPosition; 
        }

        private void OnDrawGizmos()
        {
            if (IsShowGizmos)
            {

                Gizmos.color = Color.yellow; 

                foreach (var item in GetComponentsInChildren<Transform>())
                {
                    if (item != this.transform)
                    {
                        Gizmos.DrawWireCube(item.position, Vector3.one / 2);
                    }

                   
                }


            }
        }
    }
}
