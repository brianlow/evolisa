using System.Drawing;
using System.Drawing.Imaging;
using GenArt.Classes;

namespace GenArt.Core.Classes
{
    public struct Pixel
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }

    public class FitnessCalculatorDanB
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
                    for (int i = 0; i < numPixels; i++)
                    {
                        int r = p1->R - p2->R;
                        int g = p1->G - p2->G;
                        int b = p1->B - p2->B;
                        error += r * r + g * g + b * b;
                        p1++;
                        p2++;
                    }
                }
            }
            sourceBitmap.UnlockBits(sourceBits);
            newBitmap.UnlockBits(newBits);

            return error;
        }

    }
}
