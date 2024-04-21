/*
 * File: ImageProcessing.cs
 * Description: 图像处理函数集
 * Author: tianlan
 * Last update at 24/3/3    22:25
 * 
 * Update Records:
 * tianlan  24/3/3  创建主框架
 */

using System.Collections.Generic;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Core
{
    public static class ImageProcessing
    {
        /// <summary>
        /// 处理操作的类型
        /// </summary>
        public enum ProcessType
        {
            GaussianBlur,   // 高斯模糊
            CommonSmooth    // 普通平滑
        }

        /// <summary>
        /// 处理参数
        /// </summary>
        private struct ProcessArgument
        {
            public int          BlurSize;       // 卷积核半径
            public ProcessType  ProcessType;    // 处理类型
        };

        /// <summary>
        /// 用于缓存已经计算过的权重矩阵
        /// </summary>
        private static readonly Dictionary<ProcessArgument, float[]> WeightCache = new();

        private static readonly int InputTexture = Shader.PropertyToID("InputTexture");
        private static readonly int OutputTexture = Shader.PropertyToID("OutputTexture");
        private static readonly int Width = Shader.PropertyToID("width");
        private static readonly int Height = Shader.PropertyToID("height");
        private static readonly int BlurSize = Shader.PropertyToID("BlurSize");

        /// <summary>
        /// 处理器委托
        /// </summary>
        /// <param name="blurSize">卷积核半径</param>
        /// <returns>权重矩阵</returns>
        private delegate float[] ProcessDelegate(int blurSize);

        /// <summary>
        /// 对图像进行平滑处理
        /// </summary>
        /// <param name="processType">处理类型</param>
        /// <param name="inTexture">输入纹理</param>
        /// <param name="outTexture">输出纹理</param>
        /// <param name="blurSize">卷积核半径</param>
        /// <param name="useBlackEdge">是否使用黑边</param>
        public static void SmoothProcess(ProcessType processType, Texture2D inTexture, out Texture2D outTexture, int blurSize = 3, bool useBlackEdge = false)
        {
            if(blurSize <= 0)
            {
                outTexture = inTexture;
                return;
            }

            // 高斯模糊特例值，使用GPU计算
            if(processType == ProcessType.GaussianBlur && useBlackEdge == false && blurSize > 0)
            {
                GuassianComputeShader(inTexture, out outTexture, blurSize);
                return;
            }

            switch (processType)
            {
                case ProcessType.GaussianBlur:
                    SmoothProcess(GetGaussianWeightMatrix, inTexture, out outTexture, blurSize, useBlackEdge);
                    break;
                case ProcessType.CommonSmooth:
                    SmoothProcess(GetCommonWeightMatrix, inTexture, out outTexture, blurSize, useBlackEdge);
                    break;
                default:
                    outTexture = inTexture;
                    break;
            }
        }

        /// <summary>
        /// 对图像进行平滑处理的具体执行函数
        /// </summary>
        /// <param name="processDelegate">处理器函数，用于获取权重矩阵</param>
        /// <param name="inTexture">输入纹理</param>
        /// <param name="outTexture">输出纹理</param>
        /// <param name="blurSize">卷积核半径</param>
        /// <param name="useBlackEdge">是否使用黑边</param>
        private static void SmoothProcess(ProcessDelegate processDelegate, Texture2D inTexture, out Texture2D outTexture, int blurSize, bool useBlackEdge)
        {
            var width = processDelegate.Invoke(blurSize);

            // foobar
            outTexture = new Texture2D(2, 2);
        }

        /// <summary>
        /// 获取对应的高斯模糊权重矩阵
        /// </summary>
        /// <param name="blurSize">卷积核半径</param>
        /// <returns>权重矩阵</returns>
        private static float[] GetGaussianWeightMatrix(int blurSize)
        {
            return new float[] { };
        }

        /// <summary>
        /// 获取对应的普通平滑权重矩阵
        /// </summary>
        /// <param name="blurSize">卷积核半径</param>
        /// <returns>权重矩阵</returns>
        private static float[] GetCommonWeightMatrix(int blurSize)
        {
            return new float[] { };
        }

        private static void GuassianComputeShader(Texture2D inTexture, out Texture2D outTexture, int blurSize)
        {
            ComputeShader computeShader = Resources.Load<ComputeShader>("ComputeShaders/GaussianComputeShader");
            if(!computeShader)
            {
                outTexture = inTexture;
                Logger.LogError("ImageProcessing:GaussianComputeShader() Failed to load compute shader.");
                return;
            }

            int kernelHandle = computeShader.FindKernel("CSMain");

            RenderTexture inRenderTexture = new RenderTexture(inTexture.width, inTexture.height, 0)
                {
                    enableRandomWrite = true
                };
            inRenderTexture.Create();

            Graphics.Blit(inTexture, inRenderTexture);

            RenderTexture outRenderTexture = new RenderTexture(inTexture.width, inTexture.height, 0)
                {
                    enableRandomWrite = true
                };
            outRenderTexture.Create();

            computeShader.SetTexture(kernelHandle, InputTexture, inRenderTexture);
            computeShader.SetTexture(kernelHandle, OutputTexture, outRenderTexture);
            computeShader.SetInt(Width, inTexture.width);
            computeShader.SetInt(Height, inTexture.height);
            computeShader.SetInt(BlurSize, blurSize);

            computeShader.Dispatch(kernelHandle, inTexture.width / 8, inTexture.height / 8, 1);

            outTexture = new Texture2D(inTexture.width, inTexture.height);
            RenderTexture.active = outRenderTexture;
            outTexture.ReadPixels(new Rect(0, 0, inTexture.width, inTexture.height), 0, 0);
            outTexture.Apply();

            RenderTexture.active = null;
            inRenderTexture.Release();
            outRenderTexture.Release();
        }
    }
}

