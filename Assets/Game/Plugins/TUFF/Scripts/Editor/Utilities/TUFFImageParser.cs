using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    public static class TUFFImageParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path of the gif</param>
        /// <returns>The timing of each frame in a float array. Returns null if timings could not be obtained.</returns>
        public static float[] GetGIFTimings(string path)
        {
            float[] tmp = new float[0];
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Path was empty!");
                return null;
            }
            Image gif = GetImageFromPath(path);
            if (gif == null) {
                Debug.LogWarning("File could not be obtained!");
                return null;
            }
            if (!gif.RawFormat.Equals(ImageFormat.Gif)) {
                Debug.LogWarning("File is not a gif!");
                return null; 
            }
            if (!ImageAnimator.CanAnimate(gif)) {
                Debug.LogWarning("File is not animated!");
                return null;
            }
            var frameDimension = new FrameDimension(gif.FrameDimensionsList[0]);
            int frameCount = gif.GetFrameCount(frameDimension);
            byte[] propertyBytes = gif.GetPropertyItem(20736).Value; // Get the bytes corresponding to the delay property
            int bytes = 4;
            tmp = new float[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                int frameTime = System.BitConverter.ToInt32(propertyBytes, i * bytes);
                tmp[i] = frameTime * 0.01f;
            }

            return tmp;
        }
        public static Image GetImageFromPath(string path)
        {
            return Image.FromFile(path);
        }
    }
}