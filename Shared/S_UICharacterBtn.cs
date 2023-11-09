using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace Shared
{
    public class S_UICharacterBtn : MonoBehaviour, ISelectHandler
    {
        [SerializeField] EventSystem m_eventSystem;
        [SerializeField] GameObject m_redirectFocus;
        [SerializeField] UnityEvent m_onSelectAction;
        // [SerializeField] Vector2 m_hilightSize;
        [SerializeField] float m_duration = 0.2f;
        [SerializeField] Image m_img;
        [SerializeField] Sprite m_disable;
        [SerializeField] Sprite m_enable;

        bool m_isAnimating = false;
        [SerializeField] Vector3 m_hilightScale = Vector3.one;
        [SerializeField] Vector3 m_defaultScale = Vector3.one;
        [SerializeField] Color m_color;

        public void OnSelect(BaseEventData eventData)
        {
            if (m_isAnimating) return;
            S_UIManager.BtnWasSelected(eventData.selectedObject);
            m_onSelectAction?.Invoke();
            if (m_eventSystem.alreadySelecting) StartCoroutine(TrySelect());
            else m_eventSystem.SetSelectedGameObject(m_redirectFocus);
            StartCoroutine(SelectAnimation());
        }

        IEnumerator SelectAnimation()
        {
            if (!m_isAnimating)
            {
                m_isAnimating = true;
                m_img.sprite = m_enable;
                m_img.color = m_color;
                float timer = 0f;
                while (timer <= m_duration)
                {
                    timer += Time.unscaledDeltaTime;
                    float sin = Mathf.Sin(Mathf.Clamp01(timer / m_duration) * Mathf.PI);
                    float x = sin * sin;

                    m_img.transform.localScale = Vector3.Lerp(m_defaultScale, m_hilightScale, x);

                    yield return new WaitForEndOfFrame();
                }
                m_img.sprite = m_disable;
                m_isAnimating = false;
                m_img.color = Color.white;
            }
        }

        IEnumerator TrySelect()
        {
            bool set = false;
            while (!set)
            {
                if (!m_eventSystem.alreadySelecting)
                {
                    m_eventSystem.SetSelectedGameObject(m_redirectFocus);
                    set = true;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
