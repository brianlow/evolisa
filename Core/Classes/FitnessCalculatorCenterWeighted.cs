using System;
using System.Drawing;
using System.Drawing.Imaging;
using GenArt.Classes;

namespace GenArt.Core.Classes
{
    public class FitnessCalculatorCenterWeighted
    {
        public static double GetDrawingFitness(Bitmap newBitmap, Bitmap sourceBitmap)
        {
            double error = 0;

            BitmapData newBits = newBitmap.LockBits(
                new Rectangle(0, 0, Tools.MaxWidth, Tools.MaxHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            BitmapData sourceBits = sourceBitmap.LockBits(
                new Rectangle(0, 0, Tools.MaxWidth, Tools.MaxHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            unchecked
            {
                unsafe
                {
                    Pixel* p1 = (Pixel*)newBits.Scan0.ToPointer();
                    Pixel* p2 = (Pixel*)sourceBits.Scan0.ToPointer();
                    int numPixels = newBits.Width*newBits.Height;
                    for (int i = 0; i < numPixels; i += 1)
                    {
                        int r = p1->R - p2->R;
                        int g = p1->G - p2->G;
                        int b = p1->B - p2->B;
                        error += r * r + g * g + b * b;
                        p1 += 1;
                        p2 += 1;
                    }

                    int x1 = newBits.Width / 4;
                    int x2 = newBits.Width / 4 * 3;
                    int y1 = newBits.Height / 4;
                    int y2 = newBits.Height / 4 * 3;

                    int heightOfCenter = y2 - y1;
                    int widthOfCenter = x2 - x1;

                    p1 += y1*newBits.Width;
                    p2 += y1*newBits.Width;
                    for (int y = y1; y < y2; y++)
                    {
                        p1 = (Pixel*)newBits.Scan0.ToPointer();
                        p2 = (Pixel*)sourceBits.Scan0.ToPointer();
                        p1 += y * newBits.Width + x1;
                        p2 += y * newBits.Width + x1;

                        for (int x = x1; x < x2; x++)
                        {
                            int r = p1->R - p2->R;
                            int g = p1->G - p2->G;
                            int b = p1->B - p2->B;
                            error += r * r + g * g + b * b;
                            p1 += 1;
                            p2 += 1;
                        }
                    }
                }
            }
            sourceBitmap.UnlockBits(sourceBits);
            newBitmap.UnlockBits(newBits);

            return error;
        }

    }
}
