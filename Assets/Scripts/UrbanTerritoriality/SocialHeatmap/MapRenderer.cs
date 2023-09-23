using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.Maps
{
    /** A class with some methods useful for rendering layers in a map */
    public class MapRenderer
    {
        /** A delegate for a function that
         * takes two colors as an argument
         * and returns a color as output. */
        public delegate Color ColoringFunction(Color current, Color replacement);

        /** Replace a color with another one.
         * @param col1 The first color
         * @param col2 The second color.
         * @returns The second color.
         * */
        public static Color ReplaceColor(Color col1, Color col2)
        {
            return col2;
        }

        /** Add two colors together.
         * @param col1 The first color.
         * @param col2 The second color.
         * @return The two colors added together.
         * */
        public static Color AddColor(Color col1, Color col2)
        {
            return col1 + col2;
        }

        /** Mix col1 and col2 based on the alpha value of col2 
         * and add alphas of both.
         * @param col1 The first color.
         * @param col2 The second color.
         * @return Returns col1 and col2 mixed based
         * on the alpha values of col2 with the alpha
         * value being the sum of col1 and col2.
         */
        public static Color MixColor(Color col1, Color col2)
        {
            float a = col2.a;
            float one_a = 1 - a;

            float r = col1.r * one_a +
                col2.r * a;
            float g = col1.g * one_a +
                col2.g * a;
            float b = col1.b * one_a +
                col2.b * a;
            return new Color(r, g, b, col1.a + col2.a);
        }
        
        /** Mix col1 and col2 based on the alpha value of col1
         * and add alphas of both.
         * @param col1 The first color
         * @param col1 The second color
         * @returns Col1 and col2 mixed based on the alpha
         * value of col1 with the alpha of the resulting
         * color being the sum of the alpha of col1 and
         * col2.
         */
        public static Color MixColorReverse(Color col1, Color col2)
        {
            return MixColor(col2, col1);
        }

        /** Adds a grid showing the cells in a map
         * to a texture.
         * @param tex The texture represented as
         * an array of colors.
         * @param texWidth The width of the texture.
         * @param texHeight The height of the texture.
         * @param map A PaintGrid containing the map data.
         * @param col The color of the grid.
         * @param colorFunction A ColoringFunction to use
         * when painting the grid onto the map.
         */
        public static void AddGrid(ref Color[] tex,
            int texWidth, int texHeight,
            PaintGrid map, Color col, ColoringFunction colorFunction)
        {
            int mapWidth = map.Width;
            int mapHeight = map.Height;
            int mapX = 0;
            int mapY = 0;
            int lastMapX = -1;
            int lastMapY = -1;

            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    int texIndex = y * texWidth + x;
                    mapX = x * mapWidth / texWidth;
                    mapY = y * mapHeight / texHeight;

                    if (mapX != lastMapX || mapY != lastMapY)
                    {
                        tex[texIndex] = colorFunction(tex[texIndex], col);
                    }
                    lastMapX = mapX;
                }
                lastMapY = mapY;
            }
        }

        /** Add a color to a texture. A ColoringFunction is
         * used to control how the color and the color
         * currently in the texture are mixed together.
         * @param tex The texture represented as an
         * array of colors.
         * @param texWidth The width of the texture.
         * @param texHeight The height of the texture.
         * @param col The color to use.
         * @param colorFunction a function controlling how
         * col and the color in the texture are mixed.
         */ 
        public static void AddColor(ref Color[] tex,
            int texWidth, int texHeight,
            Color col, ColoringFunction colorFunction)
        {
            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texHeight; x++)
                {
                    int texIndex = y * texWidth + x;
                    tex[texIndex] = colorFunction(tex[texIndex], col);
                }
            }
        }

        /** Render a map layer to a texture.
         * @param tex The texture to render to represented
         * as an array of colors.
         * @param texWidth The width of the texture.
         * @param texHeight The height of the texture.
         * @param map A PaintGrid with the map data.
         * @param col The color to use for the map.
         * @param colorFunction A ColoringFunction to
         * use for mixing col with the color currently
         * in the tex array.
         * */
        public static void RenderMapLayer(ref Color[] tex,
            int texWidth, int texHeight,
            PaintGrid map, Color col, ColoringFunction colorFunction)
        {
            int mapWidth = map.Width;
            int mapHeight = map.Height;

            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    int texIndex = y * texWidth + x;
                    int mapX = x * mapWidth / texWidth;
                    int mapY = y * mapHeight / texHeight;
                    int mapIndex = mapY * mapWidth + mapX;
                    Color addedColor = col * map.grid[mapIndex];
                    Color currentColor = tex[texIndex];
                    tex[texIndex] = colorFunction(currentColor, addedColor);
                }
            }
        }
    }
}

