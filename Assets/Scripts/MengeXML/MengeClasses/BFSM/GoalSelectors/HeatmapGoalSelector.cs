using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace Menge
{
    namespace BFSM
    {
        // Struct to hold the result of each thread
        struct ThreadResult
        {
            public Vector2 position; // xy position of the pixel in the texture
            public float redValue; // highest red value in the portion processed by this thread
        }

        [Serializable]
        public class HeatmapGoalSelector : GoalSelector
        {
            public override string Name => "heatmap";

            public Texture spatialHeatmap;
            public Texture fieldOfPerceptionlHeatmap;
            public RenderTexture outputHeatmap;
            public Vector2 offset;
            public float rotation;
            public Material combineTexturesMaterial;


            // compute shader variables to retrieve the pixel with the highest value in a texture
            public ComputeShader computeShader;
            public ComputeBuffer resultBuffer;

            public override XmlSchema GetSchema()
            {
                return null;
            }

            public override void ReadXml(XmlReader reader)
            {
                throw new NotImplementedException();
            }

            public override void WriteXml(XmlWriter writer)
            {
                writer.WriteStartElement("GoalSelector");
                writer.WriteAttributeString("type", Name);
                writer.WriteEndElement();
            }


            public void updateHeatmap()
            {
                // return if the input textures are null
                if (spatialHeatmap == null || fieldOfPerceptionlHeatmap == null) return;

                // Debug.Log("updating heatmap texture");

                // use the material to combine the textures and store the result in the output texture
                combineTexturesMaterial.SetTexture("_tex1", spatialHeatmap);
                combineTexturesMaterial.SetTexture("_tex2", fieldOfPerceptionlHeatmap);
                combineTexturesMaterial.SetVector("_offset", offset);
                combineTexturesMaterial.SetFloat("_rotation", rotation);
                combineTexturesMaterial.SetFloat("_normalize", 1);

                // create a temporary render texture to store the result of the material
                if(outputHeatmap != null) outputHeatmap.Release();
                outputHeatmap = new RenderTexture(spatialHeatmap.width, spatialHeatmap.height, 0, spatialHeatmap.graphicsFormat);
                RenderTexture.active = outputHeatmap;
                Graphics.Blit(spatialHeatmap, outputHeatmap);
                Graphics.Blit(fieldOfPerceptionlHeatmap, outputHeatmap, combineTexturesMaterial);


                RenderTexture.active = null;

            }

            // sets the field of perception values so that the spatial heatmap is 
            // updated based on the position and orientation of the agent, then
            // returns a goal based on a spacial heatmap and a perception field
            // if the agent cannot perceive any goal, it returns null
            // for now this is based on the red value of the pixel with the highest value
            public PointGoal getGoal(UnityEngine.Vector2 position, UnityEngine.Vector2 orientation)
            {
                if (spatialHeatmap == null) return null;

                // center the position on the field of view texture by subtracting half the width and height
                UnityEngine.Vector2 agentCenterFOV = new UnityEngine.Vector2(position.x - fieldOfPerceptionlHeatmap.width / 2, position.y - fieldOfPerceptionlHeatmap.height / 2);
                // UnityEngine.Vector2 agentCenterFOV = new UnityEngine.Vector2(position.x, position.y);

                // compute the offset in terms of UV coordinates
                // note that, for some reason, in the shader graph the directions are "flipped"
                // a negative offset moves the texture to the right and a positive offset moves the texture to the left
                Vector2 uvRatio = new Vector2((float)spatialHeatmap.width / fieldOfPerceptionlHeatmap.width, (float)spatialHeatmap.height / fieldOfPerceptionlHeatmap.height);
                offset = -(new Vector2((agentCenterFOV.x * uvRatio.x) / spatialHeatmap.width, (agentCenterFOV.y * uvRatio.y) / spatialHeatmap.height));

                // compute the rotation in degrees by computing the angle between the orientation and the 2D "up" vector (0, 1)
                Vector2 up2D = new Vector2(0.0f, 1.0f); // 2D "up" vector
                orientation.Normalize();
                up2D.Normalize();
                float cosAngle = Vector2.Dot(orientation, up2D);
                float angle = Mathf.Acos(cosAngle);
                float angleInDegrees = angle * Mathf.Rad2Deg;
                // determine the direction of the angle with the cross product (if the sign is positive, the angle is clockwise)
                float sign = Mathf.Sign(orientation.x * up2D.y - orientation.y * up2D.x); // determine direction of angle

                // same as for the offset, the direction of the rotation is "flipped" in the shader graph
                // a negative rotation rotates the texture clockwise and a positive rotation rotates the texture counter-clockwise
                rotation = -(angleInDegrees * sign); // multiply angle by direction to get final angle
                // Debug.LogFormat("Orientation: {0}", rotation);

                return getGoal();
            }

            // returns a goal based on a spacial heatmap and a perception field
            // if the agent cannot perceive any goal, it returns null
            // for now this is based on the red value of the pixel with the highest value
            public PointGoal getGoal()
            {
                if (spatialHeatmap == null) return null;

                updateHeatmap();

                // use a compute shader to retrieve the pixel with the highest value
                // considering that the texture is grayscale and so RGB values are the same
                // we can use the R value to retrieve the highest value
                // the compute shader will return the x and y coordinates of the pixel with the highest value
                ThreadResult highestValuePixel = getHighestValuePixel();

                if (highestValuePixel.redValue == 0)
                {
                    return null;
                }

                return new PointGoal(highestValuePixel.position);
            }

            private ThreadResult getHighestValuePixel()
            {
                // Create the result buffer with the same length as the number of threads
                int numThreadsX = Mathf.CeilToInt((float)outputHeatmap.width / 16);
                int numThreadsY = Mathf.CeilToInt((float)outputHeatmap.height / 16);
                resultBuffer = new ComputeBuffer(numThreadsX * numThreadsY, sizeof(float) * 3);

                // Set the compute shader parameters
                int kernelHandle = computeShader.FindKernel("CSMain");
                computeShader.SetTexture(kernelHandle, "inputTexture", outputHeatmap);
                computeShader.SetInt("inputTextureWidth", outputHeatmap.width);
                computeShader.SetInt("inputTextureHeight", outputHeatmap.height);
                computeShader.SetBuffer(kernelHandle, "resultBuffer", resultBuffer);

                // Dispatch the compute shader
                computeShader.Dispatch(kernelHandle, numThreadsX, numThreadsY, 1);

                // Read the result buffer into an array
                ThreadResult[] results = new ThreadResult[numThreadsX * numThreadsY];
                resultBuffer.GetData(results);

                // Find the pixel with the highest red value among all results
                ThreadResult highestPixelValue = new ThreadResult();
                highestPixelValue.position = Vector2.zero;
                highestPixelValue.redValue = 0;

                foreach (ThreadResult result in results)
                {
                    if (result.redValue > highestPixelValue.redValue)
                    {
                        highestPixelValue = result;
                        // highestPixelValue.redValue = result.redValue;
                        // highestPixelValue.position.x result.position.x;
                    }
                }

                // Release the result buffer
                resultBuffer.Release();

                return highestPixelValue;
            }
        }
    }
}