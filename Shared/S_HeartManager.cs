using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using JetBrains.Annotations;

namespace Shared
{
    public class S_HeartManager : MonoBehaviour
    {
        [SerializeField] List<Image> m_hearts;
        [SerializeField] Sprite m_full;
        [SerializeField] Sprite m_empty;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            UpdateHearts(S_GameManager.Instance.Hearts);
            
        }
        private void OnEnable()
        {
            S_GameManager.OnHeartsUpdate += UpdateHearts;
        }
        private void OnDisable()
        {
            S_GameManager.OnHeartsUpdate -= UpdateHearts;
        }


        private void UpdateHearts(int value)
        {
            


            for (int i = 0; i < m_hearts.Count; i++)
            {
                m_hearts[i].sprite = (i < value ? m_full : m_empty);
            }
        }

        
    }
}
