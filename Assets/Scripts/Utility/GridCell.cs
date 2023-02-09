using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class GridCell
    {
        private int width;
        private int depth;
        private float cellSize;
        private int[,] gridArray;

        private Vector3 originPosition;



        public GridCell(int width, int depth, float cellSize, Vector3 originPosition)
        {
            this.width = width;
            this.depth = depth;
            this.cellSize = cellSize;
            this.originPosition = originPosition;

            gridArray = new int[width, depth];


            DrawLine();
        }
        private void DrawLine()
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.yellow, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.yellow, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, depth), GetWorldPosition(width, depth), Color.yellow, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, depth), Color.yellow, 100f);
        }

        /// <summary>
        /// 使用Debug.DrawLine只能使用世界坐标
        /// </summary>
        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y, 0) * cellSize + originPosition;
        }

        /// <summary>
        /// GetXY这里的XY指的是二维数组的Index
        /// </summary>
        private void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }
        public int GetX(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return x;
        }
        public int GetY(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return y;
        }

        public void SetValue(int x, int z, int value)
        {
            if (x >= 0 && x < width && z >= 0 && z < depth)
            {
                gridArray[x, z] = value;
            }
        }
        public void SetValue(Vector3 worldPosition, int value)
        {
            int x, z;
            GetXY(worldPosition, out x, out z);
            SetValue(x, z, value);
        }
        public int GetValue(int x, int z)
        {
            if (x >= 0 && x < width && z >= 0 && z < depth)
            {
                return gridArray[x, z];
            }
            return 0;
        }


        public int GetValue(Vector3 worldPosition)
        {
            int x, z;
            GetXY(worldPosition, out x, out z);
            return GetValue(x, z);
        }

        public Vector3 OneCellSnapUpdate2(RaycastHit hit)
        {
            int x, z;
            GetXY(hit.point, out x, out z);
            if (x >= 0 && x < width && z >= 0 && z < depth)
            {
                var vector3 = GetWorldPosition(x, z);
                return vector3 + new Vector3(0.5f, 0, 0.5f) * cellSize;
            }
            return hit.point;
        }
        public Vector3 OneCellSnapUpdate2D(Vector3 position, out bool isInCell)
        {
            isInCell = false;
            int x, z;
            GetXY(position, out x, out z);
            if (x >= 0 && x < width && z >= 0 && z < depth)
            {
                isInCell = true;
                var vector3 = GetWorldPosition(x, z);
                return vector3 + new Vector3(0.5f, 0.5f, 0) * cellSize;
            }
            return position;
        }
        public Vector3 TwoCellSnap2D(Vector3 position, out bool isInCell)
        {
            isInCell= false;
            int x, y;
            GetXY(position, out x, out y);
            if (x >= 0 && x < width && y >= 0 && y < depth)
            {
                isInCell = true;
                if (x >= 0 && x < width - 1)
                {
                    var vector3 = GetWorldPosition(x, y);
                    return vector3 + new Vector3(1f, 0.5f, 0) * cellSize;
                }
                if (x >= width - 1 && x < width)
                {
                    var vector3 = GetWorldPosition(x-1, y);
                    return vector3 + new Vector3(1f, 0.5f, 0) * cellSize;
                }
            }
            return position;
        }
        public Vector3 ThreeCellSnap2D(Vector3 position, out bool isInCell)
        {
            isInCell = false;
            int x, y;
            GetXY(position, out x, out y);
            if (x >= 0 && x < width && y >= 0 && y < depth)
            {
                isInCell = true;
                var vector3 = GetWorldPosition(0, y);
                return vector3 + new Vector3(1.5f,0.5f,0) * cellSize;
            }
            return position;
        }
        public Vector3 FourCellSnap2D(Vector3 position, out bool isInCell)
        {
            isInCell = false;
            int x, y;
            GetXY(position, out x, out y);
            if (x >= 0 && x < width && y >= 0 && y < depth)
            {
                isInCell = true;
                if (x >= 0 && x < width - 1)
                {
                    if (y >= 0 && y < depth - 1)
                    {
                        var vector3 = GetWorldPosition(x, y);
                        return vector3 + new Vector3(1f, 1f, 0) * cellSize;
                    }
                    else if (y >= depth - 1 && y < depth)
                    {
                        var vector3 = GetWorldPosition(x,y- 1);
                        return vector3 + new Vector3(1f, 1f, 0) * cellSize;
                    }
                }
                else if (x >= width - 1 && x < width)
                {
                    if (y >= 0 && y < depth - 1)
                    {
                        var vector3 = GetWorldPosition(x - 1, y);
                        return vector3 + new Vector3(1f, 1f, 0) * cellSize;
                    }
                    else if (y >= depth - 1 && y < depth)
                    {
                        var vector3 = GetWorldPosition(x - 1, y - 1);
                        return vector3 + new Vector3(1f, 1f, 0) * cellSize;
                    }
                }
            }
            return position;
        }
        public Vector3 SixCellSnap2D(Vector3 position, out bool isInCell)
        {
            isInCell = false;
            int x, y;
            GetXY(position, out x, out y);
            if (x >= 0 && x < width && y >= 0 && y < depth)
            {
                isInCell = true;
                if (x >= 0 && x < width - 1)
                {
                    if (y >= 0 && y < depth - 2)
                    {
                        var vector3 = GetWorldPosition(x, y);
                        return vector3 + new Vector3(1f, 1.5f, 0) * cellSize;
                    }
                    else if (y >= depth - 2 && y < depth)
                    {
                        var vector3 = GetWorldPosition(x, 1);
                        return vector3 + new Vector3(1f, 1.5f, 0) * cellSize;
                    }
                }
                else if (x >= width - 1 && x < width)
                {
                    if (y >= 0 && y < depth - 2)
                    {
                        var vector3 = GetWorldPosition(x - 1, y);
                        return vector3 + new Vector3(1f, 1.5f, 0) * cellSize;
                    }
                    else if (y >= depth - 2 && y < depth)
                    {
                        var vector3 = GetWorldPosition(x - 1, 1);
                        return vector3 + new Vector3(1f, 1.5f, 0) * cellSize;
                    }
                }
            }
            return position;
        }
    }
}