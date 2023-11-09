using DumortierMatthieu.AI;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_SummonRenderTextureCamera : MonoBehaviour
    {

        Transform m_Playerpos; 

        Transform m_CameraPos;
        [SerializeField]
        Vector3 m_OffSet = Vector3.zero;



        private void Awake()
        {
            m_CameraPos = Camera.main.transform;
        }
        void Start()
        {
            try
            {
                m_Playerpos = S_GameManager.Instance.CurrentPlayer.transform;
            }
            catch (UnityEngine.UnassignedReferenceException)
            {

                print(" pas assigné le player pour le render texture"); 
            }
           

            if (m_Playerpos == null) return; 

            transform.position = AIMethods.PointBetweenToPoint(m_Playerpos.position, m_CameraPos.position, 0.5f);
        
        }

        // Update is called once per frame
        void Update()
        {

            
            

           

            if (m_Playerpos ==null)
            {
                m_Playerpos = S_GameManager.Instance.CurrentPlayer.transform; 
                return; 
            }

            Vector3 relativePos = m_Playerpos.position - transform.position;

            
            transform.SetPositionAndRotation(AIMethods.PointBetweenToPoint(m_Playerpos.position, m_CameraPos.position, 0.5f) + m_OffSet,
                Quaternion.LookRotation(relativePos + m_OffSet));

        }
    }
}
