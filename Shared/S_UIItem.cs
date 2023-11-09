using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shared
{
    public class S_UIItem : MonoBehaviour, ISelectHandler, ICancelHandler, ISubmitHandler
    {
        public virtual void OnSelect(BaseEventData eventData)
        {
            S_UIManager.BtnWasSelected(eventData.selectedObject); 
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            S_UIManager.BtnWasCancel();
            S_UIMenuLayer cg;
            if (transform.parent.TryGetComponent(out cg))
            {
                S_UIManager.s_instance.ChangeCanvas(cg.PreviousMenu);
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            S_UIManager.BtnWasClicked();
        }
    }
}
