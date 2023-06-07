using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.Utilities
{
	/** A static class with various useful methods. */
	public static class Util
	{
		/**
         * Convert an array of Vector2 values to an array of 
         * Vector3 values.
         * @param points The points to convert.
         * @param y The y value in of the vectors in the
         * returned array
         * @return The points converted to Vector3 points.
         */
		//TODO add test case
		public static Vector3[] ConvertToPath3d(Vector2[] points, float y)
		{
			int n = points.Length;
			Vector3[] points3d = new Vector3[n];
			for (int i = 0; i < n; i++)
			{
				points3d[i] = new Vector3(points[i].x, y, points[i].y);
			}
			return points3d;
		}

		/** Calculates the cross product of two 2 dimensional vectors */
		public static float CrossProduct(Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.y - v1.y * v2.x;
		}

		/** Get the intersecting point of two 2 dimensional lines
         * that are both infinitely long.
         * @param p1 First point in line 1
         * @param p2 Second point in line 1
         * @param p3 First point in line 2
         * @param p4 Second point in line 2
         * @return The intersecting point of the two lines if it exists.
         * If the lines do not intersect then null is returned.
         */
		public static Vector2? LineIntersection(Vector2 p1,
			Vector2 p2,
			Vector2 p3,
			Vector2 p4)
		{
			float x1y2_y1x2 = p1.x * p2.y - p1.y * p2.x;
			float x3y4_y3x4 = p3.x * p4.y - p3.y * p4.x;
			float x3_x4 = p3.x - p4.x;
			float x1_x2 = p1.x - p2.x;
			float y3_y4 = p3.y - p4.y;
			float y1_y2 = p1.y - p2.y;
			float divisor = x1_x2 * y3_y4 - y1_y2 * x3_x4;

			/* If this value is equel to 0 then the lines do not intersect */
			if (divisor == 0)
			{
				return null;
			}

			float multiplier = 1f / divisor;
			float x = (x1y2_y1x2 * x3_x4 - x1_x2 * x3y4_y3x4) * multiplier;
			float y = (x1y2_y1x2 * y3_y4 - y1_y2 * x3y4_y3x4) * multiplier;

			return new Vector2(x, y);
		}

		/**
         * Calculate the intersecting point of an infinitely long line
         * and a ray that has a starting point, a direction and is
         * infinitely long in that direction but does not extend
         * behind the starting point.
         * @param rayStart The start of the ray
         * @param rayDirection The direction of the ray
         * @param linePoint1 The first point in the line
         * @param linePoint2 The second point in the line
         * @return The point of intersection if it exists.
         * If there is no intersection then null is returned.
         */
		public static Vector2? RayLineIntersection(Vector2 rayStart,
			Vector2 rayDirection, Vector2 linePoint1, Vector2 linePoint2)
		{
			Vector2 rayPoint2 = rayStart + rayDirection;
			Vector2? inter = LineIntersection(rayStart, rayPoint2,
				linePoint1, linePoint2);
			if (inter == null)
			{
				return null;
			}

			Vector2 inter2 = (Vector2)inter;

			Vector2 rayStartToInter = inter2 - rayStart;
			if (Vector2.Dot(rayStartToInter.normalized, rayDirection.normalized) < 0)
			{
				return null;
			}
			return inter2;
		}

		/** A triangle wave function.
         * @param x Input value.
         * @return The value of the triangle function for the input x.
         */
		public static float TriangleWave(float x)
		{
			float a = x - Mathf.Floor(x);
			if (a < 0.25)
			{
				return a * 4;
			}
			if (a >= 0.25 && a < 0.75)
			{
				return 2 - a * 4;
			}
			return a * 4 - 4;
		}

		/** Calculate sum of some numbers.
         * @param values Numbers to be added.
         * @return Sum of the numbers.
         */
		public static float Sum(float[] values)
		{
			float sum = 0;
			foreach (float x in values)
			{
				sum += x;
			}
			return sum;
		}

		/** Calculates the sum of a 3 dimensional vector.
         * @param v The vector to calculate the sum for.
         * @return Sum of the values in the vector.
         * */
		public static float Sum(Vector3 v)
		{
			return v.x + v.y + v.z;
		}

		/** Calculate the mean of some values. 
         * @param values The values to calculate the mean of.
         * @return The mean of the values.
         */
		public static float Mean(float[] values)
		{
			return Sum(values) / (float)values.Length;
		}

		/** Calculate the mean of a 3 dimensional vector.
         * @param v The vector to calculate the mean of.
         * @return The mean of the three values in the vector.
         */
		public static float Mean(Vector3 v)
		{
			return Sum(v) / 3;
		}

		/** Find maximum value in array. 
         * @param values An array of numbers.
         * @return The largest number in the 
         * input array.
         */
		public static float Max(float[] values)
		{
			if (values.Length == 0) return 0;
			float max = values[0];
			foreach (float val in values) if (val > max) max = val;
			return max;
		}

		public static float Min(float[] values)
		{
			if (values.Length == 0) return 0;
			float min = values[0];
			foreach (float val in values) if (val < min) min = val;
			return min;
		}

		/** Clamp value between min and max and return value.
         * @param x An integer.
         * @param min A minumum value.
         * @param max A maximum value.
         * @returns min if x is less than min. max if x is
         * greater than max. Otherwise x is returned.
         * */
		public static int Clamp(int x, int min, int max)
		{
			if (x < min) return min;
			if (x > max) return max;
			return x;
		}

		/** Increment a value x but limit the
         * size of it to maxValue.
         * @param x The number to increment.
         * @param maxValue The maximum number.
         * @return x + 1 as long as x is smaller than
         * maxValue, else x is returned.
         */
		public static int LimitedIncrement(int x, int maxValue)
		{
			if (x < maxValue)
			{
				x++;
			}
			return x;
		}

		/** Decrement a value x but limit the value to minValue.
         * @param x The value to decrement.
         * @param minValue The minium value.
         * @return x - 1 if x is larger than minValue, else
         * x is returned.
         */
		public static int LimitedDecrement(int x, int minValue)
		{
			if (x > minValue)
			{
				x--;
			}
			return x;
		}

		/** Convert degrees to radians.
         * @param degree A degree value to be converted.
         * @return The input value expressed in radians.
         */
		public static float DegToRadian(float degree)
		{
			return degree * Mathf.PI / 180;
		}

		/**
         * Normalizes a map, copies the values into another
         * and returns the meanchange
         */
		public static float Normalize(PaintGrid workGrid, PaintGrid mPaintGrid)
		{
			int n = workGrid.grid.Length;
			float max = float.MinValue;
			for (int i = 0; i < n; i++)
			{
				float val = workGrid.grid[i];
				if (val > max) max = val;
			}

			float changeSum = 0;
			for (int i = 0; i < n; i++)
			{
				float newVal = (workGrid.grid[i] / max);
				changeSum += Mathf.Abs(newVal - mPaintGrid.grid[i]);
				mPaintGrid.grid[i] = newVal;
			}
			return changeSum / (float)n;
		}

		/**
         * Normalizes a map, copies the values into another
         * and returns the meanchange
         */
		public static float RangeNormalization(
			PaintGrid workGrid, PaintGrid mPaintGrid, float min, float max)
		{
			int n = workGrid.grid.Length;

			float changeSum = 0;
			for (int i = 0; i < n; i++)
			{
				float newVal = (workGrid.grid[i] - min) / (max - min);
				
				changeSum += Mathf.Abs(newVal - mPaintGrid.grid[i]);
				mPaintGrid.grid[i] = newVal;
			}

			return changeSum / (float)n;
		}

		/**
         * Normalizes a map, copies the values into another
         * and returns the meanchange
         */
		public static float MinMaxNormalization(
			PaintGrid workGrid, PaintGrid mPaintGrid)
		{
			int n = workGrid.grid.Length;

			float max = float.MinValue;
			float min = float.MaxValue;


			for (int i = 0; i < n; i++)
			{
				float val = workGrid.grid[i];
				if (val > max) max = val;
				if (val < min) min = val;
			}

			float maxNewValue = float.MinValue;

			float changeSum = 0;
			for (int i = 0; i < n; i++)
			{
				float newVal = (workGrid.grid[i] - min) / (max - min);

				if (newVal > maxNewValue) maxNewValue = newVal;

				changeSum += Mathf.Abs(newVal - mPaintGrid.grid[i]);
				mPaintGrid.grid[i] = newVal;
			}

			return changeSum / (float)n;
		}

		/**
	 	* Normalizes a map, copies the values into another
	 	* and returns the meanchange
	 	*/
		public static float ZValueNormalization(
			PaintGrid workGrid, PaintGrid mPaintGrid)
		{
			int n = workGrid.grid.Length;

			float mean = 0;
			float standardDeviation = 0;

			int nonZeroValues = 1;

			for (int i = 0; i < n; i++)
			{
				if (workGrid.grid[i] > 0)
				{
					nonZeroValues++;
					mean += workGrid.grid[i];
				}
			}

			mean = mean / nonZeroValues;

			for (int i = 0; i < n; i++)
			{
				if (workGrid.grid[i] > 0)
				{
					standardDeviation += Mathf.Pow((workGrid.grid[i] - mean), 2);
				}
			}

			standardDeviation = standardDeviation / nonZeroValues;
			standardDeviation = Mathf.Sqrt(standardDeviation);

			for (int i = 0; i < n; i++)
			{
				if (workGrid.grid[i] != 0)
				{
					float newVal = (workGrid.grid[i] - mean) / standardDeviation;
					mPaintGrid.grid[i] = LogisticCurve(newVal);
					// if (newVal <= 0)
					// 	mPaintGrid.grid[i] = 0;
					// else if (newVal >= 1)
					// 	mPaintGrid.grid[i] = 1;
					// else
					// 	mPaintGrid.grid[i] = newVal;
				}
			}

			return standardDeviation;
		}

		public static float LogisticCurve(float value)
		{
			float slope = 2;
			value = value * slope;
			value = Mathf.Exp(value) + 1;
			value = 1 - (1 / value);
			return value;
		}

		public static int GradientHashCode(Gradient gradient)
		{
			int res = 0;
			res += gradient.mode.GetHashCode();
			foreach (GradientAlphaKey ak in gradient.alphaKeys)
				res += ak.GetHashCode();
			foreach (GradientColorKey ck in gradient.colorKeys)
				res += ck.GetHashCode();
			return res;
		}
	}
}
