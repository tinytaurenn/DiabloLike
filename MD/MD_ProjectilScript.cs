using Shared;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DumortierMatthieu
{
    public  abstract class  MD_ProjectilScript : MonoBehaviour
    {

        [SerializeField]
        protected int m_ProjectilDamage = 15;

        public int Damage
        {
            get
            {
                return m_ProjectilDamage;
            }
            set
            {
                m_ProjectilDamage = value;
            }
        }

        [SerializeField] protected float m_Speed = 5f;

        [SerializeField] protected float m_DestroyTime = 3f;



        //protected virtual 

        protected virtual void Start()
        {
            Destroy(this.gameObject, m_DestroyTime);
        }

        protected abstract void DealDamage(GameObject entity);


        


    }
}
