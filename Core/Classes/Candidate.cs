using System.Drawing;
using GenArt.Core.AST;

namespace GenArt.Core.Classes
{
    public class Candidate
    {
        private readonly DnaDrawing drawing;
        private double errorLevel;
        private Bitmap drawingAsBitmap;

        public Candidate()
        {
            drawing = new DnaDrawing();
            drawing.Init();
            errorLevel = double.MaxValue;
        }

        public Candidate(Candidate candidate)
        {
            drawing = candidate.Drawing.Clone();
            errorLevel = double.MaxValue;
        }

        public Candidate(DnaDrawing drawing)
        {
            this.drawing = drawing;
            errorLevel = double.MaxValue;
        }

        public DnaDrawing Drawing
        {
            get { return drawing; }
        }

        public double ErrorLevel
        {
            get { return errorLevel; }
        }

        public void CalculateErrorLevel(Bitmap sourceBitmap)
        {
            if (drawingAsBitmap == null)
            {
                drawingAsBitmap = Renderer.RenderToBitmap(drawing, 1);
            }
            errorLevel = FitnessCalculatorAsm.GetDrawingFitness(drawingAsBitmap, sourceBitmap);
        }
    }
}