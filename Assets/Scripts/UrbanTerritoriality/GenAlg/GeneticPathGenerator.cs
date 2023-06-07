using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;
using UrbanTerritoriality.Utilities;
using UrbanTerritoriality.Maps;

namespace UrbanTerritoriality.GenAlg
{
    /**
     * Class used for generating paths from a start
     * location to an end location.
     * Evolutionary computation is used here to generate
     * a path with as low cost as possible. The process
     * is as follows:
     * First initialize a population of some random paths.
     * 
     * Then repeat the following over and over:
     * Create child paths that are copies of some
     * paths in the current population and mutate them.
     * Calculate the costs of all the paths
     * Sort the paths based on the cost
     * Remove a certain amount of paths with the highest cost.
     */ 
	public class GeneticPathGenerator {

        /** Some weights used for calculating the cost of a path */

        /** Weight for the ColliderMap in
         * cost function */
        public float colliderWeight = 3f;

        /** Weight for the TerritorialHeatmap
         * the cost function. */
        public float heatmapWeight = 1f;

        /** Weight for maximum line ratio
         * in the cost function.
         * Maximum line ratio here, means the ratio
         * between the longest and shortest
         * line in a path.
         * */
        public float maxLineRatioWeight = 0.01f;

        /** Weight for the maximum angle in
         * the cost function.
         * Maximum angle here, means the maximum
         * angle between two adjacent lines in
         * a path.
         */
        public float maxAngleWeight = 0.01f;

        /** Weight for agent angle in cost function.
         * Here agent angle means the angle between
         * the first line in a path and the forward
         * vector of the agent traveling along the path.
         */
        public float agentAngleWeight = 0.02f;

        /** Starting point of the path */
        public Vector2 start;

		/** The end point of the path */
		public Vector2 end;

		/** A population of paths to evolve */
		public List<PathDNA> population;

        /** Number of paths when the population is shrinked. */
		public int minPopulation;

        /** Number of extra paths created when the population is increased */
		public int extraPopulation;

        /** Heatmap used to calculate the cost of a path */
        public TerritorialHeatmap heatmap;

        /** A collider map used when calculating the cost of a path */
        public ColliderMap colliderMap;

        /** A component used for drawing lines in the scene. */
        public MultilineDrawer lineDrawer;

        /** Heatmap zones that belong to this agent
          * if the character is using one or more.
          */
        public HeatSpace[] ownHeatSpaces = null;

        /** The path following agent that is using this generator */
        public PathFollowingAgent pathFollowingAgent;

        /** The character game object */
        private GameObject agentGameObject;

        /** Construct a new GeneticPathGenerator
         * @param heatmap The heatmap to use
         * @param agentGameObject The GameObject of the characer using 
         * the this GeneticPathGenerator
         * @param minPop The minimum population. This is the number of
         * paths after the population has been shrinked.
         * @param extraPop The extra population. This is the number of
         * paths that are added when the population is increased.
         */
		public GeneticPathGenerator(
            TerritorialHeatmap heatmap,
            GameObject agentGameObject,
            int minPop, int extraPop)
		{
            this.agentGameObject = agentGameObject;
            this.heatmap = heatmap;
			minPopulation = minPop;
			extraPopulation = extraPop;
			start = new Vector2(0, 0);
			end = new Vector2(0, 0);
		}

        /** Initialize the population */
		public void InitPopulation()
		{
			population = new List<PathDNA>();
			for (int i = 0; i < minPopulation; i++)
			{
                PathDNA p = new PathDNA();
                p.cost = CalculateCost(p);
                population.Add(p);
			}
		}

        /**
         * Draw a path in the unity scene
         * in the current frame in edit mode.
         * @param path The path to draw.
         * @param color The color of the drawn path.
         */
        void DrawPath(PathDNA path, Color color)
        {
            Vector2[] points = GetTotalPath(path);
            Vector3[] points3d = ConvertToPath3d(points);
            int n = points3d.Length;
            for (int i = 0; i < n - 1; i++)
            {
                Debug.DrawLine(points3d[i], points3d[i + 1], color);
            }
        }

        /**
         * Pass one generation in the evolutionary computation.
         * This involves the following.
         * Children are created and mutated.
         * Costs of all the paths are calculated.
         * The paths are drawn.
         * The population is sorted.
         * The population is shrinked.
         * The best path (The one with the lowest cost)
         * is drawn in a unique color.
         */
        public void PassOneGeneration()
        {
            /* Create children */
            for (int i = 0; i < extraPopulation; i++)
            {
                PathDNA child;
                if (i == 0)
                {
                    /* Let the first child always be a a straight path from
                     * start to end */
                    child = new PathDNA();
                }
                else
                {
                    int parentIndex = Random.Range((int)0, (int)minPopulation);
                    child = population[parentIndex].MakeCopy();
                    Mutate(ref child);
                }
                population.Add(child);
            }

            /* Update the costs of all the paths */
            int n = population.Count;
            for (int i = 0; i < n; i++)
            {
                population[i].cost = CalculateCost(population[i]);
            }

            /* Draw the generated paths */
            for (int i = 0; i < n; i++)
            {
                float modCost = population[i].cost * 0.3f;
                DrawPath(population[i], new Color(modCost, 1 - modCost, 0, 0.2f));
            }

            /* Sort the population */
            population.Sort();
            
            /* Shrink the population */
            population.RemoveRange(minPopulation, extraPopulation);

            /* Draw the best path in a unique color */
            //DrawPath(GetBestPath(), new Color(0f, 0f, 1f, 1f));
        }

        /**
         * Convert a list of Vector2 values to a list of 
         * Vector3 values where the y values are the equal
         * to the y value in the position of the collider map.
         * @param points The points to convert.
         * @return The points converted to Vector3 points.
         */
         //TODO maybee use position of character instead of colliderMap
        public Vector3[] ConvertToPath3d(Vector2[] points)
        {
            int n = points.Length;
            Vector3[] points3d = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                points3d[i] = new Vector3(points[i].x, colliderMap.transform.position.y, points[i].y);
            }
            return points3d;
        }

        /**
         * Calculate the average cost of all the
         * paths in the population.
         * @return The average cost of the paths in
         * the population.
         */
        public float avgCost()
        {
            int n = population.Count;
            float costSum = 0;
            for (int i = 0; i < n; i++)
            {
                costSum += population[i].cost;
            }
            return costSum / n;
        }

        /** Get the path with the least cost.
         * @return The path with the lowest cost.
         */
        public PathDNA GetBestPath()
        {
            return population[0];
        }

        /** Calculate the maximum angle (in degrees) between
         * two adjacent lines in a path
         * @param points All the points in a path. Including the
         * start and end points.
         * @return The angle of between the two adgacent lines
         * in the path that have the greatest angle between them.
         */
        public float MaxAngleInPath(Vector2[] points)
        {
            int n = points.Length;
            float max = 0;
            for (int i = 0; i < n - 2; i++)
            {
                Vector2 v1 = points[i + 1] - points[i];
                Vector2 v2 = points[i + 2] - points[i + 1];
                float angle = Vector2.Angle(v1, v2);
                if (i == 0) max = angle;
                if (angle > max) max = angle;
            }
            return max;
        }

        /** Get all the points in a path including the start and end point
         * @param path The path to get the points in. The path should not
         * contain the start and end points. Just the inbetween points.
         * @return A list of all the points in the path.
         */
        public Vector2[] GetTotalPath(PathDNA path)
        {
            int n = path.inbetweenPoints.Count + 2;
            Vector2[] points = new Vector2[n];
            points[0] = start;
            points[n - 1] = end;
            for (int i = 1; i < n - 1; i++)
            {
                points[i] = path.inbetweenPoints[i - 1];
            }
            return points;
        }

        /** Calculate the ratio between the length of the
         * longest and shortest line
         * in a path.
         * @param path The points in the path.
         * @return A floating point number that is the
         * length of the longest line divided by the length
         * of the shortest line.
         */
        public static float MaxLineRatio(Vector2[] path)
        {
            float maxLength = 1;
            float minLength = 1;
            int n = path.Length;
            for (int i = 0; i < n - 1; i++)
            {
                float length = Vector2.Distance(path[i], path[i + 1]);
                if (i == 0)
                {
                    maxLength = length;
                    minLength = length;
                }
                if (length > maxLength) maxLength = length;
                if (length < minLength) minLength = length;
            }
            return maxLength / (minLength + 0.0000001f);
        }

        /* Calculates the cost of a path.
         * @param path The path to calculate the cost of.
         * @return A number that is the cost of the path.
         */
        public float CalculateCost(PathDNA path)
        {
            Vector2[] totalPath = GetTotalPath(path);
            int n = totalPath.Length;
            float cost = 0;
            if (n > 50)
            {
                cost = float.MaxValue;
            }
            for (int i = 0; i < n - 1; i++)
            {
                cost += heatmapWeight * heatmap.LineCost(
                    totalPath[i], totalPath[i + 1], pathFollowingAgent);
                cost += colliderWeight * colliderMap.LineCost(
                    totalPath[i], totalPath[i + 1], pathFollowingAgent);
            }
            float maxLineRatio = MaxLineRatio(totalPath);
            float maxAngle = MaxAngleInPath(totalPath);
            
            /* Calculate angle between agent forward vector and first line */
            Vector3 agentForward = agentGameObject.transform.forward;
            Vector2 agentForward2d =
                new Vector2(agentForward.x, agentForward.z);
            Vector2 firstLine = totalPath[1] - totalPath[0];
            float agentAngle = Vector2.Angle(agentForward2d, firstLine);

            cost = cost +
                maxLineRatioWeight * maxLineRatio +
                maxAngleWeight * maxAngle +
                agentAngleWeight * agentAngle;

            return cost;
        }

        /** Get a random number of points to add
         * or remove from a path
         * @return Returns a number that can be
         * the number of points to remove or
         * add to a path.
         */
        private int RandomPointCount()
        {
            int n = 0;
            if (Random.Range(0f, 1f) < 0.5f)
            {
                n = 1;
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    float x = Random.Range(0f, 2f);
                    n = (int)Mathf.Floor(x * x);
                }
            }
            return n;
        }

        /** Mutates a path.
         * Operations that are possible are adding and removing
         * points from the path and modifying the positions
         * of points in the path.
         * @param A reference to a PathDNA object. This
         * object will be modified by the method.
         * */
        public void Mutate(ref PathDNA path)
        {
            int n = RandomPointCount();
            for (int i = 0; i < n; i++)
            {
                AddRandomPoint(ref path);
            }
            n = RandomPointCount();
            for (int i = 0; i < n; i++)
            {
                RemoveRandomPoint(ref path);
            }
            MutatePoints(ref path);
        }

        /** Add a random inbeteen point midway between two
         * random adjacent points in the path.
         * @param path The path to add points in. This
         * object will be modified by the method.
         * @return The index of the line where the point was added
         * starting at 0 and ending at path.inbetweenPoints.Count
         */
        public int AddRandomPoint(ref PathDNA path)
        {
            int lineNr = Random.Range((int)0, (int)(path.inbetweenPoints.Count + 1));
            Vector2 p1 = lineNr == 0 ? start : path.inbetweenPoints[lineNr - 1];
            Vector2 p2 = lineNr == path.inbetweenPoints.Count ? end : path.inbetweenPoints[lineNr];
            Vector2 np = (p1 + p2) / 2f;
            path.inbetweenPoints.Insert(lineNr, np);
            return lineNr;
        }

        /**
         * Removes a random inbetween point in a path.
         * @param path Reference to the path to remove
         * a point from. This object will be modified
         * by the method.
         * @return Returns the index of the point that
         * was removed from the path.
         */
        public int RemoveRandomPoint(ref PathDNA path)
        {
            int pointIndex = -1;
            if (path.inbetweenPoints.Count > 0)
            {
                pointIndex = Random.Range((int)0, (int)path.inbetweenPoints.Count);
                path.inbetweenPoints.RemoveAt(pointIndex);
            }
            return pointIndex;
        }
        
        /** Mutates the inbetween points in a path.
         * @param path Reference to he path whose points
         * are to be mutated. This object will be modified
         * by this method.
         */
        public void MutatePoints(ref PathDNA path)
        {
            float range = Mathf.Pow(10, Random.Range(-1f, 1f));
            int n = path.inbetweenPoints.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2 p = path.inbetweenPoints[i];
                p.x += Random.Range(-range, range);
                p.y += Random.Range(-range, range);
                path.inbetweenPoints[i] = p;
            }
        }
	}
}

