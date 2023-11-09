using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace Shared
{
    public class S_BtnMenu : MonoBehaviour, ISelectHandler, ICancelHandler, ISubmitHandler, IDeselectHandler
    {
        [SerializeField] Text m_text;
        [SerializeField] Vector2 m_hilightScale = new(0.8f, 1f);
        [SerializeField] Color m_hilightColor = new();
        [SerializeField] UnityEvent m_onClickAction;
        [SerializeField] UnityEvent m_onSelectAction;
        [SerializeField] UnityEvent m_onCancelAction;
        [SerializeField] bool m_deselectOnSubmit = false;
        Color m_defaultColor = new();

        private void Awake()
        {
            if (m_text) m_defaultColor = m_text.color;
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            if (m_text) m_text.color = m_hilightColor;
            m_onSelectAction?.Invoke();
            transform.localScale = Vector3.one * m_hilightScale.y;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (m_text)  m_text.color = m_defaultColor;
            transform.localScale = Vector3.one * m_hilightScale.x;
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (m_deselectOnSubmit) OnDeselect(eventData);
            m_onClickAction?.Invoke();
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            m_onCancelAction?.Invoke();
        }
    }
}
