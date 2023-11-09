using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DumortierMatthieu
{


    [ExecuteInEditMode, ImageEffectAllowedInSceneView] // important for debuging, allow the PP to execute in editor 
    public class MD_SummonPPScript : MonoBehaviour
    {
        //[SerializeField]
        //protected Material m_PostProcessMaterial;

        [SerializeField]
        protected Material m_PostProcessMaterial;



        protected virtual void Update()
        {

        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (m_PostProcessMaterial)
            {
                Graphics.Blit(source, destination, m_PostProcessMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
                print("There is no Post Process Material ");
            }
        }
    }
}
