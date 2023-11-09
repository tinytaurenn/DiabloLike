using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Shared
{

    
    public class CheckPoint : MonoBehaviour
    {


        //public static event Action<Vector3> OnTrigger;
        [SerializeField]
        bool m_IsDestroyedOnTrigger = true;

        public int m_IndexInParentList = 0; 

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        

        private void OnTriggerEnter(Collider other)
        {
            var checkManagerScript = transform.parent.GetComponent<CheckPointManager>(); 

            if (!other.CompareTag("Player"))
            {
                return; 
            }

            checkManagerScript.SetLastPos(transform.position);

            if (m_IsDestroyedOnTrigger)
            {
                Destroy(this.gameObject);
                checkManagerScript.m_ActualIndex = m_IndexInParentList ; 
                checkManagerScript.m_CheckPointList.Remove(this.transform);

            }

            //OnTrigger?.Invoke(transform.position);

            //print("on trigger enter ");
        }
    }
}
