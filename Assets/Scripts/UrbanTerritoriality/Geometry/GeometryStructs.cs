using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Geometry
{
    /** A structre representing a 
     * 2 dimensional line.
     * The line is defined by two points in the line.
     */
    [System.Serializable]
    public struct Line2d
    {
        /**
         * One of the two points in the line.
         */
        public Vector2 p1;

        /**
         * The other of the two points in the line.
         */
        public Vector2 p2;

        /**
         * Constructor
         * @param p1 One of the two points in the line.
         * @param p2 One of the two points in the line.
         */
        public Line2d(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }

    /**
     * A structure representing a 3 dimensional line.
     * The line is defined by two points in the line.
     */
    [System.Serializable]
    public struct Line3d
    {
        /**
         * One of the two points defining the line.
         */
        public Vector3 p1;

        /**
         * The other of the two points defining the line.
         */
        public Vector3 p2;

        /**
         * Construtor
         * @param p1 One the two points defining the line.
         * @param p2 The other of the two points that define the line.
         */
        public Line3d(Vector3 p1, Vector3 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }

    /**
     * A structure representing a 2D triangle.
     * The triangle is defined by its three corner points.
     */
    [System.Serializable]
    public struct Triangle2d
    {
        /** One of the three corners of the triangle */
        public Vector2 p1;

        /** One of the three corners of the triangle */
        public Vector2 p2;

        /** One of the three corners of the triangle */
        public Vector2 p3;

        /**
         * Constructor
         * @param p1 One of the three corners.
         * @param p2 One of the three corners.
         * @param p3 One of the three corners.
         */
        public Triangle2d(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }

    /**
     * A structure representing a 3D triangle.
     * The triangle is defined by its three corner points.
     */
    [System.Serializable]
    public struct Triangle3d
    {
        /** One of the three corners of the triangle */
        public Vector3 p1;

        /** One of the three corners of the triangle */
        public Vector3 p2;

        /** One of the three corners of the triangle */
        public Vector3 p3;

        /**
         * Constructor
         * @param p1 One of the three corners.
         * @param p2 One of the three corners.
         * @param p3 One of the three corners.
         */
        public Triangle3d(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }

    /**
     * A structure defining a 2D rectangle
     * The rectangle is defined by two corners of the rectangle
     * each being a 2d vector with a x and y value.
     */
    [System.Serializable]
    public struct Rectangle2d
    {
        /** The corner of the rectangle having the lowest x
         * and y value */
        public Vector2 minCorner;

        /**
         * The corner of the rectangle having the highest x
         * and y value
         */
        public Vector2 maxCorner;

        /**
         * Constructor
         * @param minCorner The corner of the rectangle having the lowest x
         * and y value.
         * @param maxCorner The corner of the rectangle having the highest x
         * and y value.
         */
        public Rectangle2d(Vector2 minCorner, Vector2 maxCorner)
        {
            this.minCorner = minCorner;
            this.maxCorner = maxCorner;
        }
    }

    /**
     * A structure representing a 3 dimensional box
     * The box is represented by two corner points.
     */
    [System.Serializable]
    public struct Box3d
    {
        /**
         * The corner of the box that has the lowest x
         * and y value.
         */
        public Vector3 minCorner;

        /**
         * The corner of the box that has the highest x
         * and y value.
         */
        public Vector3 maxCorner;

        /**
         * Constructor
         * @param minCorner The corner point of the box
         * that has the lowest x and y value.
         * @param maxCorner The corner point of the box
         * that has the highest x and y value.
         */
        public Box3d(Vector3 minCorner, Vector3 maxCorner)
        {
            this.minCorner = minCorner;
            this.maxCorner = maxCorner;
        }
    }
}

