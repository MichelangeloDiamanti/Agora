using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.Agent;

namespace UrbanTerritoriality.GenAlg
{
    /** A type of a SocialPathProvider
     * providing a path by using a GeneticPathGenerator
     * to generate the path.
     */
    public class GeneticPathProvider : SocialPathProvider
    {
        /** A GeneticPathGenerator for generating the path. */
        public GeneticPathGenerator PathGenerator
        { get { return pathGenerator; } }
        private GeneticPathGenerator pathGenerator;

        /** Do some initialization */
        protected override void initialize()
        {
            pathGenerator = new GeneticPathGenerator(
                heatmap, agentGameObject, 10, 30);
            pathGenerator.colliderMap = colliderMap;
            pathGenerator.ownHeatSpaces = ownHeatSpaces;
            pathGenerator.start = agentGameObject.transform.position;
            pathGenerator.end = agentGameObject.transform.position;
            pathGenerator.pathFollowingAgent = pathFollowingAgent;
            pathGenerator.InitPopulation();
        }

        /** Unity Update method */
        private void Update()
        {
            if (initialized)
            {
                pathGenerator.colliderWeight = colliderWeight;
                pathGenerator.heatmapWeight = heatmapWeight;
                pathGenerator.maxLineRatioWeight = maxLineRatioWeight;
                pathGenerator.maxAngleWeight = maxAngleWeight;
                pathGenerator.agentAngleWeight = agentAngleWeight;
                pathGenerator.PassOneGeneration();
            }
        }

        /** Get the best path from start to end.
         * @param start The starting position of the path.
         * @param end The end position of the path.
         * @returns Returns an array of points representing the path,
         * including the start and end points. The starting position
         * is the first element in the array and the end position
         * is the last element in the array.
         */
        public override Vector2[] GetBestPath(Vector2 start, Vector2 end)
        {
            pathGenerator.start = start;
            pathGenerator.end = end;
            PathDNA path = pathGenerator.GetBestPath();
            Vector2[] totalPath = pathGenerator.GetTotalPath(path);
            return totalPath;
        }
    }
}
