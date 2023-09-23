using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;

namespace Agora.Evaluator.Metrics
{
    public class MSEEvaluatorNode : Node
    {
        [Input] public Texture2D image1;
        [Input] public Texture2D image2;
        [Input] public Texture2D mask;
        [Output] public double similarityScore;

        private Texture2D lastImage1;
        private Texture2D lastImage2;

        private float maxPixelValue;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "similarityScore")
            {
                return similarityScore;
            }
            return null;
        }

        public void UpdateSimilarityScore()
        {
            Texture2D currentImage1 = GetInputValue<Texture2D>("image1");
            Texture2D currentImage2 = GetInputValue<Texture2D>("image2");

            if (currentImage1 != lastImage1 || currentImage2 != lastImage2)
            {
                lastImage1 = currentImage1;
                lastImage2 = currentImage2;
                similarityScore = GetNormalizedMSE();
            }
        }

        public double GetNormalizedMSE()
        {
            double rawSimilarityScore = CalculateMSE();

            // Normalize the MSE value between 0 and 100
            double normalizedMSE = (rawSimilarityScore / maxPixelValue) * 100;

            // Calculate similarity score where 100 is the most similar and 0 is the least similar
            double similarityScorePercentage = 100 - normalizedMSE;

            return similarityScorePercentage;
        }

        private int GetMaxPixelValue()
        {
            // Determine the maximum possible pixel value based on the image's format
            int maxPixelValue = 255; // Default to 8-bit images (0-255)

            if (image1 != null)
            {
                switch (image1.format)
                {
                    // 16-bit integer images
                    case TextureFormat.R16:
                    case TextureFormat.RG16:
                    case TextureFormat.RGB48:
                    case TextureFormat.RGBA64:
                        maxPixelValue = 65535;
                        break;

                    // 16-bit floating point images
                    case TextureFormat.RHalf:
                    case TextureFormat.RGHalf:
                    case TextureFormat.RGBAHalf:
                        maxPixelValue = int.MaxValue; // Float images have a very large range
                        break;

                    // 32-bit floating point images
                    case TextureFormat.RFloat:
                    case TextureFormat.RGFloat:
                    case TextureFormat.RGBAFloat:
                        maxPixelValue = int.MaxValue; // Float images have a very large range
                        break;

                    // 8-bit integer images (and other formats)
                    default:
                        maxPixelValue = 255;
                        break;
                }
            }

            return maxPixelValue;
        }

        public double CalculateMSE()
        {
            double mse = 0;
            Texture2D img1 = GetInputValue<Texture2D>("image1");
            Texture2D img2 = GetInputValue<Texture2D>("image2");
            Texture2D mask = GetInputValue<Texture2D>("mask");

            maxPixelValue = GetMaxPixelValue();

            if (img1 == null || img2 == null)
            {
                return maxPixelValue; // Return an error value if the inputs are not connected
            }

            if (img1.width != img2.width || img1.height != img2.height)
            {
                Debug.LogWarning("Cannot compute MSE for images with different dimensions.");
                return maxPixelValue; // Return an error value if the images have different sizes
            }

            Mat mat1 = new Mat(img1.height, img1.width, CvType.CV_8UC3);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(img1, mat1);

            Mat mat2 = new Mat(img2.height, img2.width, CvType.CV_8UC3);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(img2, mat2);

            // If a mask is provided, compute the MSE only on the masked region
            if (mask != null && mask.width == img1.width && mask.height == img1.height)
            {
                Mat maskMat = new Mat(mask.height, mask.width, CvType.CV_8UC1);
                OpenCVForUnity.UnityUtils.Utils.texture2DToMat(mask, maskMat);

                // Calculate the MSE with the mask applied
                Mat diff = new Mat();
                Core.absdiff(mat1, mat2, diff);
                Core.pow(diff, 2, diff);

                // copy the values from diff to maskedDiff only for the positions where the mask has non-zero values 
                Mat maskedDiff = new Mat();
                diff.copyTo(maskedDiff, maskMat);

                double sumMaskedDiff = Core.sumElems(maskedDiff).val[0] + Core.sumElems(maskedDiff).val[1] + Core.sumElems(maskedDiff).val[2];
                
                // calculates the number of non-zero pixels in the mask (white pixels)
                int numWhitePixels = Core.countNonZero(maskMat);

                // calculates the MSE (mean of the squared differences) only for the significant portions
                // so the pixels that are not masked out
                mse = sumMaskedDiff / (numWhitePixels * 3);
            }
            else
            {
                Mat diff = new Mat();
                Core.absdiff(mat1, mat2, diff);
                Core.pow(diff, 2, diff);
                Scalar mseScalar = Core.mean(diff);
                mse = (mseScalar.val[0] + mseScalar.val[1] + mseScalar.val[2]) / 3;
            }
            return mse;
        }
    }
}