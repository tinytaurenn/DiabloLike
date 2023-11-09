using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class S_AudioListener : MonoBehaviour
    {
        [SerializeField] float m_findTargetFreq = 0.5f;
        Transform m_target;
        bool m_inCoroutine = false;


        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            S_GameManager.OnPlayerSwitch += UpdateTarget;
            S_GameManager.OnPlayerAfterRevive += UpdateTarget;
            transform.forward = Camera.main.transform.forward;
            UpdateTarget();
        }

        private void LateUpdate()
        {
            if (m_target) transform.position = m_target.position;
        }

        private void UpdateTarget()
        {
            if (S_GameManager.Instance.CurrentPlayer != null) // Set new target :
            {
                m_target = S_GameManager.Instance.CurrentPlayer.transform;
            }
            else if (!m_inCoroutine) StartCoroutine(WaitNextUpdate());

        }

        private IEnumerator WaitNextUpdate()
        {
            m_inCoroutine = true;
            yield return new WaitForSeconds(m_findTargetFreq);
            m_inCoroutine = false;
            UpdateTarget();
        }
    }
}
