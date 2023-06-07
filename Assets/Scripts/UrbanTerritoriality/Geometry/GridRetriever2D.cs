using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Geometry
{

    /**
     * A class for fast retrieval of objects in a scene.
     * When objects are added to the retriever, a 2d rectangle
     * specifying their bounds along the two vertical axis
     * (x and z in a Unity scene)
     * must be supplied.
     * All added objects within a certain area in a scene can then be retrieved
     * simply be supplying a 2d rectangle defining the area in a scene.
     */
    public class GridRetriever2D<T>
    {
        /** The data managed by the retriever */
        protected Dictionary<Vector2Int, HashSet<T>> data;

        /** The cell size of the grid */
        public float CellSize { get { return cellSize; } }
        protected float cellSize;

        /** Get the cell for where a position in the world
         * belongs in.
         * @param position The position in the world to get
         * the cell for.
         * @return Returns the cell that the position lands in.
         */
        public virtual Vector2Int GetCellForPosition(Vector2 position)
        {
            Vector2 scaled = position / cellSize;
            int x = Mathf.RoundToInt(scaled.x);
            int y = Mathf.RoundToInt(scaled.y);
            Vector2Int cell = new Vector2Int(x, y);
            return cell;
        }

        /** Useful method for calculating the cell size for the retriever
         * given some variables.
         * @param areaSizeX The length of the area where the objects are along the x axis.
         * @param areaSizeZ The length of the area where the objects are along the z axis.
         * @param objectsPerCell How many objects should there be in a cell on average.
         * This value can vary depending on the situation. Typical values might be between 1 and 1000.
         * It is recommended to try a few different values and see what gives the best performance.
         * @param totalNrOfObjects The total number of objects.
         * @return The cell size resulting from the parameters supplied.
         */
        public static float CalculateCellSize(
            float areaSizeX, 
            float areaSizeZ,
            float objectsPerCell,
            float totalNrOfObjects)
        {
            return Mathf.Sqrt(objectsPerCell * areaSizeX * areaSizeZ / totalNrOfObjects);
        }

        /**
         * Construct a new GridRetriever2D object
         * @param cellSize The cell size of the grid the retriever uses
         * for storing the objects.
         */
        public GridRetriever2D(float cellSize)
        {
            this.cellSize = cellSize;
            data = new Dictionary<Vector2Int, HashSet<T>>();
        }

        /**
         * Add a new object into the retriever.
         * @param bounds A 2D rectangle specifying the bounds of
         * the object along the two horizontal axis (x and z
         * for a Unity scene).
         */
        public virtual void Add(T value, Rectangle2d bounds)
        {
            Vector2Int maxCell = GetCellForPosition(bounds.maxCorner);
            Vector2Int minCell = GetCellForPosition(bounds.minCorner);
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector2Int key = new Vector2Int(x, y);
                    if (!data.ContainsKey(key))
                    {
                        data[key] = new HashSet<T>();
                    }
                    data[key].Add(value);
                }
            }
        }

        /**
         * Get all objects withing certain bounds in a scene.
         * @param bounds A 2d rectangle specifying the bounds
         * along the two horizontal axis (x and z for Unity).
         * @return Returns all objects that that have been added
         * to the retriever that are within the bounds.
         */
        public virtual HashSet<T> Retrieve(Rectangle2d bounds)
        {
            HashSet<T> values = new HashSet<T>();

            Vector2Int maxCell = GetCellForPosition(bounds.maxCorner);
            Vector2Int minCell = GetCellForPosition(bounds.minCorner);
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector2Int key = new Vector2Int(x, y);
                    if (data.ContainsKey(key))
                    {
                        foreach (T val in data[key])
                        {
                            values.Add(val);
                        }
                    }
                }
            }
            return values;
        }
    }
}

