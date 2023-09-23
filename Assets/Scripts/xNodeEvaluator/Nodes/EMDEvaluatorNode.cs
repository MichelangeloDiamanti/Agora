using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.XimgprocModule;

namespace Agora.Evaluator.Metrics
{
    public class EMDEvaluatorNode : Node
    {
        [Input] public Texture2D image1;
        [Input] public Texture2D image2;
        [Input] public Texture2D mask;


        public int maxWidth = 100;
        public int maxHeight = 100;

        [Output] public float emd;

        protected override void Init()
        {
            base.Init();
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "emd")
            {
                emd = CalculateEMD();
                return emd;
            }
            return null;
        }

        public float CalculateEMD()
        {
            image1 = GetInputValue<Texture2D>("image1");
            image2 = GetInputValue<Texture2D>("image2");
            Texture2D maskTexture = GetInputValue<Texture2D>("mask");
            if (image1 == null || image2 == null)
            {
                return 0;
            }

            if (image1.width != image2.width || image1.height != image2.height)
            {
                Debug.LogWarning("Cannot compute EMD for images with different dimensions.");
                return 0;
            }

            Mat mat1 = new Mat(image1.height, image1.width, CvType.CV_8UC4);
            Mat mat2 = new Mat(image2.height, image2.width, CvType.CV_8UC4);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(image1, mat1);
            OpenCVForUnity.UnityUtils.Utils.texture2DToMat(image2, mat2);

            // If a mask is provided apply the mask to both images. This will set the masked regions to black (zero intensity)
            // and the EMD will be computed only on the masked regions (it already ignores zero intensity pixels)
            if (maskTexture != null)
            {
                Mat maskMat = new Mat(maskTexture.height, maskTexture.width, CvType.CV_8UC1);
                OpenCVForUnity.UnityUtils.Utils.texture2DToMat(maskTexture, maskMat);

                // Set masked regions to black (zero intensity) in both images
                mat1.setTo(new Scalar(0, 0, 0, 0), maskMat);
                mat2.setTo(new Scalar(0, 0, 0, 0), maskMat);
            }

            float aspectRatio = (float)mat1.width() / mat1.height();
            int newWidth = maxWidth;
            int newHeight = Mathf.RoundToInt(newWidth / aspectRatio);

            if (newHeight > maxHeight)
            {
                newHeight = maxHeight;
                newWidth = Mathf.RoundToInt(newHeight * aspectRatio);
            }

            Size newSize = new Size(newWidth, newHeight);

            Imgproc.resize(mat1, mat1, newSize);
            Imgproc.resize(mat2, mat2, newSize);

            Mat gray1 = new Mat();
            Mat gray2 = new Mat();
            Imgproc.cvtColor(mat1, gray1, Imgproc.COLOR_RGBA2GRAY);
            Imgproc.cvtColor(mat2, gray2, Imgproc.COLOR_RGBA2GRAY);

            Mat signature1 = CreateSignature(gray1);
            Mat signature2 = CreateSignature(gray2);


            float emd;
            try
            {
                emd = Imgproc.EMD(signature1, signature2, Imgproc.CV_DIST_L2);
            }
            catch (System.Exception)
            {
                throw;
            }

            return emd;
        }

        Mat CreateSignature(Mat gray)
        {
            int nonzeroCount = Core.countNonZero(gray);
            Mat signature = new Mat(nonzeroCount, 3, CvType.CV_32FC1);

            int idx = 0;
            for (int i = 0; i < gray.rows(); i++)
            {
                for (int j = 0; j < gray.cols(); j++)
                {
                    float value = (float)gray.get(i, j)[0];
                    if (value > 0)
                    {
                        signature.put(idx, 0, value);
                        signature.put(idx, 1, i);
                        signature.put(idx, 2, j);
                        idx++;
                    }
                }
            }
            return signature;
        }
    }
}
