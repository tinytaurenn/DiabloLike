using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_LineRendererSummonTrail : MonoBehaviour
    {
        [SerializeField]
        LineRenderer m_LineRenderer;

        float m_TicDistance = 0.3f;
        [SerializeField]
        int m_MaxCount = 100; 
        void Start()
        {
            
            m_LineRenderer.gameObject.transform.SetParent(null);
            m_LineRenderer.gameObject.transform.position = Vector3.zero;
            m_LineRenderer.SetPosition(0, transform.position);
        
        }

        // Update is called once per frame
        void Update()
        {

           


            if (Vector3.Distance(transform.position, m_LineRenderer.GetPosition(m_LineRenderer.positionCount - 1)) > m_TicDistance)
            {

                if (m_LineRenderer.positionCount < m_MaxCount)
                {
                    m_LineRenderer.positionCount += 1;
                    m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, transform.position);
                }
                else
                {
                    for (int i = 0; i < m_LineRenderer.positionCount-1; i++)
                    {

                       

                        m_LineRenderer.SetPosition(i, m_LineRenderer.GetPosition(i + 1));

                    }
                    m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, transform.position);
                }

                
            }
        
        }
    }
}
