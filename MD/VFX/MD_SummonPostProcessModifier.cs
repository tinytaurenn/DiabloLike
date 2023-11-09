using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{
    public class MD_SummonPostProcessModifier : MonoBehaviour
    {

        [SerializeField]
        bool m_Toggle = false;

        [SerializeField]
        bool m_IsShaderEnabled = false;

        [SerializeField]
        float m_LerpSpeed = 0.5f; 

        [SerializeField]
        Material m_PostProcessMaterial; 

        MaterialPropertyBlock m_PropertyBlock;

        string m_OverSeeIntensity = "_OverSeeIntensity";

        string m_GlobalLerpFloat = "_GlobalLerp";

        string m_Step = "_Step";

        //activated

        const float m_OverSeeValue = 10f;

        const float m_StepValue = 0.5f;

        const float m_GlobalLerp = 0f;

        private void Start()
        {
            m_PostProcessMaterial.SetFloat(m_Step, 1);

            m_PostProcessMaterial.SetFloat(m_GlobalLerpFloat, 1);
        }

        private void Update()
        {
            //if (m_Toggle)
            //{
            //    ActivateInstant(); 

            //    m_Toggle = false;
            //}

            if (m_IsShaderEnabled) 
            { 
                EnabledUpdate();
            }
            else
            {
                DisabledUpdate();
            }
        }

        void ActivateInstant()
        {
            m_PostProcessMaterial.SetFloat(m_OverSeeIntensity, m_OverSeeValue); 

            m_PostProcessMaterial.SetFloat(m_Step, m_StepValue);

            m_PostProcessMaterial.SetFloat(m_GlobalLerpFloat, m_GlobalLerp);
        }

         void DesactivateInstant()
        {
            m_PostProcessMaterial.SetFloat(m_OverSeeIntensity, 1);

            m_PostProcessMaterial.SetFloat(m_Step, 1);

            m_PostProcessMaterial.SetFloat(m_GlobalLerpFloat, 1);
        }

        public IEnumerator SwitchActivate(float time)
        {

            m_IsShaderEnabled = !m_IsShaderEnabled; 

            m_Toggle = true; 
            yield return new WaitForSeconds(time);

            m_Toggle = false; 


        }

        public IEnumerator SwitchActivate(float time, bool enabled)
        {

            m_IsShaderEnabled = enabled; 

            m_Toggle = true;
            yield return new WaitForSeconds(time);

            m_Toggle = false;

            if (enabled == false)
            {
                DesactivateInstant();

            }


        }

        void EnabledUpdate()
        {
            if (m_Toggle == false)
            {
                return; 
            }

            m_PostProcessMaterial.SetFloat(m_OverSeeIntensity, Mathf.Lerp(m_PostProcessMaterial.GetFloat(m_OverSeeIntensity), m_OverSeeValue, m_LerpSpeed * Time.deltaTime));

            m_PostProcessMaterial.SetFloat(m_Step, Mathf.Lerp(m_PostProcessMaterial.GetFloat(m_Step), m_StepValue, m_LerpSpeed * Time.deltaTime));

            m_PostProcessMaterial.SetFloat(m_GlobalLerpFloat, Mathf.Lerp(m_PostProcessMaterial.GetFloat(m_GlobalLerpFloat), m_GlobalLerp, m_LerpSpeed * Time.deltaTime));
        }

         void DisabledUpdate()
        {

            if (m_Toggle == false)
            {
                return;
            }
            m_PostProcessMaterial.SetFloat(m_OverSeeIntensity, Mathf.Lerp(m_PostProcessMaterial.GetFloat(m_OverSeeIntensity), 1, m_LerpSpeed * Time.deltaTime));

            m_PostProcessMaterial.SetFloat(m_Step, Mathf.Lerp(m_PostProcessMaterial.GetFloat(m_Step), 1, m_LerpSpeed * Time.deltaTime));

            m_PostProcessMaterial.SetFloat(m_GlobalLerpFloat, Mathf.Lerp(m_PostProcessMaterial.GetFloat(m_GlobalLerpFloat), 1, m_LerpSpeed * Time.deltaTime));
        }

    }
}
