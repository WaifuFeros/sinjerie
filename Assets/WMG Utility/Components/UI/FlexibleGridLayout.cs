using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WMG.Utilities
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        public FitType fitType;
        public int rows;
        public int columns;
        public Vector2 cellSize = new Vector2(100f, 100f);
        public Vector2 spacing;
        public bool fitX;
        public bool fitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            int childCount = GetChildCount();

            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
            {
                //fitX = true;
                //fitY = true;

                float sqrRt = Mathf.Sqrt(childCount);
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
            }
            
            if (fitType == FitType.Width || fitType == FitType.FixedColumns)
            {
                rows = Mathf.CeilToInt(childCount / (float)columns);
            }
            if (fitType == FitType.Height || fitType == FitType.FixedRows)
            {
                columns = Mathf.CeilToInt(childCount / (float)rows);
            }

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            //float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)(columns - 2)) * 2) - (padding.left / (float)columns) - (padding.right / (float)columns);
            float cellWidth = (parentWidth - padding.left - padding.right - (spacing.x * (columns - 1))) / columns;
            //float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)(rows - 2)) * 2) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
            float cellHeight = (parentHeight - padding.top - padding.bottom - (spacing.y * (rows - 1))) / rows;

            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;

            int columnCount = 0;
            int rowCount = 0;

            var children = GetChildren();

            for (int i = 0; i < children.Count; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                var item = children[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            
        }

        public override void SetLayoutHorizontal()
        {
            
        }

        public override void SetLayoutVertical()
        {
            
        }

        private int GetChildCount()
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    count++;
            }
            return count;
        }

        private List<RectTransform> GetChildren()
        {
            List<RectTransform> children = new List<RectTransform>();

            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf && child.TryGetComponent<RectTransform>(out RectTransform rect))
                    children.Add(rect);
            }
            return children;
        }
    }
}
