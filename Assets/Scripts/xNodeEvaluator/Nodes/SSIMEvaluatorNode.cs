using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

namespace Agora.Evaluator.Metrics
{
    public class SSIMEvaluatorNode : Node
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
                similarityScore = GetMSSIM();
                return similarityScore;
            }
            return null;
        }

        // Implementation from https://docs.opencv.org/4.x/d5/dc4/tutorial_video_input_psnr_ssim.html
        // since the metric was not readily available in OpenCVForUnity
        // computes the mean of the structural similarity index 
        public double GetMSSIM()
        {
            image1 = GetInputValue<Texture2D>("image1");
            image2 = GetInputValue<Texture2D>("image2");
            Texture2D maskTexture = GetInputValue<Texture2D>("mask");
            if (image1 == null || image2 == null)
            {
                return 0; // Return an error value if the inputs are not connected
            }

            if (image1.width != image2.width || image1.height != image2.height)
            {
                Debug.LogWarning("Cannot compute SSIM for images with different dimensions.");
                return 0; // Return an error value if the images have different sizes
            }

            Mat i1 = new Mat(image1.height, image1.width, CvType.CV_8UC3);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(image1, i1);

            Mat i2 = new Mat(image2.height, image2.width, CvType.CV_8UC3);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(image2, i2);

            double C1 = 6.5025, C2 = 58.5225;
            int d = CvType.CV_32F;

            Mat I1 = new Mat(), I2 = new Mat();
            i1.convertTo(I1, d);
            i2.convertTo(I2, d);

            Mat I1_2 = I1.mul(I1);
            Mat I2_2 = I2.mul(I2);
            Mat I1_I2 = I1.mul(I2);

            Mat mu1 = new Mat(), mu2 = new Mat();
            Imgproc.GaussianBlur(I1, mu1, new Size(11, 11), 1.5);
            Imgproc.GaussianBlur(I2, mu2, new Size(11, 11), 1.5);

            Mat mu1_2 = mu1.mul(mu1);
            Mat mu2_2 = mu2.mul(mu2);
            Mat mu1_mu2 = mu1.mul(mu2);

            Mat sigma1_2 = new Mat(), sigma2_2 = new Mat(), sigma12 = new Mat();
            Imgproc.GaussianBlur(I1_2, sigma1_2, new Size(11, 11), 1.5);
            Core.subtract(sigma1_2, mu1_2, sigma1_2);

            Imgproc.GaussianBlur(I2_2, sigma2_2, new Size(11, 11), 1.5);
            Core.subtract(sigma2_2, mu2_2, sigma2_2);

            Imgproc.GaussianBlur(I1_I2, sigma12, new Size(11, 11), 1.5);
            Core.subtract(sigma12, mu1_mu2, sigma12);

            Mat t1 = new Mat(), t2 = new Mat(), t3 = new Mat();
            Mat C1Mat = Mat.ones(mu1_mu2.size(), mu1_mu2.type()) * C1;
            Mat C2Mat = Mat.ones(mu1_mu2.size(), mu1_mu2.type()) * C2;
            Core.add(mu1_mu2, mu1_mu2, t1);
            Core.add(t1, C1Mat, t1);

            Core.add(sigma12, sigma12, t2);
            Core.add(t2, C2Mat, t2);

            t3 = t1.mul(t2);

            Core.add(mu1_2, mu2_2, t1);
            Core.add(t1, C1Mat, t1);

            Core.add(sigma1_2, sigma2_2, t2);
            Core.add(t2, C2Mat, t2);

            t1 = t1.mul(t2);

            Mat ssim_map = new Mat();
            Core.divide(t3, t1, ssim_map);

            Scalar mssim = Core.mean(ssim_map);

            // If a mask is provided, compute the SSIM only on the masked region
            if (maskTexture != null)
            {
                // we are computing the SSIM for the entire image and then averaging the SSIM values within the masked region
                // this is not the same as computing the SSIM only on the masked region, as the computation is inherently influenced 
                // by the neighboring pixels due to the Gaussian blur operation
                Mat maskMat = new Mat(maskTexture.height, maskTexture.width, CvType.CV_8UC1);
                OpenCVForUnity.UnityUtils.Utils.texture2DToMat(maskTexture, maskMat);

                Mat maskedSsimMap = new Mat();
                ssim_map.copyTo(maskedSsimMap, maskMat);

                int numWhitePixels = Core.countNonZero(maskMat);
                double sumMaskedSsim = Core.sumElems(maskedSsimMap).val[0];

                return (sumMaskedSsim / numWhitePixels) * 100;
            }

            return mssim.val[0] * 100;
        }

    }
}