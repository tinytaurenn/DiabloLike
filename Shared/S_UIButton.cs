using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Shared
{
    public class S_UIButton : S_UIItem, IDeselectHandler
    {
        [SerializeField] Vector2 m_hilightScale = new(0.8f, 1f);
        [SerializeField] Color m_hilightColor;
        [SerializeField] Text m_text;
        [SerializeField] bool m_deselectOnSubmit = false;
        Color m_colorBuffer;

        private void Awake()
        {
            if (m_text)
                m_colorBuffer = m_text.color;
        }
        public override void OnSelect(BaseEventData eventData)
        {
            S_UIManager.BtnWasSelected(eventData.selectedObject); 
            if (m_text)
                m_text.color = m_hilightColor;
            transform.localScale = Vector3.one * m_hilightScale.y;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (m_text)
                m_text.color = m_colorBuffer;
            transform.localScale = Vector3.one * m_hilightScale.x;
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            if (m_deselectOnSubmit) OnDeselect(eventData);
        }
    }
}
