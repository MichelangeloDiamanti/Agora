using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

namespace Agora.Evaluator.Metrics
{
    public class PSNREvaluatorNode : Node
    {
        [Input] public Texture2D image1;
        [Input] public Texture2D image2;
        [Input] public Texture2D mask;
        [Output] public double psnrValue;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "psnrValue")
            {
                psnrValue = GetPSNR();
                return psnrValue;
            }
            return null;
        }

        public double GetPSNR()
        {
            double psnr = 0;
            image1 = GetInputValue<Texture2D>("image1");
            image2 = GetInputValue<Texture2D>("image2");
            Texture2D maskTexture = GetInputValue<Texture2D>("mask");
            if (image1 == null || image2 == null)
            {
                return 0; // Return an error value if the inputs are not connected
            }

            if (image1.width != image2.width || image1.height != image2.height)
            {
                Debug.LogWarning("Cannot compute PSNR for images with different dimensions.");
                return 0; // Return an error value if the images have different sizes
            }

            Mat i1 = new Mat(image1.height, image1.width, CvType.CV_8UC3);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(image1, i1);

            Mat i2 = new Mat(image2.height, image2.width, CvType.CV_8UC3);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(image2, i2);

            // If a mask is provided, compute the PSNR only on the masked region
            if (maskTexture != null)
            {
                Mat maskMat = new Mat(maskTexture.height, maskTexture.width, CvType.CV_8UC1);
                OpenCVForUnity.UnityUtils.Utils.texture2DToMat(maskTexture, maskMat);

                // Calculate the MSE with the mask applied
                Mat diff = new Mat();
                Core.absdiff(i1, i2, diff);
                Core.pow(diff, 2, diff);

                // copy the values from diff to maskedDiff only for the positions where the mask has non-zero values 
                Mat maskedDiff = new Mat();
                diff.copyTo(maskedDiff, maskMat);

                double sumMaskedDiff = Core.sumElems(maskedDiff).val[0] + Core.sumElems(maskedDiff).val[1] + Core.sumElems(maskedDiff).val[2];

                // calculates the number of non-zero pixels in the mask (white pixels)
                int numWhitePixels = Core.countNonZero(maskMat);

                // calculates the MSE (mean of the squared differences) only for the significant portions
                // so the pixels that are not masked out
                double mse = sumMaskedDiff / (numWhitePixels * 3);

                // Calculate the PSNR
                psnr = 10 * System.Math.Log10(System.Math.Pow(255, 2) / mse);
            }
            else
            {
                // Calculate the PSNR
                psnr = OpenCVForUnity.CoreModule.Core.PSNR(i1, i2);
            }

            // PSNR is expressed in decibels, that can go from 0 to infinity. The higher the value, the better the quality of the image.
            // we want to express the PSNR in a range from 0 to 100, so we need to normalize it.
            // double normalizedPsnr = SigmoidFunction(OpenCVForUnity.CoreModule.Core.PSNR(i1, i2));

            // return normalizedPsnr * 100;
            return psnr;
        }

        /*
        The sigmoid function is a popular choice for mapping unbounded values to a fixed range because it's smooth, continuous, and has an S-shaped curve. The output of the sigmoid function ranges from 0 to 1, with values close to 0 for large negative inputs and values close to 1 for large positive inputs.
        */
        public double SigmoidFunction(double x)
        {
            double scaleFactor = 0.1;
            double shiftFactor = 25;
            return 1.0 / (1.0 + System.Math.Exp(-scaleFactor * (x - shiftFactor)));
        }
    }
}
