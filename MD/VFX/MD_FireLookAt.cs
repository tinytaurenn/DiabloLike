using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace DumortierMatthieu
{

    
    public class MD_FireLookAt : MonoBehaviour
    {
        Camera m_Cam;
        Renderer m_Renderer;

        [SerializeField]
        bool m_ReverseLookAt = false;

        [SerializeField]
        bool m_IsLockY = true; 

        Quaternion m_BaseRotation;

        [SerializeField]
        bool m_IsAddingBaseRotation = false; 

        MaterialPropertyBlock m_MaterialPropertyBlock;
        private void Awake()
        {
            m_Renderer = GetComponent<Renderer>(); 
            m_Cam = Camera.main;
            m_BaseRotation = transform.rotation; 


        }
        private void OnEnable()
        {
            m_Cam = Camera.main;

            m_MaterialPropertyBlock = new MaterialPropertyBlock();

            Vector3 randomPos = Random.insideUnitSphere;
            //print("test fire "); 

            m_MaterialPropertyBlock.SetVector("_CameraPosition",Random.Range(0f, 100f) * randomPos);

            m_Renderer.SetPropertyBlock(m_MaterialPropertyBlock);
            
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 pos; 

            if (m_IsLockY)
            {
                 pos = (new Vector3(m_Cam.transform.position.x, transform.position.y, m_Cam.transform.position.z));
            }
            else
            {
                 pos = (new Vector3(m_Cam.transform.position.x, m_Cam.transform.position.y, m_Cam.transform.position.z));
            }

            

            if (m_ReverseLookAt)
            {
                transform.LookAt(transform.position - (pos - transform.position));

                
            }
            else
            {
                transform.LookAt(pos);
            }


            if (m_IsAddingBaseRotation)
            {

                Vector3 rotation = transform.rotation.eulerAngles;
                rotation = m_BaseRotation.eulerAngles + rotation;

                transform.rotation = Quaternion.Euler(rotation);
            }






        }


       
    }
}
