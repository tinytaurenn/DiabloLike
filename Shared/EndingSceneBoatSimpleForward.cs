using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class EndingSceneBoatSimpleForward : MonoBehaviour
    {
        [SerializeField]
        private float m_Speed;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

            transform.position += Vector3.forward * m_Speed * Time.deltaTime; 
        
        }
    }
}
