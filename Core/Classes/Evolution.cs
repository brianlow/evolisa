using System.Drawing;
using System.Drawing.Imaging;

namespace GenArt.Core.Classes
{
    public class Evolution
    {
        private readonly Bitmap sourceBitmap;
        private Candidate currentCandidate;
        private long generation;
        private long selected;
        private double errorLevelAtLastEvent;

        public delegate void ReachedSignificantChangeEvent(Evolution evolution);
        public event ReachedSignificantChangeEvent ReachedSignificantChange;

        public Evolution(Bitmap sourceBitmap)
        {
            this.sourceBitmap = CloneBitmap(sourceBitmap);
            currentCandidate = new Candidate();
        }

        public Candidate CurrentCandidate
        {
            get { return currentCandidate; }
        }

        public long Generation
        {
            get { return generation; }
        }

        public long Selected
        {
            get { return selected; }
        }

        public void StartEvolution()
        {
            generation = 0;
            selected = 0;
            errorLevelAtLastEvent = double.MaxValue;

            while (true)
            {
                var newCandidiate = new Candidate(currentCandidate);
                while (newCandidiate.Drawing.IsDirty == false)
                {
                    newCandidiate.Drawing.Mutate();
                }

                generation++;

                newCandidiate.CalculateErrorLevel(sourceBitmap);

                if (newCandidiate.ErrorLevel <= currentCandidate.ErrorLevel)
                {
                    selected++;

                    lock (currentCandidate)
                    {
                        currentCandidate = newCandidiate;
                    }

                    if (HasChangedOverFivePercent())
                    {
                        if (ReachedSignificantChange != null)
                        {
                            ReachedSignificantChange(this);
                        }
                        errorLevelAtLastEvent = newCandidiate.ErrorLevel;
                    }
                }
            }
        }

        private bool HasChangedOverFivePercent()
        {
            double newErrorLevel = currentCandidate.ErrorLevel;
            double percentChange = (errorLevelAtLastEvent - newErrorLevel)/errorLevelAtLastEvent;
            return percentChange > 0.005;
        }

        private static Bitmap CloneBitmap(Bitmap sourceBitmap)
        {
            return sourceBitmap.Clone(new Rectangle(new Point(0, 0), sourceBitmap.Size), PixelFormat.Format32bppArgb);
        }

    }
}