using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMG.Utilities
{
    public class TabGroup : MonoBehaviour
    {
        public TabButton SelectedTab { get; private set; }

        [SerializeField] private List<TabPageAssociation> pages;
        
        private Dictionary<TabButton, GameObject> _pageDictionary;

        private void Start()
        {
            _pageDictionary = new Dictionary<TabButton, GameObject>();
            foreach (var item in pages)
            {
                _pageDictionary.Add(item.Button, item.Page);
                item.Button.tabGroup = this;
            }

            OnTabSelected(pages[0].Button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            return;
            //if (SelectedTab == null || button != SelectedTab)
            //    button.background.sprite = tabHover;
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton button)
        {
            SelectedTab = button;

            foreach (var item in _pageDictionary)
            {
                item.Value.SetActive(item.Key == SelectedTab);
            }
        }

        public void ResetTabs()
        {
            //foreach (var button in _pageDictionary)
            //{
            //    if (SelectedTab != null && button == SelectedTab)
            //        continue;
            //    //button.background.sprite = tabIdle;
            //}
        }
    }

    [System.Serializable]
    public struct TabPageAssociation
    {
        public TabButton Button;
        public GameObject Page;
    }
}
