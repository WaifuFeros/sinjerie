using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WMG.Utilities
{
    [DisallowMultipleComponent]
    [SelectionBase]
    public class TabButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        public TabGroup tabGroup;
        public Sprite SelectedImage;

        private Image background;
        
        public void OnPointerClick(PointerEventData eventData) => Press();
        public void OnSubmit(BaseEventData eventData) => Press();

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            SelectTab();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            SelectTab();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            DeselectTab();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            DeselectTab();
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            tabGroup.OnTabSelected(this);
        }

        private void SelectTab()
        {
            tabGroup.OnTabEnter(this);
        }

        private void DeselectTab()
        {
            tabGroup.OnTabExit(this);
            
        }

        protected new void OnValidate()
        {
            if (tabGroup == null)
            {
                if (transform.parent.TryGetComponent(out TabGroup group))
                    tabGroup = group;
                else
                    Debug.LogWarning("Tab Button didn't found a Tab Group component", this);
            }

            if (background == null)
            {
                if (TryGetComponent(out Image image))
                {
                    background = image;
                }
            }
        }
    }
}
