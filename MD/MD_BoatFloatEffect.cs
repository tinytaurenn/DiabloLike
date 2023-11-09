using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DumortierMatthieu
{

    
    public class MD_BoatFloatEffect : MonoBehaviour
    {

        [SerializeField]
        float m_RotateForce = 1f ;
        [SerializeField]
        float m_DeepForce = 1f ;

        [SerializeField]
        float m_DeepLerp;
        [SerializeField]
        AnimationCurve m_DeepCurve;


        [SerializeField]
        float m_RotateLerp;
        [SerializeField]
        AnimationCurve m_RotateCurve;

        Vector3 m_BasePos;
        Vector3 m_BaseRot;

        [SerializeField]
        Vector3 m_FixRot;

        float m_RandomTimeRot;
        float m_RandomTimePos; 

       


        

        void Start()
        {
            //m_SpecialRotationValue.Normalize();

            m_BasePos = transform.position; 
            m_BaseRot = transform.rotation.eulerAngles;


            m_RandomTimePos = Random.Range(1, 99); 
            m_RandomTimeRot = Random.Range(1, 99); 

        
        }

        // Update is called once per frame
        void Update()
        {
            
            BoatRotation();
            BoatDeep();
        
        }

       

        void BoatRotation()
        {
            

            Vector3 newRotation = m_BaseRot;

            float rotateForce = m_RotateCurve.Evaluate(Mathf.Repeat((Time.time  + m_RandomTimeRot )* m_RotateLerp, 1));

            rotateForce *= m_RotateForce;

           

            newRotation.x = rotateForce;

            var rotate2 = Quaternion.Euler(m_FixRot); 
            


            //m_DeepLerp = rotateForce; 

            transform.rotation = Quaternion.Euler(newRotation + m_FixRot)    ;
        }

        void BoatDeep()
        {

           
            Vector3 pos = transform.position; 



            float YPos  = m_DeepCurve.Evaluate(Mathf.Repeat(( Time.time + m_RandomTimePos) * m_DeepLerp, 1));

            YPos *= m_DeepForce; 

            pos.y = YPos +m_BasePos.y;

            transform.position = pos;

        }
    }
}
