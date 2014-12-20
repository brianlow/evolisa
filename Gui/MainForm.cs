using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using GenArt.Classes;
using GenArt.Core.AST;
using GenArt.Core.Classes;

namespace GenArt
{
    public partial class MainForm : Form
    {
        public static Settings Settings;

        private Candidate currentCandidate = new Candidate();

//        private int generation;
        private bool isRunning;
        private DateTime lastRepaint = DateTime.MinValue;
        private int lastSelected;
        private TimeSpan repaintIntervall = new TimeSpan(0, 0, 0, 0, 0);
        private int repaintOnSelectedSteps = 3;
//        private int selected;
        private SettingsForm settingsForm;
        private Color[,] sourceColors;
        private DateTime startTime;
        private int sequence;

        private Thread thread;

        public MainForm()
        {
            InitializeComponent();
            Settings = Serializer.DeserializeSettings();
            if (Settings == null)
                Settings = new Settings();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Trace.WriteLine("");
            Trace.WriteLine("--------------------------------------------------------------------");
            Trace.WriteLine("Starting app...");
            Trace.WriteLine(string.Format(
                "{0,-19} {1,15} {2,15} {3,9} {4,9} {5,9} {6,9}", 
                "Date", "ErrorLevel", "Generation", "Gen/sec", "Points", "Polygons", "Pts/Poly"));
        }



//        private Bitmap GetSourceBitmap()
//        {
//            Bitmap bitmap = (Bitmap) picPattern.Image;
//            return bitmap.Clone(new Rectangle(new Point(0,0), bitmap.Size), PixelFormat.Format32bppArgb);            
//        }

        private void StartEvolution()
        {
            Evolution evolution = new Evolution((Bitmap) picPattern.Image);
            evolution.ReachedSignificantChange += evolution_ReachedSignificantChange;
            evolution.StartEvolution();

        }

        private void evolution_ReachedSignificantChange(Evolution evolution)
        {
            if (InvokeRequired)
            {
                Evolution.ReachedSignificantChangeEvent d = evolution_ReachedSignificantChange;
                Invoke(d, evolution);
                return;
            }

            lock (currentCandidate)
            {
                currentCandidate = evolution.CurrentCandidate;
                pnlCanvas.Invalidate();
            }

            //SaveImageToFile(currentCandidate.Drawing);
            //SaveDrawingToFile(currentCandidate.Drawing)


//            TimeSpan totalTime = DateTime.Now - startTime;
//            double generationsPerSec = generation/totalTime.TotalSeconds;
//            int polygons = current.Drawing.Polygons.Count;
//            int points = current.Drawing.PointCount;
//            double pointsPerPolygon = 0;
//            if (polygons != 0)
//                pointsPerPolygon = (double)points / polygons;
//
//
//            Trace.WriteLine(string.Format(
//                "{0:yyyy-MM-dd HH:mm:ss} {1,15} {2,15} {3,9:0.0} {4,9} {5,9} {6,9:0.0}",
//                DateTime.Now, currentCandidate .ErrorLevel, generation, generationsPerSec, points, polygons, pointsPerPolygon));
            
        }

//        private void StartEvolution()
//        {
//            double errorLevelAtLastSave = double.MaxValue;
//            Bitmap sourceBitmap = GetSourceBitmap();
//
//            if (currentCandidate == null)
//            {
//                currentCandidate = new Candidate();
//            }
//            lastSelected = 0;
//
//            while (isRunning)
//            {
//                var newCandidiate = new Candidate(currentCandidate);
//                newCandidiate.Drawing.Mutate();
//
//                if (newCandidiate.Drawing.IsDirty)
//                {
//                    generation++;
//
//                    newCandidiate.CalculateErrorLevel(sourceBitmap);
//
//                    if (newCandidiate.ErrorLevel <= currentCandidate.ErrorLevel)
//                    {
//                        selected++;
//
//                        lock (currentCandidate)
//                        {
//                            currentCandidate = newCandidiate;
//                        }
//
//
//                        double percentChange = (errorLevelAtLastSave - newCandidiate.ErrorLevel) /
//                                               errorLevelAtLastSave;
//                        if (percentChange > 0.005)
//                        {
//                            SaveImageToFile(newCandidiate.Drawing);
//
//                            errorLevelAtLastSave = newCandidiate.ErrorLevel;
//
//                            TimeSpan totalTime = DateTime.Now - startTime;
//                            double generationsPerSec = generation/totalTime.TotalSeconds;
//                            int polygons = currentCandidate.Drawing.Polygons.Count;
//                            int points = currentCandidate.Drawing.PointCount;
//                            double pointsPerPolygon = 0;
//                            if (polygons != 0)
//                                pointsPerPolygon = (double)points / polygons;
//
//
//                            Trace.WriteLine(string.Format(
//                                "{0:yyyy-MM-dd HH:mm:ss} {1,15} {2,15} {3,9:0.0} {4,9} {5,9} {6,9:0.0}",
//                                DateTime.Now, errorLevelAtLastSave, generation, generationsPerSec, points, polygons, pointsPerPolygon));
//                        }
//
//                    }
//                }
//                //else, discard new drawing
//            }
//        }

        private void SaveImageToFile(DnaDrawing drawing)
        {
            const string filenameFormat = @"F:\Temp\EvoLisaRender\EvoLisa-{0:000000000}.png";
            const int scale = 3;

            string filename = string.Format(filenameFormat, sequence++);

            using (Bitmap buffer = Renderer.RenderToBitmap(drawing, scale))
            {
                buffer.Save(filename, ImageFormat.Png);
            }
        }

//        ////covnerts the source image to a Color[,] for faster lookup
//        private void SetupSourceColorMatrix()
//        {
//            sourceColors = new Color[Tools.MaxWidth,Tools.MaxHeight];
//            var sourceImage = picPattern.Image as Bitmap;
//
//            if (sourceImage == null)
//                throw new NotSupportedException("A source image of Bitmap format must be provided");
//
//            for (int y = 0; y < Tools.MaxHeight; y++)
//            {
//                for (int x = 0; x < Tools.MaxWidth; x++)
//                {
//                    Color c = sourceImage.GetPixel(x, y);
//                    sourceColors[x, y] = c;
//                }
//            }
//        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isRunning)
                Stop();
            else
                Start();
        }

        private void Start()
        {
            btnStart.Text = "Stop";
            isRunning = true;
            tmrRedraw.Enabled = true;
            startTime = DateTime.Now;

            if (thread != null)
                KillThread();

            thread = new Thread(StartEvolution)
                         {
                             IsBackground = true,
                             Priority = ThreadPriority.AboveNormal
                         };

            thread.Start();
        }

        private void KillThread()
        {
            if (thread != null)
            {
                thread.Abort();
            }
            thread = null;
        }

        private void Stop()
        {
            if (isRunning)
                KillThread();

            btnStart.Text = "Start";
            isRunning = false;
            tmrRedraw.Enabled = false;
        }

        private void tmrRedraw_Tick(object sender, EventArgs e)
        {
//            if (currentCandidate == null)
//                return;
//
//            int polygons = currentCandidate.Drawing.Polygons.Count;
//            int points = currentCandidate.Drawing.PointCount;
//            double avg = 0;
//            if (polygons != 0)
//                avg = points/polygons;
//            TimeSpan totalTime = DateTime.Now - startTime;
//            double generationsPerSec = currentCandidate.Generation / totalTime.TotalSeconds;
//
            toolStripStatusLabelFitness.Text = currentCandidate.ErrorLevel.ToString();
//            toolStripStatusLabelGeneration.Text = currentCandidate.Generation.ToString();
//            toolStripStatusLabelSelected.Text = selected.ToString();
//            toolStripStatusLabelGenerationsPerSec.Text = generationsPerSec.ToString("0.0");
//
//            bool shouldRepaint = false;
//            if (repaintIntervall.Ticks > 0)
//                if (lastRepaint < DateTime.Now - repaintIntervall)
//                    shouldRepaint = true;
//
////            if (repaintOnSelectedSteps > 0)
////                if (lastSelected + repaintOnSelectedSteps < selected)
////                    shouldRepaint = true;
//
//            if (shouldRepaint)
//            {
//                pnlCanvas.Invalidate();
//                lastRepaint = DateTime.Now;
//                lastSelected = selected;
//            }
        }

        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (currentCandidate == null)
            {
                e.Graphics.Clear(Color.Black);
                return;
            }

            DnaDrawing currentDrawing = currentCandidate.Drawing.Clone();

            int scale = trackBarScale.Value;
            Bitmap buffer = Renderer.RenderToBitmap(currentDrawing, scale);
            e.Graphics.DrawImage(buffer, 0, 0);
        }

        private void OpenImage()
        {
            Stop();

            string fileName = FileUtil.GetOpenFileName(FileUtil.ImgExtension);
            if (string.IsNullOrEmpty(fileName))
                return;

            picPattern.Image = Image.FromFile(fileName);

            Tools.MaxHeight = picPattern.Height;
            Tools.MaxWidth = picPattern.Width;

            SetCanvasSize();

            splitContainer1.SplitterDistance = picPattern.Width + 30;
        }

        private void SetCanvasSize()
        {
            pnlCanvas.Height = trackBarScale.Value*picPattern.Height;
            pnlCanvas.Width = trackBarScale.Value*picPattern.Width;
            pnlCanvas.Invalidate();
        }

        private void OpenDNA()
        {
            Stop();

            DnaDrawing drawing = Serializer.DeserializeDnaDrawing(FileUtil.GetOpenFileName(FileUtil.DnaExtension));
            if (drawing != null)
            {
                currentCandidate = new Candidate(drawing);
                pnlCanvas.Invalidate();
                lastRepaint = DateTime.Now;
            }
        }

        private void SaveDNA()
        {
            string fileName = FileUtil.GetSaveFileName(FileUtil.DnaExtension);
            if (string.IsNullOrEmpty(fileName) == false && currentCandidate != null)
            {
                lock (currentCandidate)
                {
                    SaveDnaDrawingToFile(fileName, currentCandidate.Drawing);
                }
            }
        }

        private static void SaveDnaDrawingToFile(string fileName, DnaDrawing drawing)
        {
            DnaDrawing clone;
            clone = drawing.Clone();
            if (clone != null)
                Serializer.Serialize(clone, fileName);
        }


        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settingsForm != null)
                if (settingsForm.IsDisposed)
                    settingsForm = null;

            if (settingsForm == null)
                settingsForm = new SettingsForm();

            settingsForm.Show();
        }

        private void sourceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImage();
        }

        private void dNAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDNA();
        }

        private void dNAToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveDNA();
        }

        private void trackBarScale_Scroll(object sender, EventArgs e)
        {
            SetCanvasSize();
        }
    }
}