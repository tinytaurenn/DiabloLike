using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class S_ImageCorrectionPP : MonoBehaviour
    {
        [SerializeField]
        private Material m_imageCorrectionMat;


        private void Start()
        {
            m_imageCorrectionMat = new(m_imageCorrectionMat);
            m_imageCorrectionMat.SetFloat("_Brightness", PlayerPrefs.GetFloat("Brightness"));
            m_imageCorrectionMat.SetFloat("_Gamma", PlayerPrefs.GetFloat("Gamma"));
        }

        public void ChangeBrightness(float value)
        {
            m_imageCorrectionMat.SetFloat("_Brightness", value);
            PlayerPrefs.SetFloat("Brightness", value);
        }

        public void ChangeGamma(float value)
        {
            m_imageCorrectionMat.SetFloat("_Gamma", value);
            PlayerPrefs.SetFloat("Gamma", value);
        }

        private void Update()
        {
            
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (m_imageCorrectionMat)
            {
                Graphics.Blit(source, destination, m_imageCorrectionMat);
            }
            else
            {
                Graphics.Blit(source, destination);
                print("Missing Post Process material");
            }
        }
    }
}
