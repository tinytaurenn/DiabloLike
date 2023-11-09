
using Shared;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace DumortierMatthieu
{
    public class MD_PotionEffectScript : MonoBehaviour
    {
        public float PotionLifeTime { get; set; } = 8f; 

        float m_CreationTime = 0f; 
        [SerializeField]
        AnimationCurve m_SmokeLightCurve; 

        [SerializeField]
        Light[] m_Lights; 

        [SerializeField]
        float m_PotionDistance = 1.3f;
        [SerializeField]
        int m_EffectValue = 40;
        [SerializeField]
        float m_PotionEffectDuration = 5f;
        [SerializeField]
        float m_PotionTicTime = 0.5f;
        [SerializeField]
        float m_PotionDelay = 0.3f;

        [SerializeField]
        GameObject m_HealEffect; 


        [SerializeField]
        LayerMask m_EnemyMask;


        [Space(10)]
        [Header("SOUNDS")]
        [SerializeField]
        MD_SoundManager m_HealFeedBackSound;
        [SerializeField]
        MD_SoundManager m_PotionBreakSound;

        bool m_IsHealingAudio = false;

        [SerializeField]
        float m_MaxVolume = 0.6f;

        [SerializeField]
        Vector2 m_volumeFadeTime = new Vector2(0.5f, 2f); 


        void Start()
        {

            StartCoroutine(PotionEffectRoutine(transform.position, m_PotionEffectDuration, m_PotionTicTime, m_PotionDelay));

            StartCoroutine(LightCurveRoutine(PotionLifeTime)); 
        }
        private void OnEnable()
        {
            m_CreationTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            
        
        }

        IEnumerator LightCurveRoutine(float time)
        {
            float i = 0f;
            float rate = 1 / time; 

            while (i < 1)
            {
                foreach (var item in m_Lights)
                {
                    item.intensity = m_SmokeLightCurve.Evaluate(i);
                }
                i += Time.deltaTime * rate;
                yield return 0; 
            }
        }

      

        IEnumerator PotionEffectRoutine(Vector3 pos, float time, float ticTime, float delay)
        {



            yield return new WaitForSeconds(delay);

            float numberOfTic = time / ticTime;
            int damageValue = (int)(m_EffectValue / numberOfTic);


            while (numberOfTic > 0)
            {
                yield return new WaitForSeconds(ticTime);
                numberOfTic--;
                //print("potion tic");

                PotionTicEffect(pos, damageValue);


            }






        }

        void PotionTicEffect(Vector3 pos, int damage)
        {
            //print("potion tic damage ");
            Collider[] nearbyEnemies = Physics.OverlapSphere(pos, m_PotionDistance, m_EnemyMask);

            //print("potion tic enemies : " + nearbyEnemies.Length);

            foreach (Collider enemy in nearbyEnemies)
            {
                

                if (enemy.TryGetComponent<IEntityStyle>(out IEntityStyle component))
                {

                    if (component.GetStyle() == IEntityStyle.Style.Good)
                    {
                        if (enemy.TryGetComponent<IDamageable>(out IDamageable damageableComponent))
                        {
                            print("potion tic effect on good");
                            damageableComponent.TakeDamage(damage, IDamageable.EAttackSource.Heal);
                        }

                    }
                    else if (component.GetStyle() == IEntityStyle.Style.Evil)
                    {
                        if (enemy.TryGetComponent<IHealable>(out IHealable healbleComponent))
                        {
                            //print("potion tic effect on evil");
                            healbleComponent.Heal(damage);

                            if (!m_IsHealingAudio)
                            {
                                m_IsHealingAudio = true;
                                m_HealFeedBackSound.AddAndActivate(transform);

                                StartCoroutine(m_HealFeedBackSound.UnFadeAudioSource(m_volumeFadeTime.x, m_MaxVolume));

                                

                                StartCoroutine(m_HealFeedBackSound.FadeAudioSource(m_volumeFadeTime.y));

                            }

                            


                            var Heal = Instantiate(m_HealEffect, enemy.transform.position, m_HealEffect.transform.rotation); 
                            Heal.GetComponent<MD_HealPSScript>().MeshRenderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();

                        }

                        

                    }
                }
                else
                {
                    if (enemy.TryGetComponent<IDamageable>(out IDamageable damageableComponent))
                    {
                        print("potion tic effect enemy");
                        damageableComponent.TakeDamage(damage, IDamageable.EAttackSource.Heal);
                    }
                }





            }

            
        }



        private void OnDrawGizmos()
        {

            Gizmos.color = Color.gray; 
            Gizmos.DrawWireSphere(transform.position, m_PotionDistance);
        }

        private void OnParticleCollision(GameObject other)
        {
            m_PotionBreakSound.Activation(transform); 
        }
    }
}
