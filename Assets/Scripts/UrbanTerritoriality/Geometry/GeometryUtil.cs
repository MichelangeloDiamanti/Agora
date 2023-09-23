using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Utilities;

namespace UrbanTerritoriality.Geometry
{
    /** An axis in 3D space */
    public enum Axis
    {
        X, Y, Z
    }
    
    /** Type of intersection of a line and a plane in 3D */
    public enum LinePlaneIntersectionType3D
    {
        /** There is an intersection in a single point on the line. */
        POINT_INTERSECTION,

        /** The line is parallel to the plane and is inside it.
         * All points in the line are also on the plane. */
        PARALLEL_INTERSECTION,

        /** The line is parallel to the plane but is outside the plane.
         * There is no point of intersection */
        NO_INTERSECTION
    }

    /** Results from a line plane intersection test */
    public struct LinePlaneIntersection3D
    {
        /** The type of the intersection, that is was
         * there an intersection in a single point,
         * does the line lie inside the plane or
         * is there no intersection */
        public LinePlaneIntersectionType3D type;

        /** The point of intersection if there is
         * an intersection in a single point.
         * If there is not a single point of intersection
         * then this should be set to null. That happens
         * if the line is parallel to the plane.
         */
        public Vector3? point;

        /** Constructor
         * @param type The type of the intersection
         * @param point The point of the intersection
         */
        public LinePlaneIntersection3D(
            LinePlaneIntersectionType3D type,
            Vector3? point)
        {
            this.type = type;
            this.point = point;
        }
    }

    /** A static class with some functions
     * that perform geometry computations */
    public static class GeometryUtil
    {
        /** Calculate the point of intersection of two 2 dimensional line
         * segments. If the line segments do not intersect null is returned.
         * @param line1 One of the two lines
         * @param line2 One of the two lines
         * @return The point of intersection if the line segments intersect.
         * If they do not intersect then null is returned.
         */
        public static Vector2? LineSegment2DIntersect(Line2d line1, Line2d line2)
        {
            Vector2? inter = Util.LineIntersection(line1.p1, line1.p2, line2.p1, line2.p2);
            if (inter == null)
            {
                return null;
            }
            Vector2 point = (Vector2)inter;

            Vector2 line1Vec1 = (line1.p2 - line1.p1).normalized;
            Vector2 line1Inter1 = (point - line1.p1).normalized;
            if (Vector2.Dot(line1Vec1, line1Inter1) < 0) return null;

            Vector2 line1Vec2 = (line1.p1 - line1.p2).normalized;
            Vector2 line1Inter2 = (point - line1.p2).normalized;
            if (Vector2.Dot(line1Vec2, line1Inter2) < 0) return null;

            Vector2 line2Vec1 = (line2.p2 - line2.p1).normalized;
            Vector2 line2Inter1 = (point - line2.p1).normalized;
            if (Vector2.Dot(line2Vec1, line2Inter1) < 0) return null;

            Vector2 line2Vec2 = (line2.p1 - line2.p2).normalized;
            Vector2 line2Inter2 = (point - line2.p2).normalized;
            if (Vector2.Dot(line2Vec2, line2Inter2) < 0) return null;

            return point;
        }

        /**
         * Check if a 2d point is within a 2d triangle
         * @param p The point to check
         * @param triangle The triangel to check if the point is within
         * @param includeEdges If this is set to true the function 
         * will return true if the point is exactly on an edge of the triangle.
         * If it is false, the function will return true only if the point
         * is strictly within the triangle.
         * @return Returns true if the point is within the triangle,
         * else false.
         */
        public static bool IsPointWithinTriangle(Vector2 p, Triangle2d triangle, bool includeEdges)
        {
            float y2_y3 = triangle.p2.y - triangle.p3.y;
            float x3_x2 = triangle.p3.x - triangle.p2.x;
            float x1_x3 = triangle.p1.x - triangle.p3.x;
            float y1_y3 = triangle.p1.y - triangle.p3.y;
            float x_x3 = p.x - triangle.p3.x;
            float y_y3 = p.y - triangle.p3.y;
            float y3_y1 = triangle.p3.y - triangle.p1.y;
            float det = y2_y3 * x1_x3 + x3_x2 * y1_y3;
            float bar1 = (y2_y3 * x_x3 + x3_x2 * y_y3) / det;
            float bar2 = (y3_y1 * x_x3 + x1_x3 * y_y3) / det;
            float bar3 = 1 - bar1 - bar2;

            return includeEdges ? (bar1 >= 0 && bar1 <= 1 &&
                bar2 >= 0 && bar2 <= 1 &&
                bar3 >= 0 && bar3 <= 1) :
                (bar1 > 0 && bar1 < 1 &&
                bar2 > 0 && bar2 < 1 &&
                bar3 > 0 && bar3 < 1);
        }

        /**
         * Find the intersection of two 2d triangles
         * @param triangle1 One of the two triangles
         * @param triangle2 One of the two triangles
         * @return Returns an array of 2d points that
         * are the corners of the polygon that is formed
         * by the intersection of the two triangles. The points
         * are not necessarily in the correct order and some
         * points may be repeated.
         */
        public static Vector2[]
            GetTriangleIntersection2D(
            Triangle2d triangle1, 
            Triangle2d triangle2)
        {
            Vector2[] points = new Vector2[15];
            int pointCount = 0;
            Line2d[] lines1 = new Line2d[]
            {
                new Line2d(triangle1.p1, triangle1.p2),
                new Line2d(triangle1.p2, triangle1.p3),
                new Line2d(triangle1.p3, triangle1.p1)
            };

            Line2d[] lines2 = new Line2d[]
            {
                new Line2d(triangle2.p1, triangle2.p2),
                new Line2d(triangle2.p2, triangle2.p3),
                new Line2d(triangle2.p3, triangle2.p1)
            };

            int n = 3;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Vector2? inter = LineSegment2DIntersect(lines1[i], lines2[j]);
                    if (inter != null)
                    {
                        points[pointCount] = (Vector2)inter;
                        pointCount++;
                    }
                }
            }

            Vector2[] points1 = new Vector2[]
            {
                triangle1.p1,
                triangle1.p2,
                triangle1.p3
            };
            Vector2[] points2 = new Vector2[]
            {
                triangle2.p1,
                triangle2.p2,
                triangle2.p3
            };
            for (int i = 0; i < n; i++)
            {
                if (IsPointWithinTriangle(points1[i], triangle2, true))
                {
                    points[pointCount] = points1[i];
                    pointCount++;
                }
                if (IsPointWithinTriangle(points2[i], triangle1, true))
                {
                    points[pointCount] = points2[i];
                    pointCount++;
                }
            }

            Vector2[] result = new Vector2[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                result[i] = points[i];
            }

            return result;
        }

        /**
         * Get the intersecting point of a line and a plane
         * if there is one.
         * @param line The line
         * @param plane A triangle representing the plane
         * @return The results contain a LinePlaneIntersectionType3D enum
         * that tell the type of intersection. There are 3 types
         * and one is if there is an intersection in a point,
         * another one is if the line is contained in the plane
         * and another one is if the line is parallel to the plane
         * but does not touch it. If there is an intersection
         * in a single point then the results will contain that point
         */
        public static LinePlaneIntersection3D GetLinePlaneIntersection3D(
            Line3d line, Triangle3d plane)
        {
            Vector3 planeVec1 = plane.p2 - plane.p1;
            Vector3 planeVec2 = plane.p3 - plane.p1;
            Vector3 planeNormal = Vector3.Cross(planeVec1, planeVec2);
            Vector3 lineVec = (line.p2 - line.p1);

            float numerator = Vector3.Dot(plane.p1 - line.p1, planeNormal);
            float denominator = Vector3.Dot(lineVec, planeNormal);

            LinePlaneIntersection3D result =
                new LinePlaneIntersection3D(
                    LinePlaneIntersectionType3D.NO_INTERSECTION,
                    null);

            if (denominator == 0)
            {
                result.type = numerator == 0 ?
                    LinePlaneIntersectionType3D.PARALLEL_INTERSECTION :
                    LinePlaneIntersectionType3D.NO_INTERSECTION;
            }
            else
            {
                result.type = LinePlaneIntersectionType3D.POINT_INTERSECTION;
                float d = numerator / denominator;
                result.point = d * lineVec + line.p1;
            }

            return result;
        }

        /**
         * Check if triangle1 is above triangle2 at every point
         * where the two triangles overlap as seen from above.
         * @param triangle1 The triangle to check if it is above the other
         * @param triangle2 The triangle to check if the other is above it
         * @return True if all points in triangle1 are above
         * all points in triangle2 where the two triangles overlap
         * as seen through the vertical axis.
         */
        public static bool IsTriangleAboveAtEveryVerticalIntersection(
            Triangle3d triangle1,
            Triangle3d triangle2)
        {
            /* Create 2d versions of both */
            Triangle2d t1_2d = new Triangle2d(
                new Vector2(triangle1.p1.x, triangle1.p1.z),
                new Vector2(triangle1.p2.x, triangle1.p2.z),
                new Vector2(triangle1.p3.x, triangle1.p3.z));
            Triangle2d t2_2d = new Triangle2d(
                new Vector2(triangle2.p1.x, triangle2.p1.z),
                new Vector2(triangle2.p2.x, triangle2.p2.z),
                new Vector2(triangle2.p3.x, triangle2.p3.z));

            /* Use GetTriangleIntersection2D to find important points */
            Vector2[] points = GetTriangleIntersection2D(t1_2d, t2_2d);


            /* Create vertical lines containing these points */
            int n = points.Length;
            for (int i = 0; i < n; i++)
            {
                Line3d vertical = new Line3d(
                    new Vector3(points[i].x, 0, points[i].y),
                    new Vector3(points[i].x, 1, points[i].y));

                /* Find points of intersection of these lines with both triangles */
                LinePlaneIntersection3D inter1 = GetLinePlaneIntersection3D(vertical,
                    triangle1);
                LinePlaneIntersection3D inter2 = GetLinePlaneIntersection3D(vertical,
                    triangle2);
                if (inter1.type == LinePlaneIntersectionType3D.POINT_INTERSECTION
                    && inter2.type == LinePlaneIntersectionType3D.POINT_INTERSECTION)
                {
                    Vector3 inter1point = (Vector3)inter1.point;
                    Vector3 inter2point = (Vector3)inter2.point;
                    if (inter2point.y >= inter1point.y) return false;
                }
            }

            return true;
        }

        /**
         * Checks if two triangles have any corners that are the same.
         * @param triangle1 One of the two triangles.
         * @param triangle2 One of the two triangles.
         * @return Returns true if the two triangles have any corner that
         * is the same, else false.
         */
        public static bool AreSharingVertices(Triangle3d triangle1, Triangle3d triangle2)
        {
            Vector3[] tri1 = new Vector3[] {
                triangle1.p1,
                triangle1.p2,
                triangle1.p3
            };
            Vector3[] tri2 = new Vector3[]
            {
                triangle2.p1,
                triangle2.p2,
                triangle2.p3
            };
            int three = 3;
            for (int i = 0; i < three; i++)
            {
                for (int j = 0; j < three; j++)
                {
                    if (tri1[i] == tri2[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Get the bounding box of a triangle in 3D space.
         * @param triangle The triangle.
         * @return The bounding box of the triangle.
         */
        public static Box3d GetBoundingBox(Triangle3d triangle)
        {
            float fmax = float.MaxValue;
            float fmin = float.MinValue;
            Box3d bounds = new Box3d(
                new Vector3(fmax, fmax, fmax),
                new Vector3(fmin, fmin, fmin));

            Vector3[] corners = new Vector3[] { triangle.p1, triangle.p2, triangle.p3 };
            int n = corners.Length;
            for (int i = 0; i < n; i++)
            {
                if (corners[i].x > bounds.maxCorner.x) bounds.maxCorner.x = corners[i].x;
                if (corners[i].x < bounds.minCorner.x) bounds.minCorner.x = corners[i].x;
                if (corners[i].y > bounds.maxCorner.y) bounds.maxCorner.y = corners[i].y;
                if (corners[i].y < bounds.minCorner.y) bounds.minCorner.y = corners[i].y;
                if (corners[i].z > bounds.maxCorner.z) bounds.maxCorner.z = corners[i].z;
                if (corners[i].z < bounds.minCorner.z) bounds.minCorner.z = corners[i].z;
            }
            return bounds;
        }

        /**
         * Get the bounding 2D rectangle in the horizontal plane of a 3D triangle.
         * @param triangle The triangle.
         * @return The bounding rectangle of the triangle in the 
         * horizontal plane (perpendicular to the y axis).
         */
        public static Rectangle2d GetBoundingRectangle(Triangle3d triangle)
        {
            float fmax = float.MaxValue;
            float fmin = float.MinValue;
            Rectangle2d bounds = new Rectangle2d(
                new Vector2(fmax, fmax),
                new Vector2(fmin, fmin));

            Vector3[] corners = new Vector3[] { triangle.p1, triangle.p2, triangle.p3 };
            int n = corners.Length;
            for (int i = 0; i < n; i++)
            {
                if (corners[i].x > bounds.maxCorner.x) bounds.maxCorner.x = corners[i].x;
                if (corners[i].x < bounds.minCorner.x) bounds.minCorner.x = corners[i].x;
                if (corners[i].z > bounds.maxCorner.y) bounds.maxCorner.y = corners[i].z;
                if (corners[i].z < bounds.minCorner.y) bounds.minCorner.y = corners[i].z;
            }
            return bounds;
        }

        /**
         * Convert a 3D vector to a 2D vector
         * @param v The vector to convert
         * @param axisToDiscard The axis of the 3D vector
         * that will not be used in the new 2D vector.
         * @return A 2D vector where the x part and y part
         * are equal to some two parts of the supplied 3D vector.
         */
        public static Vector2 ConvertToVector2(Vector3 v, Axis axisToDiscard)
        {
            if (axisToDiscard == Axis.X)
            {
                return new Vector2(v.y, v.z);
            }
            else if (axisToDiscard == Axis.Y)
            {
                return new Vector2(v.x, v.z);
            }
            return new Vector2(v.x, v.y);
        }

        /**
         * Convert a 2D vector to a 3D vector.
         * @param v The vector to convert.
         * @param newAxis The axis of the 3D vector that
         * should not have a copied value from the 2D vector.
         * @param newVal The value of the part of the 3D vector
         * that was not copied from the supplied 2D vector.
         * @return A 3D vector that has two values copied from
         * the input 2D vector, but one value with the supplied
         * new value.
         */
        public static Vector3 ConvertToVector3(Vector2 v, Axis newAxis, float newVal)
        {
            if (newAxis == Axis.X)
            {
                return new Vector3(newVal, v.x, v.y);
            }
            else if (newAxis == Axis.Y)
            {
                return new Vector3(v.x, newVal, v.y);
            }
            return new Vector3(v.x, v.y, newVal);
        }
    }
}

