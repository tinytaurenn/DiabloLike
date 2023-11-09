using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shared
{
    public class S_UIMenuLayer : MonoBehaviour
    {
        private void OnEnable()
        {
            S_GameManager.OnPause += UI_Active;
        }

        private void OnDisable()
        {
            S_GameManager.OnPause -= UI_Active;
        }

        void UI_Active(bool active)
        {
            if (active == false)
            {
                GetComponentInParent<S_InGameMenuManager>(active).HideParameters();
            }
           
           
        }

        [SerializeField] CanvasGroup m_previousMenu;

        public CanvasGroup PreviousMenu => m_previousMenu;


    }
}
