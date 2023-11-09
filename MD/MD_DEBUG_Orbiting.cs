using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_DEBUG_Orbiting : MonoBehaviour
    {
        // Start is called before the first frame update


        [SerializeField]
        float m_TimeOffSet = 0f;
        [SerializeField]
        float m_RoamingSpeed = 5f;
        [SerializeField]
        float m_DistanceFromFocus = 3f;
        [SerializeField]
        Transform m_Focus;
        [SerializeField]
        float m_LerpSpeed = 1f;

        float m_XValue = 0f;
        float m_YValue = 0f;

        void Update()
        {

            Orbiting(); 
        
        }


        void Orbiting()
        {
            float x = Mathf.Cos(Time.time * m_RoamingSpeed + m_TimeOffSet ) * m_DistanceFromFocus;
            float y = Mathf.Sin(Time.time * m_RoamingSpeed + m_TimeOffSet ) * m_DistanceFromFocus;
            Vector3 targetPosition = new Vector3(x, y, 0) + m_Focus.position;

            //transform.position = targetPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition, m_LerpSpeed * Time.deltaTime);
        }
    }
}
