using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;
using Unity.VisualScripting;

namespace DumortierMatthieu
{
    public class MD_RangeAttackProjectilScript : MD_ProjectilScript
    {
        [SerializeField]
        LayerMask m_EntityMask;

        [SerializeField]
        float m_OverlapRadius = 2.5f; 





        [SerializeField]
        GameObject m_ImpactParticleSystem; 

        [SerializeField]
        Transform m_TargetTransform;
       

        //[SerializeField]
        //float m_ImpactDistance = 0.5f; 

        public Transform TargetTransform
        {
            get
            {
                return m_TargetTransform;
            }
            set
            {
                m_TargetTransform = value;
            }
        }
        
        

        [SerializeField]
        float m_SizeDamageMultiplier = 0.3f; 
        [SerializeField]
        float m_GlobalSizeMultiplier = 0.5f; 

        
        [SerializeField]
        float m_LerpValue;

        [SerializeField]
        MD_SoundManager m_ExplosionSound; 

        private void Awake()
        {
            //m_Collider = GetComponent<Collider>(); 
        }


        protected override void Start()
        {
            base.Start();
            
        }

        // Update is called once per frame
        void Update()
        {
            if (m_TargetTransform ==null)
            {
                transform.position += transform.forward * Time.deltaTime * m_Speed; 
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, m_TargetTransform.position + Vector3.up, m_Speed * Time.deltaTime);

                float distancefromTarget = Vector3.Distance(transform.position, m_TargetTransform.position);

                //if (distancefromTarget <= m_ImpactDistance)
                //{
                //    DamageToEntity(m_TargetTransform.gameObject);
                //    print("projectil damage to entity target");
                //}
            }

           



            



            


        }

        void OnExplosion()
        {
            if (transform.childCount  == 0 )
            {
                return; 
            }

            Transform skullChild = transform.GetChild(0);
            skullChild.parent = null;

            var SkullPS = skullChild.GetComponent<ParticleSystem>();

            var skullMain = SkullPS.main;

            var skullEmission = SkullPS.emission;
            skullEmission.rateOverTime = 0;
            skullEmission.rateOverDistance = 0; 

            skullMain.loop = false;
            skullMain.stopAction = ParticleSystemStopAction.Destroy;

            m_ExplosionSound.Activation(transform); 
        }

        void DamageToEntity(GameObject entity)
        {
            Explosion();
            //Damage 
            DealDamage(entity); 
            Destroy(this.gameObject);
        }


        

        private void OnTriggerEnter(Collider other)
        {
            if (Physics.CheckSphere(transform.position, m_OverlapRadius, m_EntityMask))
            {
                DamageToEntity(other.gameObject);
                //print("projectil damage to entity collider");
            }
            else
            {
                Explosion(); 
                //print("projectil damage to none");
            }
        }

        void Explosion()
        {
            OnExplosion(); 

            GameObject PSObject = Instantiate(m_ImpactParticleSystem, transform.position, m_ImpactParticleSystem.transform.rotation);

            var ps = PSObject.GetComponent<ParticleSystem>().main;
            ps.startSize = m_ProjectilDamage * m_SizeDamageMultiplier * m_GlobalSizeMultiplier;
            Destroy(this.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //print("projectil collide"); 
        }

        protected override void DealDamage(GameObject entity)
        {
            if (entity.TryGetComponent<IDamageable>(out IDamageable enemy))
            {

                //enemy.TakeDamage(damage);
                enemy.TakeDamage(m_ProjectilDamage, IDamageable.EAttackSource.Range);

                //fear effect ???

            }
        }
    }
}
