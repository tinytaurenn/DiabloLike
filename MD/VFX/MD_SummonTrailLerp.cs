using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_SummonTrailLerp : MonoBehaviour
    {
        [SerializeField]
        GameObject m_Player;
        [SerializeField]
        float m_LerpSpeed; 
        // Start is called before the first frame update
        void Start()
        {

            if (m_Player == null)
            {
                m_Player = S_GameManager.Instance.CurrentPlayer; 

            }

            transform.parent = null; 

        
        }

        // Update is called once per frame
        void Update()
        {

            transform.position = Vector3.Slerp(transform.position, m_Player.transform.position, m_LerpSpeed * Time.deltaTime);
        
        }
    }
}
