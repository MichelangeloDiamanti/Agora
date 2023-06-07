// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UrbanTerritoriality.Utilities;

// namespace UrbanTerritoriality.Maps
// {
//     /** A 2 dimensional array of floats
// 	with methods to paint shapes onto it.
// 	*/
//     public class GPUPaintGrid
//     {

//         /** Method when drawing to the grid
// 		If REPLACE is used then values in the grid will
// 		be replaced with the new values
// 		If ADD is used then the new values will be
// 		added to the grid
// 		*/
//         public enum PaintMethod { ADD, REPLACE }


//         /** Get the number of columns in the grid. */
//         public int Width { get { return grid.width; } }

//         /** Get the number of rows in the grid. */
//         public int Height { get { return grid.height; } }

//         /** The grid array. */
//         public RenderTexture grid;

//         /** Value that is returned outside of the grid. */
//         public float defaultValue = 0;

//         /** Returns true if a point is within the grid. */
//         public bool IsWithinGrid(int x, int y)
//         {
//             return (x >= 0 && y >= 0 &&
//                 x < Width && y < Height);
//         }

//         /** Returns true if a point is within the grid. */
//         public bool IsWithinGrid(Vector2Int point)
//         {
//             return IsWithinGrid(point.x, point.y);
//         }

//         /** Get the PaintGrid as a texture
//          * @param gradient Gradient to convert values from the grid into colors
//          * @return A Texture2D object that is an image of the PaintGrid
//          * using the supplied gradient. If gradient is null then the texture
//          * will be a grayscale texture.
//          */
//         public virtual Texture2D GetAsTexture(Gradient gradient)
//         {
//             throw new System.NotImplementedException();
//             // Texture2D tex = new Texture2D(Width, Height);
//             // Color[] cols = new Color[Width * Height];

//             // int n = Width * Height;
//             // for (int i = 0; i < n; i++)
//             // {
//             //     if (gradient != null)
//             //     {
//             //         cols[i] = gradient.Evaluate(grid[i]);
//             //     }
//             //     else
//             //     {
//             //         cols[i] = new Color(1, 1, 1, 1) * grid[i];
//             //     }
//             // }
//             // tex.SetPixels(cols);
//             // tex.Apply();

//             // return tex;
//         }

//         public float GetMaxValue()
//         {
//             throw new System.NotImplementedException();
//             // float maxValue = float.NegativeInfinity;

//             // for (int x = 0; x < width; x++)
//             // {
//             //     for (int y = 0; y < height; y++)
//             //     {
//             //         float currentValue = grid[y * width + x];
//             //         if (currentValue > maxValue)
//             //         {
//             //             maxValue = currentValue;
//             //         }
//             //     }
//             // }

//             // return maxValue;
//         }


//         /** Get a value from a specific cell
//          * in the grid.
//          * @param x The column index, starting at 0
//          * and ending in width - 1.
//          * @param y The row index, starting at 0
//          * and ending in height - 1.
//          * @returns Returns the value in the cell.
//          * If the cell is outside the grid
//          * then the default value is returned.
//          */
//         public float GetValueAt(int x, int y)
//         {
//             if (IsWithinGrid(x, y))
//             {
//                 return grid[y * width + x];
//             }
//             else
//             {
//                 return defaultValue;
//             }
//         }

//         /**
//          * Return the (x, y) of a index on the grid.      
//          */
//         public Vector2Int GetXY(int index)
//         {
//             Vector2Int p = Vector2Int.zero;
//             p.x = index % width;
//             p.y = (index - p.x) / width;
//             return p;
//         }

//         /**
//          * Set the value of a cell.
//          * @param x The column of the cell.
//          * @param y The row of the cell.
//          * @param value The new value of the cell.
//          */
//         public void SetCell(int x, int y, float value)
//         {
//             if (IsWithinGrid(x, y))
//             {
//                 grid[y * width + x] = value;
//             }
//         }

//         /**
//          * Get the value of a cell
//          * @param x The column of the cell.
//          * @param y The row of the cell.
//          * @return The value at that position in the grid.
//          */
//         public float GetCellValue(int x, int y)
//         {
//             return grid[y * width + x];
//         }

//         /**
//          * Construct a new PaintGrid object.
//          * @param width Number of columns in the grid.
//          * @param height Number of rows in the grid.
//          */
//         public PaintGrid(int width, int height)
//         {
//             this.width = width;
//             this.height = height;
//             int n = width * height;
//             grid = new float[n];
//             for (int i = 0; i < n; i++)
//             {
//                 grid[i] = 0;
//             }
//         }

//         /**
//          * Set all cells to a particular value.
//          * @param value The value that all cells in the grid
//          * are to be set to.
//          */
//         public void Clear(float value)
//         {
//             defaultValue = value;
//             int n = width * height;
//             for (int i = 0; i < n; i++)
//             {
//                 grid[i] = value;
//             }
//         }

//         /**
//          * Draws an territory ellipse in the grid.
//          * @param el The territory ellipse.
//          * @param method The paint method to use
//          */
//         public void DrawTerritoryEllipse(PaintGridTerritoryEllipse el,
//             PaintMethod method)
//         {
//             DrawTerritoryEllipse(
//             el.value,
//             method,
//             el.agentCenterX,
//             el.agentCenterY,
//             el.agentRotation,
//             el.territoryWidth,
//             el.territoryFront,
//             el.territoryBack);
//         }

//         /** Draws a territory ellipse onto the grid
// 		This can e.g. represent a characters personal space
// 		in a 2 dimensional plane.
// 		@param value The value to put in the grid inside this ellipse.
// 		@param method The paint method, see the PaintMethod enum.
// 		@param agentCenterX The x part of the agents position.
// 		@param agentCenterY The y part of the agents position.
// 		@param agentRotation The rotation of the agent in radians.
// 		@param territoryWidth Width of the ellipse.
// 		@territoryFront: Length of the ellipse in front of the agent.
// 		@territoryBack: Length of the ellipse behind the agent.
// 		*/
//         public void DrawTerritoryEllipse(
//         float value,
//         PaintMethod method,
//         float agentCenterX,
//         float agentCenterY,
//         float agentRotation,
//         float territoryWidth,
//         float territoryFront,
//         float territoryBack)
//         {
//             float dist = (territoryBack - territoryFront) / 2;
//             float centerX = agentCenterX + Mathf.Cos(agentRotation - Mathf.PI / 2) * dist;
//             float centerY = agentCenterY + Mathf.Sin(agentRotation - Mathf.PI / 2) * dist;
//             float radiusX = territoryWidth / 2;
//             float radiusY = (territoryFront + territoryBack) / 2;
//             DrawEllipseFast(value, method, centerX, centerY, radiusX, radiusY, agentRotation);
//         }

//         /** Checks if a point is within a territory ellipse
//          * @param pointX The x part of the point.
//          * @param pointY The y part of the point.
//          * @param agentCenterX The x part of the agents center.
//          * @param agentCenterY The y part of the agents center.
//          * @param agentRotation The rotation of the agent in radians.
//          * @param territoryWidth The width of the ellipse.
//          * @param territoryFront The length of the ellipse in front of the agent.
//          * @param territoryBack The length of the ellipse behind the agent.
//          * @return Returns true if the point is within the ellipse, else false.
//          */
//         public static bool IsPointWithinTerritoryEllipse(
//             float pointX,
//             float pointY,
//             float agentCenterX,
//             float agentCenterY,
//             float agentRotation,
//             float territoryWidth,
//             float territoryFront,
//             float territoryBack)
//         {
//             float dist = (territoryBack - territoryFront) / 2;
//             float centerX = agentCenterX + Mathf.Cos(agentRotation - Mathf.PI / 2) * dist;
//             float centerY = agentCenterY + Mathf.Sin(agentRotation - Mathf.PI / 2) * dist;
//             float radiusX = territoryWidth / 2;
//             float radiusY = (territoryFront + territoryBack) / 2;
//             return IsPointWithinEllipse(pointX, pointY, centerX, centerY,
//                 radiusX, radiusY, agentRotation);
//         }

//         /**
//          * Checks if a 2 dimensional point is within a territory ellipse.
//          * @param pointX The x part of the point.
//          * @param pointY The y part of the point.
//          * @param el The territory ellipse.
//          * @return True if the point is within the ellipse.
//          */
//         public static bool IsPointWithinTerritoryEllipse(float pointX,
//             float pointY, PaintGridTerritoryEllipse el)
//         {
//             return IsPointWithinTerritoryEllipse(pointX, pointY,
//                 el.agentCenterX, el.agentCenterY, el.agentRotation,
//                 el.territoryWidth, el.territoryFront, el.territoryBack);
//         }

//         /**
//          * Draws a triangle onto the paint grid
//          * @param value The value inside the triangle
//          * @param method The PaintMethod to use
//          * @param t1 First point of the triangle
//          * @param t2 Second point of the triangle
//          * @param t3 Third point of the triangle
//          * @param expansion This is mainly useful
//          * if the triangle to be drawn is part of a collection
//          * of triangles that form a polygon and no holes should
//          * appear between the triangles. For good results, set
//          * this to some small value such as 0.001. Values larger
//          * than 1 will result in incorrect triangles. If this is
//          * null then this will not be used.
//          */
//         public void DrawTriangle(
//             float value,
//             PaintMethod method,
//             Vector2 t1,
//             Vector2 t2,
//             Vector2 t3,
//             float? expansion)
//         {
//             float[] arrX = new float[] { t1.x, t2.x, t3.x };
//             float[] arrY = new float[] { t1.y, t2.y, t3.y };
//             int maxX = (int)Util.Max(arrX) + 1;
//             int minX = (int)Util.Min(arrX) - 1;
//             int maxY = (int)Util.Max(arrY) + 1;
//             int minY = (int)Util.Min(arrY) - 1;

//             if (maxX >= width) maxX = width - 1;
//             if (minX < 0) minX = 0;
//             if (maxY >= height) maxY = height - 1;
//             if (minY < 0) minY = 0;

//             if (expansion != null)
//             {
//                 float e = (float)expansion;
//                 Vector2 triangleMean = (t1 + t2 + t3) / 3f;
//                 t1 = t1 + (t1 - triangleMean).normalized * e;
//                 t2 = t2 + (t2 - triangleMean).normalized * e;
//                 t3 = t3 + (t3 - triangleMean).normalized * e;
//             }

//             for (int x = minX; x <= maxX; x++)
//             {
//                 for (int y = minY; y <= maxY; y++)
//                 {
//                     if (Geometry.GeometryUtil.IsPointWithinTriangle(new Vector2(x, y),
//                         new Geometry.Triangle2d(t1, t2, t3), true))
//                     {
//                         int index = y * width + x;
//                         if (method == PaintMethod.REPLACE)
//                         {
//                             grid[index] = value;
//                         }
//                         else
//                         {
//                             grid[index] += value;
//                         }
//                     }
//                 }
//             }
//         }

//         /** Draws an ellipse to the grid.
// 		* @param value The value to put in the grid inside the ellipse.
// 		* @param method The paint method, see the PaintMethod enum.
// 		* @param centerX The x part of the center of the ellipse.
// 		* @param centerY The y part of the center of the ellipse.
// 		* @param radiusX The radius of the ellipse along the x axis when it is not rotated.
// 		* @param radiusY The radius of the ellipse along the y axis when it is not rotated.
// 		* @param rotation The rotation of the ellipse in radians.
// 		*/
//         public void DrawEllipse(
//         float value,
//         PaintMethod method,
//         float centerX,
//         float centerY,
//         float radiusX,
//         float radiusY,
//         float rotation)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 for (int x = 0; x < width; x++)
//                 {
//                     float x_h = x - centerX;
//                     float y_k = y - centerY;
//                     float cosA = Mathf.Cos(rotation);
//                     float sinA = Mathf.Sin(rotation);
//                     float a2 = radiusX * radiusX;
//                     float b2 = radiusY * radiusY;

//                     float term1upper = x_h * cosA + y_k * sinA;
//                     term1upper = term1upper * term1upper;
//                     float term1 = term1upper / a2;

//                     float term2upper = x_h * sinA - y_k * cosA;
//                     term2upper = term2upper * term2upper;
//                     float term2 = term2upper / b2;

//                     if (term1 + term2 <= 1)
//                     {
//                         int index = y * width + x;
//                         if (method == PaintMethod.REPLACE)
//                         {
//                             grid[index] = value;
//                         }
//                         else
//                         {
//                             grid[index] += value;
//                         }
//                     }
//                 }
//             }
//         }

//         /** Checks if this PaintGrid is equal to another.
//          * @param other The other PaintGrid
//          * @return True if they are equal, else false.
//          */
//         public bool IsEqual(PaintGrid other)
//         {
//             if (other.Width != width) return false;
//             if (other.Height != height) return false;
//             if (other.defaultValue != defaultValue) return false;
//             int n = width * height;
//             for (int i = 0; i < n; i++)
//             {
//                 if (other.grid[i] != grid[i]) return false;
//             }
//             return true;
//         }

//         /**
//          * Creates a new PaintGrid object that is an
//          * exact copy of this one.
//          * @return The new PaintGrid that was created.
//          */
//         public PaintGrid MakeCopy()
//         {
//             PaintGrid pg = new PaintGrid(width, height);
//             pg.defaultValue = defaultValue;
//             pg.grid = grid.Clone() as float[];
//             return pg;
//         }

//         /**
//          * Checks if a 2d point is within an ellipse.
//          * @param pointX The x part of the point.
//          * @param pointY The y part of the point.
//          * @param ellipseCenterX Center of the ellipse in the x dimension.
//          * @param ellipseCenterY Center of the ellipse in the y dimension.
//          * @param ellipseRadiusX Radius of the ellipse in the x dimension
//          * when it is not rotated.
//          * @param ellipseRadiusY Radius of the ellipse in the y dimension
//          * when it is not rotated.
//          * @param ellipseRotation The rotation of the ellipse in radians.
//          */
//         public static bool IsPointWithinEllipse(
//             float pointX,
//             float pointY,
//             float ellipseCenterX,
//             float ellipseCenterY,
//             float ellipseRadiusX,
//             float ellipseRadiusY,
//             float ellipseRotation)
//         {
//             float cosA = Mathf.Cos(ellipseRotation);
//             float sinA = Mathf.Sin(ellipseRotation);
//             float a2 = ellipseRadiusX * ellipseRadiusX;
//             float b2 = ellipseRadiusY * ellipseRadiusY;
//             float x_h = pointX - ellipseCenterX;
//             float y_k = pointY - ellipseCenterY;

//             float term1upper = x_h * cosA + y_k * sinA;
//             term1upper = term1upper * term1upper;
//             float term1 = term1upper / a2;

//             float term2upper = x_h * sinA - y_k * cosA;
//             term2upper = term2upper * term2upper;
//             float term2 = term2upper / b2;

//             return (term1 + term2 <= 1);
//         }

//         /**
//          * Draw a single pixel in the grid.
//          * @param value The value to put in the pixel.
//          * @param x Column of the pixel.
//          * @param y Row of the pixel.
//          * @param method The paint method to use.
//          */
//         public void DrawPixel(float value, int x, int y, PaintMethod method)
//         {
//             if (IsWithinGrid(x, y))
//             {
//                 int index = y * width + x;
//                 if (method == PaintMethod.REPLACE)
//                 {
//                     grid[index] = value;
//                 }
//                 else
//                 {
//                     grid[index] += value;
//                 }
//             }
//         }

//         /** A much faster version of DrawEllipse that does exactly the same thing
//         * so there is no reason to use DrawEllipse anymore. Still keeping DrawEllipse
//         * to test DrawEllipseFast.
// 		* @param value The value to put in the grid inside the ellipse.
// 		* @param method The paint method, see the PaintMethod enum.
// 		* @param centerX The x part of the center of the ellipse.
// 		* @param centerY The y part of the center of the ellipse.
// 		* @param radiusX The radius of the ellipse along the x axis when it is not rotated.
// 		* @param radiusY The radius of the ellipse along the y axis when it is not rotated.
// 		* @param rotation The rotation of the ellipse in radians.
//         */
//         public void DrawEllipseFast(
//         float value,
//         PaintMethod method,
//         float centerX,
//         float centerY,
//         float radiusX,
//         float radiusY,
//         float rotation)
//         {
//             float maxRadi = Mathf.Max(radiusX, radiusY);
//             int xStart = Util.Clamp((int)(centerX - maxRadi) - 1, 0, width - 1);
//             int xEnd = Util.Clamp((int)(centerX + maxRadi) + 1, 0, width - 1);
//             int yStart = Util.Clamp((int)(centerY - maxRadi) - 1, 0, height - 1);
//             int yEnd = Util.Clamp((int)(centerY + maxRadi) + 1, 0, height - 1);

//             float cosA = Mathf.Cos(rotation);
//             float sinA = Mathf.Sin(rotation);
//             float a2 = radiusX * radiusX;
//             float b2 = radiusY * radiusY;

//             for (int y = yStart; y <= yEnd; y++)
//             {
//                 for (int x = xStart; x <= xEnd; x++)
//                 {
//                     float x_h = x - centerX;
//                     float y_k = y - centerY;

//                     float term1upper = x_h * cosA + y_k * sinA;
//                     term1upper = term1upper * term1upper;
//                     float term1 = term1upper / a2;

//                     float term2upper = x_h * sinA - y_k * cosA;
//                     term2upper = term2upper * term2upper;
//                     float term2 = term2upper / b2;

//                     if (term1 + term2 <= 1)
//                     {
//                         int index = y * width + x;
//                         if (method == PaintMethod.REPLACE)
//                         {
//                             grid[index] = value;
//                         }
//                         else
//                         {
//                             grid[index] += value;
//                         }
//                     }
//                 }
//             }
//         }

//         public void DrawSquare(float value, PaintMethod method,
//             Vector2Int center, int size)
//         {
//             int halfSize = (int)(size / 2);
//             for (int i = center.y - halfSize; i < center.y + halfSize; i++)
//             {
//                 if (i < 0 || i > height) continue;
//                 for (int j = center.x - halfSize; j < center.x + halfSize; j++)
//                 {
//                     if (j < 0 || j > width) continue;

//                     int index = i * width + j;
//                     if (index < 0 || index > grid.Length)
//                         continue;
//                     if (method == PaintMethod.REPLACE)
//                     {
//                         grid[index] = value;
//                     }
//                     else
//                     {
//                         grid[index] += value;
//                     }
//                 }
//             }
//         }
//     }
// }
