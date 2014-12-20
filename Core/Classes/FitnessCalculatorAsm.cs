using System.Drawing;
using System.Drawing.Imaging;
using GenArt.Classes;

namespace GenArt.Core.Classes
{
    public static class FitnessCalculatorAsm
    {
        public static double GetDrawingFitness(Bitmap newBitmap, Bitmap sourceBitmap)
        {
            return Compare(newBitmap, sourceBitmap, new Rectangle(0, 0, Tools.MaxWidth, Tools.MaxHeight));
        }

        private static double Compare(Bitmap b1, Bitmap b2, Rectangle r)
        {
            BitmapData bd1 = b1.LockBits(r, ImageLockMode.ReadWrite, b1.PixelFormat);
            BitmapData bd2 = b2.LockBits(r, ImageLockMode.ReadWrite, b2.PixelFormat);
            double retVal = FIC.FastImageCompare.compare(
                bd1.Scan0,
                bd2.Scan0,
                r.Width * r.Height);
            b1.UnlockBits(bd1);
            b2.UnlockBits(bd2);
            return retVal;
        }
    }
}