using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class GenericScriptableObjectBrowser<T> : EditorWindow
    where T : ScriptableObject
{
    // =========================
    // DATA
    // =========================

    protected List<T> allItems = new();
    protected List<T> filteredItems = new();

    protected List<Group> groups = new();

    private List<T> _navigationList = new();
    private int _currentIndex = -1;

    protected VisualElement listRoot;
    protected VisualElement inspectorRoot;
    protected ToolbarSearchField searchField;

    private VisualElement _currentlySelected;

    private ScrollView _itemListScrollView;

    // =========================
    // GROUP
    // =========================

    protected class Group
    {
        public IComparable key;
        public List<T> items = new();
    }

    // =========================
    // CONFIG OVERRIDE
    // =========================

    protected virtual string WindowTitle => ObjectNames.NicifyVariableName(GetType().Name);

    protected virtual bool EnableGrouping => false;

    protected virtual bool CanCreateAsset => false;

    protected virtual string CreateFolder => "";

    protected virtual string DefaultAssetName => $"New{typeof(T).Name}";

    // GROUP + SORT
    protected virtual IComparable GetGroupKey(T item) => "Default";

    protected virtual IComparable GetSortKey(T item) => item.name;

    // SEARCH
    protected virtual bool MatchesSearch(T item, string search)
        => item.name.Contains(search, StringComparison.OrdinalIgnoreCase);

    protected virtual string GetDisplayName(T item) => item.name;

    // =========================
    // INIT
    // =========================

    public void CreateGUI()
    {
        titleContent = new GUIContent(WindowTitle);

        rootVisualElement.Clear();

        var split = new TwoPaneSplitView(
            0,
            300,
            TwoPaneSplitViewOrientation.Horizontal);

        rootVisualElement.Add(split);

        var left = new VisualElement();
        var right = new VisualElement();

        split.Add(left);
        split.Add(right);

        var scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexGrow = 1;
        scroll.style.flexShrink = 1;
        // IMPORTANT: force vertical layout
        scroll.style.flexDirection = FlexDirection.Column;
        right.Add(scroll);

        inspectorRoot = scroll;

        BuildToolbar(left);
        BuildList(left);

        RefreshItems();
        ApplyFilter("");

        rootVisualElement.focusable = true;
        //rootVisualElement.RegisterCallback<MouseDownEvent>(_ =>
        //{
        //    rootVisualElement.Focus();
        //});
        rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    // =========================
    // TOOLBAR
    // =========================

    protected virtual void BuildToolbar(VisualElement parent)
    {
        var toolbar = new Toolbar();

        searchField = new ToolbarSearchField();
        searchField.style.flexGrow = 1;   // 🔥 prend toute la place dispo
        searchField.style.flexShrink = 1; // 🔥 autorise la réduction
        searchField.style.minWidth = 0;   // ⭐ IMPORTANT (corrige ton bug)

        searchField.RegisterValueChangedCallback(evt =>
        {
            ApplyFilter(evt.newValue);
        });

        toolbar.Add(searchField);

        if (CanCreateAsset)
        {
            toolbar.Add(new ToolbarButton(CreateAsset)
            {
                text = "Create"
            });
        }

        var refreshButton = new ToolbarButton(() =>
        {
            RefreshItems();
            ApplyFilter(searchField.value);
        })
        {
            text = "Refresh"
        };

        refreshButton.style.flexShrink = 0;

        toolbar.Add(refreshButton);

        parent.Add(toolbar);
    }

    // =========================
    // LIST ROOT (IMPORTANT FIX)
    // =========================

    protected virtual void BuildList(VisualElement parent)
    {
        var scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexGrow = 1;
        scroll.style.flexShrink = 1;

        // IMPORTANT: force vertical layout
        scroll.style.flexDirection = FlexDirection.Column;

        listRoot = new VisualElement();
        listRoot.style.flexDirection = FlexDirection.Column;
        listRoot.style.width = Length.Percent(100);
        listRoot.style.flexShrink = 0;
        listRoot.style.flexGrow = 0;

        scroll.Add(listRoot);
        parent.Add(scroll);

        _itemListScrollView = scroll;
    }

    // =========================
    // DATA
    // =========================

    protected virtual void RefreshItems()
    {
        allItems = AssetDatabase
            .FindAssets($"t:{typeof(T).Name}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>)
            .Where(x => x != null)
            .OrderBy(x => x.name)
            .ToList();
    }

    // =========================
    // FILTER PIPELINE
    // =========================

    protected virtual void ApplyFilter(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            filteredItems = allItems.ToList();
        else
            filteredItems = allItems
                .Where(x => MatchesSearch(x, search))
                .ToList();

        groups.Clear();

        if (EnableGrouping)
            BuildGroups();

        RebuildUI();

        // reset selection
        _currentlySelected = null;
        inspectorRoot.Clear();

        BuildNavigationList();
        _currentIndex = -1;
    }

    // =========================
    // GROUPING
    // =========================

    protected virtual void BuildGroups()
    {
        foreach (var item in filteredItems)
        {
            var key = GetGroupKey(item);

            var group = groups.FirstOrDefault(g => Equals(g.key, key));

            if (group == null)
            {
                group = new Group { key = key };
                groups.Add(group);
            }

            group.items.Add(item);
        }

        foreach (var g in groups)
        {
            g.items = g.items
                .OrderBy(GetSortKey)
                .ToList();
        }

        groups = groups
            .OrderBy(g => g.key)
            .ToList();
    }

    protected virtual void BuildNavigationList()
    {
        _navigationList.Clear();

        if (EnableGrouping)
        {
            foreach (var g in groups)
                _navigationList.AddRange(g.items);
        }
        else
        {
            _navigationList.AddRange(filteredItems);
        }
    }

    // =========================
    // UI BUILD
    // =========================

    protected virtual void RebuildUI()
    {
        listRoot.Clear();

        if (!EnableGrouping)
        {
            foreach (var item in filteredItems)
                listRoot.Add(CreateItem(item));

            return;
        }

        foreach (var group in groups)
        {
            var foldout = new Foldout
            {
                text = group.key.ToString(),
                value = true
            };

            foldout.style.unityFontStyleAndWeight = FontStyle.Bold;

            foreach (var item in group.items)
                foldout.Add(CreateItem(item));

            listRoot.Add(foldout);
        }

        BuildNavigationList();
        _currentIndex = -1;
    }

    // =========================
    // ITEM UI (STABLE FIX)
    // =========================

    protected virtual VisualElement CreateItem(T item)
    {
        var row = new VisualElement();

        // 🔥 CRITICAL: force full width column row
        row.style.flexDirection = FlexDirection.Row;
        row.style.width = Length.Percent(100);
        row.style.flexShrink = 0;
        row.style.flexGrow = 0;
        row.style.alignItems = Align.Center;
        row.userData = item;

        var label = new Label(GetDisplayName(item));

        label.style.flexGrow = 1;
        label.style.unityTextAlign = TextAnchor.MiddleLeft;

        row.Add(label);

        ApplyItemStyle(row, false);

        row.AddManipulator(new Clickable(() =>
        {
            _currentIndex = _navigationList.IndexOf(item);
            SelectItem(item, row);
        }));

        var rightClickManipulator = new Clickable(() =>
        {
            SelectItem(item, row);
            SelectItemInProject(item);
        });
        rightClickManipulator.activators.Add(new ManipulatorActivationFilter() { modifiers = EventModifiers.Control });

        row.AddManipulator(rightClickManipulator);

        // hover (safe)
        row.RegisterCallback<MouseEnterEvent>(_ =>
        {
            if (row != _currentlySelected)
                row.style.backgroundColor = new Color(1, 1, 1, 0.05f);
        });

        row.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            if (row != _currentlySelected)
                row.style.backgroundColor = Color.clear;
        });

        return row;
    }

    // =========================
    // SELECTION
    // =========================

    protected virtual void SelectItem(T item, VisualElement row)
    {
        inspectorRoot.Clear();
        inspectorRoot.Add(new InspectorElement(new SerializedObject(item)));

        SetSelected(row);
    }

    protected virtual void SelectItemInProject(UnityEngine.Object item)
    {
        if (item == null)
            return;

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = item;
        EditorGUIUtility.PingObject(item);
    }

    private void SetSelected(VisualElement row)
    {
        if (_currentlySelected != null)
            _currentlySelected.style.backgroundColor = Color.clear;

        _currentlySelected = row;

        if (_currentlySelected != null)
        {
            _currentlySelected.style.backgroundColor =
                new Color(0.24f, 0.48f, 0.9f, 0.35f);
        }
    }

    // =========================
    // CREATE
    // =========================

    protected virtual void CreateAsset()
    {
        if (string.IsNullOrEmpty(CreateFolder))
        {
            Debug.LogError("CreateFolder not set");
            return;
        }

        var asset = CreateInstance<T>();

        var path = AssetDatabase.GenerateUniqueAssetPath(
            $"{CreateFolder}/{DefaultAssetName}.asset");

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshItems();
        ApplyFilter(searchField.value);

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
    }

    // =========================
    // STYLE HOOK
    // =========================

    protected virtual void ApplyItemStyle(VisualElement element, bool selected)
    {
        element.style.paddingLeft = 12;
        element.style.paddingTop = 2;
        element.style.paddingBottom = 2;
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        Debug.Log(nameof(OnKeyDown));

        if (_navigationList == null || _navigationList.Count == 0)
            return;

        if (evt.keyCode == KeyCode.DownArrow)
        {
            _currentIndex = Mathf.Min(_currentIndex + 1, _navigationList.Count - 1);
            SelectFromKeyboard();
            evt.StopPropagation();
        }
        else if (evt.keyCode == KeyCode.UpArrow)
        {
            _currentIndex = Mathf.Max(_currentIndex - 1, 0);
            SelectFromKeyboard();
            evt.StopPropagation();
        }
    }

    private void SelectFromKeyboard()
    {
        if (_currentIndex < 0 || _currentIndex >= _navigationList.Count)
            return;

        var item = _navigationList[_currentIndex];

        var row = FindRowVisual(item);

        if (row != null)
        {
            SelectItem(item, row);

            // 🔥 scroll automatique
            _itemListScrollView.ScrollTo(row);

        }
    }

    private VisualElement FindRowVisual(T item)
    {
        return listRoot.Query<VisualElement>()
            .Where(e => e.userData != null && e.userData.Equals(item))
            .First();
    }
}