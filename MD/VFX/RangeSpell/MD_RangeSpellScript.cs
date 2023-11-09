using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{

    [ExecuteInEditMode]
    public class MD_RangeSpellScript : MonoBehaviour
    {
        [SerializeField]
        ParticleSystem m_MainParticleSystem;
        [SerializeField]
        ParticleSystem m_SkullParticleSystem; 

        Renderer m_Renderer;
        MaterialPropertyBlock m_PropertyBlock;
        [SerializeField][Range(0,1)]
        public float m_VortexPower = 0.5f; 
        [SerializeField]
        float m_RotationSpeed = 0.5f; 
        [SerializeField]
        float m_RateOverTime = 0.5f;

        [SerializeField]
        float m_SkullMaxRate = 4f;
        [SerializeField]
        float m_SkullMinRate = 2f;

        private void OnEnable()
        {
            
            m_Renderer = GetComponent<Renderer>();
            m_PropertyBlock = new MaterialPropertyBlock(); 

        }
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            SetVortexPower(m_VortexPower);

            SetRateOverTime(m_RateOverTime); 
            SetSpeed(m_RotationSpeed);

            SkullEmissionRate(); 
        }

        void SetVortexPower(float value)
        {

            m_PropertyBlock.SetFloat("_SmoothStep", value);

            m_Renderer.SetPropertyBlock(m_PropertyBlock); 

        }

        void SetSpeed(float value)
        {

            AnimationCurve curve = new AnimationCurve();

            curve.AddKey(0, m_RotationSpeed * m_VortexPower);
            curve.AddKey(1, m_RotationSpeed * m_VortexPower);


            var rot = m_MainParticleSystem.rotationOverLifetime;

            rot.enabled = true; 
            rot.x = new ParticleSystem.MinMaxCurve(1f, curve, curve); 
            rot.y = new ParticleSystem.MinMaxCurve(1f, curve, curve); 
            rot.z = new ParticleSystem.MinMaxCurve(1f, curve, curve); 

            
        }

        void SkullEmissionRate()
        {
            var rate = m_SkullParticleSystem.emission;

            rate.rateOverTime = m_VortexPower;

            AnimationCurve curveMin = new AnimationCurve();
            AnimationCurve curveMax = new AnimationCurve();

            curveMin.AddKey(0, m_SkullMinRate);
            curveMin.AddKey(1, m_SkullMinRate);
            curveMax.AddKey(0, m_SkullMaxRate);
            curveMax.AddKey(1, m_SkullMaxRate);

            //rate.rateOverTime = value * m_SkullMaxRate; 
            rate.rateOverTime = new ParticleSystem.MinMaxCurve(m_VortexPower, curveMin, curveMax);
        }


        void SetRateOverTime(float value)
        {


            var rate = m_MainParticleSystem.emission;


            //rate.rateOverTime = value * m_SkullMaxRate; 
            rate.rateOverTime = m_RateOverTime;
        }
    }
}
