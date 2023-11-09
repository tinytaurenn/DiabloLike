using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class S_EndSceneUI : MonoBehaviour
    {
        [SerializeField] S_UIManager m_ui;
        private void OnEnable()
        {
            if (S_GameManager.Instance.UI_Manager.gameObject !=null)
            {
                S_GameManager.Instance.UI_Manager.gameObject.SetActive(false);
            }

           

            m_ui.gameObject.SetActive(true);
            m_ui.CreditReading();
        }
    }
}
