using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

/// <summary>
/// FractalFun.Attractory
/// 
/// Creates attractors based on the code described in the book 
/// "Chaos In Wonderland" by Clifford A. Pickover pg 267 M.2
/// 
/// Changes:
/// 
/// 3/20/2019 DWR Moved file save and render logic into thier own procedures
/// 1.1.12.0      and out of the respective event handlers.
/// 3/20/2019 DWR Move most event handler logic into seperate procedures so
/// 1.1.13.0      so they can be called independant of the events and to 
///               make the code cleaner.
/// 3/21/2019 DWR Added UI elements to suport rendering multiple images
/// 1.1.14.0
/// 3/22/2019 DWR Refined the looping support
/// 1.1.15.0      1. For looping mode added a folder dialog to set where we
///                  we are going to save files
///              
/// </summary>
namespace FractalFun
{
    public partial class Attractory : Form
    {
        /// <summary>
        /// Various default settings
        /// </summary>
        private double A = 0.0;                 // Parameter A value
        private double B = 0.0;                 // Parameter B value
        private double C = 0.0;                 // Parameter C value
        private double D = 0.0;                 // Parameter D value

        private double AEnd = 0.0;              // End A parameter value
        private double BEnd = 0.0;              // End B parameter value
        private double CEnd = 0.0;              // End C parameter value
        private double DEnd = 0.0;              // End D parameter value

        private double AStep = 0.0;             // A parameter loop step
        private double BStep = 0.0;             // B parameter loop step
        private double CStep = 0.0;             // C parameter loop step
        private double DStep = 0.0;             // D parameter loop step

        private double MinX_ = 0.0;             // Attractor post scale minimum x
        private double MinY_ = 0.0;             // Attractor post scale maximum x
        private double MaxX_ = 0.0;             // Attractor post scale minimum y
        private double MaxY_ = 0.0;             // Attractor post scale maximum y

        private double H = 0.0;                 // Screen height
        private double W = 0.0;                 // Screen width
        private int Scale_ = 300;               // Raw default scaling factor
        private string Name_ = "";              // Optional attractor name
        private bool CanHazResize = false;      // Allow image to be resized
        private bool HitTheBrakes_ = false;     // Stop render if true
        private bool looping_ = false;          // Doing multiple images if true
        private string BasePath = "";           // Location to save files

        /// <summary>
        /// Parameters (see above for the objects encapsulated below)
        /// </summary>
        public double A1 { get => A; set => A = value; }
        public double B1 { get => B; set => B = value; }
        public double C1 { get => C; set => C = value; }
        public double D1 { get => D; set => D = value; }

        public double H1 { get => H; set => H = value; }
        public double W1 { get => W; set => W = value; }

        public double AEnd1 { get => AEnd; set => AEnd = value; }
        public double BEnd1 { get => BEnd; set => BEnd = value; }
        public double CEnd1 { get => CEnd; set => CEnd = value; }
        public double DEnd1 { get => DEnd; set => DEnd = value; }

        public double AStep1 { get => AStep; set => AStep = value; }
        public double BStep1 { get => BStep; set => BStep = value; }
        public double CStep1 { get => CStep; set => CStep = value; }
        public double DStep1 { get => DStep; set => DStep = value; }

        public double MinX { get => MinX_; set => MinX_ = value; }
        public double MinY { get => MinY_; set => MinY_ = value; }
        public double MaxX { get => MaxX_; set => MaxX_ = value; }
        public double MaxY { get => MaxY_; set => MaxY_ = value; }

        public int IScale { get => Scale_; set => Scale_ = value; }
        public string Name1 { get => Name_; set => Name_ = value; }
        public bool ICanHazResize { get => CanHazResize; set => CanHazResize = value; }
        public bool HitTheBrakes { get => HitTheBrakes_; set => HitTheBrakes_ = value; }
        public bool IsLooping { get => looping_; set => looping_ = value; }
        public string BasePath1 { get => BasePath; set => BasePath = value; }

        public List<Attractor> Attractors;

        /// <summary>
        /// Initialize form
        /// </summary>
        public Attractory()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load the form
        /// </summary>
        /// <param name="sender">Form object</param>
        /// <param name="e">Event args</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Load defaults (Kings Dream)
            A1 = -0.966918;
            B1 = 2.879879;
            C1 = 0.765145;
            D1 = 0.744728;

            AEnd1 = 0.0;
            BEnd1 = 0.0;
            CEnd1 = 0.0;
            DEnd1 = 0.0;

            AStep1 = 0.0;
            BStep1 = 0.0;
            CStep1 = 0.0;
            DStep1 = 0.0;

            IsLooping = false;
            IScale = 300;
            Name1 = "";

            LoadPredefines();

            // Trigger the UI init
            timer1.Enabled = true;
            Application.DoEvents();
            ICanHazResize = false;
        }

        /// <summary>
        /// Set or reset program
        /// </summary>
        /// <param name="sender">Timer object</param>
        /// <param name="e">Event args</param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            SetOrResetUI();
        }

        /// <summary>
        /// Reset so we can do a new render
        /// </summary>
        /// <param name="sender">Reset button</param>
        /// <param name="e">Event args</param>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            DoReset();
        }

        /// <summary>
        /// Handle a resize
        /// </summary>
        /// <param name="sender">Picture object</param>
        /// <param name="e">Event args</param>
        private void Display_SizeChanged(object sender, EventArgs e)
        {
            DoResize();
        }

        /// <summary>
        /// Render an image with the current parameters
        /// </summary>
        /// <param name="sender">Render button</param>
        /// <param name="e">Event args</param>
        private void BtnRender_Click(object sender, EventArgs e)
        {
            IsLooping = CanLoop();
            if (IsLooping) { DoLoopRender(A1, B1, C1, D1); }
            else { DoRender(A1, B1, C1, D1); }
        }

        /// <summary>
        /// Set a flag so rendering will stop
        /// </summary>
        /// <param name="sender">Break button</param>
        /// <param name="e">Event args</param>
        private void BtnBreak_Click(object sender, EventArgs e)
        {
            if (!HitTheBrakes) { HitTheBrakes = true; }
        }

        /// <summary>
        /// Save the current image as a PNG file
        /// </summary>
        /// <param name="sender">Save button</param>
        /// <param name="e">Event args</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            DoSaveFile(0, A1, B1, C1, D1);
        }

        /// <summary>
        /// Manage selection of a new predefined attractor
        /// </summary>
        /// <param name="sender">Drop down list</param>
        /// <param name="e">Event args</param>
        private void DroopDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            int NewID = 0;
            NewID = DroopDown.SelectedIndex;
            TxtName.Text = Attractors[NewID].name;
            TxtScale.Text = Attractors[NewID].scale.ToString();
            TxtA.Text = Attractors[NewID].a.ToString();
            TxtB.Text = Attractors[NewID].b.ToString();
            TxtC.Text = Attractors[NewID].c.ToString();
            TxtD.Text = Attractors[NewID].d.ToString();
            Application.DoEvents();
        }

        /// <summary>
        /// Load the predefines editor
        /// </summary>
        /// <param name="sender">Edit button</param>
        /// <param name="e">Event args</param>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            EditPredef EditForm = new EditPredef();
            EditForm.ShowDialog();
            LoadPredefines();
        }

        /// <summary>
        /// Load the predefines file (Json) an load the drop down list
        /// </summary>
        public void LoadPredefines()
        {
            if (Attractors != null) { Attractors.Clear(); }
            // Reload predefined attractors from attached json file to drop-down
            using (StreamReader r = new StreamReader("PredefinedAttractors.json"))
            {
                string json = r.ReadToEnd();
                Attractors = JsonConvert.DeserializeObject<List<Attractor>>(json);
            }

            // Load the drop-down (since data binding a dynamic list is a bit
            // dicey, we will just add the name and id into the drop down list
            DroopDown.Items.Clear();
            foreach (var item in Attractors)
            {
                DroopDown.Items.Add(new { name = item.name, value = Convert.ToString(item.id) });
            }
            DroopDown.DisplayMember = "name";
            DroopDown.ValueMember = "value";
        }

        /// <summary>
        /// Do the initial UI set or reset before staring a new attractor
        /// </summary>
        public void SetOrResetUI()
        {
            // Move the parameter properties to text boxes 
            // (where they can be edited if desired)
            TxtA.Text = A1.ToString();
            TxtB.Text = B1.ToString();
            TxtC.Text = C1.ToString();
            TxtD.Text = D1.ToString();

            TxtAEnd.Text = AEnd1.ToString();
            TxtBEnd.Text = BEnd1.ToString();
            TxtCEnd.Text = CEnd1.ToString();
            TxtDEnd.Text = DEnd1.ToString();

            TxtAStep.Text = AStep1.ToString();
            TxtBStep.Text = BStep1.ToString();
            TxtCStep.Text = CStep1.ToString();
            TxtDStep.Text = DStep1.ToString();
            TxtScale.Text = IScale.ToString();

            TxtName.Text = Name1;

            // Move the height and width of our drawing
            // interface to read only text boxes for reference
            TxtHeight.Text = Display.Height.ToString();
            TxtWidth.Text = Display.Width.ToString();
            H1 = (double)Display.Height;
            W1 = (double)Display.Width;
            MinX = (double)Display.Width;
            MinY = (double)Display.Height;

            // Update the read only min/max values...
            // these will contain real values during render
            MaxX = 0.0;
            MaxY = 0.0;
            TxtMinX.Text = MinX.ToString();
            TxtMaxX.Text = MaxX.ToString();
            TxtMinY.Text = MinY.ToString();
            TxtMaxY.Text = MaxY.ToString();

            // Some more UI housekeeping
            Display.Refresh();
            BtnRender.Enabled = CanRender();
            BtnReset.Enabled = true;
            Application.DoEvents();

            // Allow resize
            ICanHazResize = true;
        }

        /// <summary>
        /// Manage a parameter reset
        /// </summary>
        public void DoReset()
        {
            // Convert the text box values to the parameter and scale
            // properties

            try { A1 = Convert.ToDouble(TxtA.Text.ToString()); }
            catch (Exception ExA)
            {
                MessageBox.Show("Err: " + ExA.Message, "A1", MessageBoxButtons.OK);
                A1 = -0.966918;
            }
            try { B1 = Convert.ToDouble(TxtB.Text.ToString()); }
            catch (Exception ExB)
            {
                MessageBox.Show("Err: " + ExB.Message, "B1", MessageBoxButtons.OK);
                B1 = 2.879879;
            }
            try { C1 = Convert.ToDouble(TxtC.Text.ToString()); }
            catch (Exception ExC)
            {
                MessageBox.Show("Err: " + ExC.Message, "C1", MessageBoxButtons.OK);
                C1 = 0.765145;
            }
            try { D1 = Convert.ToDouble(TxtD.Text.ToString()); }
            catch (Exception ExD)
            {
                MessageBox.Show("Err: " + ExD.Message, "D1", MessageBoxButtons.OK);
                D1 = 0.744728;
            }

            try { IScale = Convert.ToInt32(TxtScale.Text.ToString()); }
            catch (Exception ExIScale)
            {
                MessageBox.Show("Err: " + ExIScale.Message, "IScale", MessageBoxButtons.OK);
                IScale = 300;
            }
            Name1 = TxtName.Text;

            try { AEnd1 = Convert.ToDouble(TxtAEnd.Text.ToString()); }
            catch (Exception ExAEnd)
            {
                MessageBox.Show("Err: " + ExAEnd.Message, "AEnd1", MessageBoxButtons.OK);
                AEnd1 = 0.0;
            }
            try { BEnd1 = Convert.ToDouble(TxtBEnd.Text.ToString()); }
            catch (Exception ExBEnd)
            {
                MessageBox.Show("Err: " + ExBEnd.Message, "BEnd1", MessageBoxButtons.OK);
                BEnd1 = 0.0;
            }
            try { CEnd1 = Convert.ToDouble(TxtCEnd.Text.ToString()); }
            catch (Exception ExCEnd)
            {
                MessageBox.Show("Err: " + ExCEnd.Message, "CEnd1", MessageBoxButtons.OK);
                CEnd1 = 0.0;
            }
            try { DEnd1 = Convert.ToDouble(TxtDEnd.Text.ToString()); }
            catch (Exception ExDEnd)
            {
                MessageBox.Show("Err: " + ExDEnd.Message, "DEnd1", MessageBoxButtons.OK);
                DEnd1 = 0.0;
            }

            try { AStep1 = Convert.ToDouble(TxtAStep.Text.ToString()); }
            catch (Exception ExAStep)
            {
                MessageBox.Show("Err: " + ExAStep.Message, "AStep1", MessageBoxButtons.OK);
                AStep1 = 0.0;
            }
            try { BStep1 = Convert.ToDouble(TxtBStep.Text.ToString()); }
            catch (Exception ExBStep)
            {
                MessageBox.Show("Err: " + ExBStep.Message, "BStep1", MessageBoxButtons.OK);
                BStep1 = 0.0;
            }
            try { CStep1 = Convert.ToDouble(TxtCStep.Text.ToString()); }
            catch (Exception ExCStep)
            {
                MessageBox.Show("Err: " + ExCStep.Message, "CStep1", MessageBoxButtons.OK);
                CStep1 = 0.0;
            }
            try { DStep1 = Convert.ToDouble(TxtDStep.Text.ToString()); }
            catch (Exception ExDStep)
            {
                MessageBox.Show("Err: " + ExDStep.Message, "DStep1", MessageBoxButtons.OK);
                DStep1 = 0.0;
            }

            // Some UI housekeeping
            btnSave.Enabled = false;
            HitTheBrakes = false;
            TxtIteration.Text = "0";
            timer1.Enabled = true;
            Application.DoEvents();
        }

        /// <summary>
        /// Manage a form resize if we aren't otherwise occupied
        /// </summary>
        public void DoResize()
        {
            if (ICanHazResize)
            {
                TxtHeight.Text = Display.Height.ToString();
                TxtWidth.Text = Display.Width.ToString();
                H1 = (double)Display.Height;
                W1 = (double)Display.Width;
                MinX = (double)Display.Width;
                MinY = (double)Display.Height;
                MaxX = 0.0;
                MaxY = 0.0;
                TxtMinX.Text = MinX.ToString();
                TxtMaxX.Text = MaxX.ToString();
                TxtMinY.Text = MinY.ToString();
                TxtMaxY.Text = MaxY.ToString();

                Application.DoEvents();
            }
        }

        /// <summary>
        /// If we have at least A1 and B1 return true
        /// </summary>
        /// <returns>bool indicating if rendering can be done</returns>
        public bool CanRender()
        {
            if (A1 != 0.0 && B1 != 0.0)
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// See if one of the parameters is set for looping
        /// </summary>
        /// <returns>true if looping is set</returns>
        public bool CanLoop()
        {
            if ((AEnd1 != 0.0 && AStep1 != 0.0) ||
                (BEnd1 != 0.0 && BStep1 != 0.0) ||
                (CEnd1 != 0.0 && CStep1 != 0.0) ||
                (DEnd1 != 0.0 && DStep1 != 0.0))
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Get the loop start parameter value. Since we can only loop on
        /// one of the four parameters, use the first one we find
        /// </summary>
        /// <returns>parameter value</returns>
        public double GetBegin()
        {
            // Only one End and Step can be declared
            if (AEnd1 != 0.0 && AStep1 != 0.0) { return A1; }
            if (BEnd1 != 0.0 && BStep1 != 0.0) { return B1; }
            if (CEnd1 != 0.0 && CStep1 != 0.0) { return C1; }
            if (DEnd1 != 0.0 && DStep1 != 0.0) { return D1; }
            return 0.0;
        }

        /// <summary>
        /// Get the loop end parameter value
        /// </summary>
        /// <returns>parameter value</returns>
        public double GetEnd()
        {
            // Only one End and Step can be declared
            if (AEnd1 != 0.0) { return AEnd1; }
            if (BEnd1 != 0.0) { return BEnd1; }
            if (CEnd1 != 0.0) { return CEnd1; }
            if (DEnd1 != 0.0) { return DEnd1; }
            return 0.0;
        }

        /// <summary>
        /// Get the loop step value
        /// </summary>
        /// <returns>parameter value</returns>
        public double GetStep()
        {
            // Only one End and Step can be declared
            if (AStep1 != 0.0) { return AStep1; }
            if (BStep1 != 0.0) { return BStep1; }
            if (CStep1 != 0.0) { return CStep1; }
            if (DStep1 != 0.0) { return DStep1; }
            return 0.0;
        }

        /// <summary>
        /// Get the index so we know if we are looping on A, B, C or D
        /// </summary>
        /// <returns>1 through 4 (for A through D)</returns>
        public int GetParamIndex()
        {
            if (AStep1 != 0.0) { return 1; }
            if (BStep1 != 0.0) { return 2; }
            if (CStep1 != 0.0) { return 3; }
            if (DStep1 != 0.0) { return 4; }
            return 0;
        }

        /// <summary>
        /// Loop through all renders
        /// </summary>
        /// <param name="A">A1</param>
        /// <param name="B">B1</param>
        /// <param name="C">C1</param>
        /// <param name="D">D1</param>
        public void DoLoopRender(double A, double B, double C, double D)
        {
            int IFrame = 0;
            double Begin = GetBegin();
            double End = GetEnd();
            double Step = GetStep();

            SetSaveFolder.Description = "Set file save path";
            if (SetSaveFolder.ShowDialog() == DialogResult.OK)
            {
                BasePath1 = SetSaveFolder.SelectedPath;
            }
            int PIndex = GetParamIndex();

            if (Step > 0.0)
            {
                for (double Frame = Begin; Frame <= End; Frame = Frame + Step)
                {
                    IFrame++;
                    TxtFrame.Text = IFrame.ToString() + " (" + Frame.ToString() + ")";

                    switch (PIndex)
                    {
                        case 1: // Loop on A
                            DoRender(Frame, B, C, D);
                            DoSaveFile(IFrame, Frame, B, C, D);
                            break;
                        case 2: // Loop on B
                            DoRender(A, Frame, C, D);
                            DoSaveFile(IFrame, A, Frame, C, D);
                            break;
                        case 3: // Loop on C
                            DoRender(A, B, Frame, D);
                            DoSaveFile(IFrame, A, B, Frame, D);
                            break;
                        case 4: // Loop on D
                            DoRender(A, B, C, Frame);
                            DoSaveFile(IFrame, A, B, C, Frame);
                            break;
                        default:
                            break;
                    }
                    DoReset();
                    if (HitTheBrakes) { break; }
                }
            }
            else
            {
                for (double Frame = Begin; Frame >= End; Frame = Frame - Step)
                {
                    IFrame++;
                    TxtFrame.Text = IFrame.ToString() + " (" + Frame.ToString() + ")";

                    switch (PIndex)
                    {
                        case 1: // Loop on A
                            DoRender(Frame, B, C, D);
                            DoSaveFile(IFrame, Frame, B, C, D);
                            break;
                        case 2: // Loop on B
                            DoRender(A, Frame, C, D);
                            DoSaveFile(IFrame, A, Frame, C, D);
                            break;
                        case 3: // Loop on C
                            DoRender(A, B, Frame, D);
                            DoSaveFile(IFrame, A, B, Frame, D);
                            break;
                        case 4: // Loop on D
                            DoRender(A, B, C, Frame);
                            DoSaveFile(IFrame, A, B, C, Frame);
                            break;
                        default:
                            break;
                    }
                    DoReset();
                    if (HitTheBrakes) { break; }
                }
            }
            HitTheBrakes = false;
            Application.DoEvents();
        }

        /// <summary>
        /// Render an image with the current parameters, if looping one
        /// of the parameters will be the current loop value
        /// </summary>
        /// <param name="A">A1</param>
        /// <param name="B">B1</param>
        /// <param name="C">C1</param>
        /// <param name="D">D1</param>
        public void DoRender(double A, double B, double C, double D)
        {
            if (CanRender())
            {
                ICanHazResize = false;
                BtnRender.Enabled = false;
                BtnBreak.Enabled = true;
                btnSave.Enabled = false;

                // Create a 32 bit ARGB bitmap to draw on
                Bitmap Paper = MakePaper();

                // paint it white...
                for (int ix = 0; ix < (int)W1; ix++)
                {
                    for (int iy = 0; iy < (int)H1; iy++)
                    {
                        Paper.SetPixel(ix, iy, Color.White);
                    }
                }

                // Update the UI image with the clean sheet of paper
                Display.Image = Paper;
                Application.DoEvents();

                // Init a few working variables
                double x = 0.1;
                double y = 0.1;
                double xn;
                double yn;
                double xs;
                double ys;
                double i = 0.0;

                for (Int64 n = 1; n <= 10000000; n++)
                {
                    i = n;

                    // Calculate new XY coordinates
                    xn = Math.Sin(y * B) + C * Math.Sin(x * B);
                    yn = Math.Sin(x * A) + D * Math.Sin(y * A);

                    // Update the coordinates
                    x = xn;
                    y = yn;

                    // Scale the coordinates (scale then center)
                    xs = (x * IScale) + (W1 / 2.0);
                    ys = (y * IScale) + (H1 / 2.0);

                    // Collect scaled plot min/max values (assists if we need to rescale)
                    if (xs < MinX) { MinX = xs; }
                    if (xs > MaxX) { MaxX = xs; }
                    if (ys < MinY) { MinY = ys; }
                    if (ys > MaxY) { MaxY = ys; }

                    // If the coordinate is in bounds, plot it
                    if (xs >= 0 && xs <= W1 && ys >= 0 && ys <= H1)
                    {
                        Paper.SetPixel((int)xs, (int)ys, GetColor(xs, ys, Paper));
                        // Every 1000 iterations, update the UI
                        if ((int)n % 1000 == 0)
                        {
                            // Update the UI
                            TxtIteration.Text = i.ToString();
                            Display.Image = Paper;
                            TxtMinX.Text = MinX.ToString();
                            TxtMaxX.Text = MaxX.ToString();
                            TxtMinY.Text = MinY.ToString();
                            TxtMaxY.Text = MaxY.ToString();
                            Application.DoEvents();
                        }
                    }
                    // See if we should stop the render early
                    if (HitTheBrakes) { break; }
                }
                // Final post render UI update
                TxtIteration.Text = i.ToString();
                Display.Image = Paper;
                TxtMinX.Text = MinX.ToString();
                TxtMaxX.Text = MaxX.ToString();
                TxtMinY.Text = MinY.ToString();
                TxtMaxY.Text = MaxY.ToString();
                BtnReset.Enabled = true;
                BtnBreak.Enabled = false;
                btnSave.Enabled = true;
                Application.DoEvents();
                ICanHazResize = true;
            }
            if (!IsLooping) { HitTheBrakes = false; }
        }

        /// <summary>
        /// Create a bitmap for us to draw on
        /// </summary>
        /// <returns>An image bitmap</returns>
        public Bitmap MakePaper()
        {
            return new Bitmap((int)W1, (int)H1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Set first time pixels to light gray and then darken them each
        /// time they are revisited until they are black
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="image">Bitmap image</param>
        /// <returns>An ARGB color</returns>
        public Color GetColor(double x, double y, Bitmap image)
        {
            // Set high and low range for a pixel
            Color Lo = Color.Black;
            Color Hi = Color.FromArgb(255, (int)0xE8, (int)0xE8, (int)0xE8); // Lighter gray

            // Grab the color of the current pixel
            Color got = image.GetPixel((int)x, (int)y);

            // Break out the current pixel color info
            byte r = got.R;
            byte g = got.G;
            byte b = got.B;
            byte a = got.A;

            // If we are brighter than black, darken
            if (r > Lo.R) { r--; }
            if (g > Lo.G) { g--; }
            if (b > Lo.B) { b--; }

            // If we are brighter than lighter gray, bump to lighter gray
            if (r > Hi.R) { r = Hi.R; }
            if (g > Hi.G) { g = Hi.G; }
            if (b > Hi.B) { b = Hi.B; }

            // Clamp at black
            if (r <= Lo.R) { r = Lo.R; }
            if (g <= Lo.G) { g = Lo.G; }
            if (b <= Lo.B) { b = Lo.B; }

            // Reassemble and return the new color
            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Using the save file dialog, save the current image as a PNG file
        /// </summary>
        /// <param name="Frame">Current Loop value if looping</param>
        /// <param name="A">A1</param>
        /// <param name="B">B1</param>
        /// <param name="C">C1</param>
        /// <param name="D">D1</param>
        public void DoSaveFile(int Frame, double A, double B, double C, double D)
        {
            string SaveFile = "";
            DateTime NowLocal = DateTime.Now;
            string Frac2 = FormatDT(NowLocal);

            if (!IsLooping)
            {
                SaveFractal.FileName = "Attractor " + TxtName.Text + " " + Frac2 + ".png";
                SaveFractal.Title = "Save image to file";
                if (SaveFractal.ShowDialog() == DialogResult.OK)
                {
                    SaveFile = SaveFractal.FileName;
                }
            }
            else
            {
                //string Path = System.IO.Path.GetDirectoryName(SaveFractal.FileName);
                SaveFile = BasePath1 + "\\" + "Attractor " + TxtName.Text + "-F" + Frame.ToString() + " " + Frac2 + ".png";
            }
            Display.Image.Save(SaveFile, System.Drawing.Imaging.ImageFormat.Png);
            SaveLog(SaveFile, A, B, C, D);
        }

        /// <summary>
        /// Given the local date and time format so it's suitable for a filename
        /// </summary>
        /// <param name="DT">Local date and time as DateTime</param>
        /// <returns>string dd-mm-yyyy_hh-mm-ss</returns>
        public string FormatDT(DateTime DT)
        {
            string Ret = DT.Month.ToString() + "-";
            Ret += DT.Day.ToString() + "-";
            Ret += DT.Year.ToString() + "_";
            Ret += DT.Hour.ToString() + "-";
            Ret += DT.Minute.ToString() + "-";
            Ret += DT.Second.ToString();
            return Ret;
        }

        /// <summary>
        /// Extract the path from the image save path and append image info
        /// in the file "LogFile.txt" in unquoted CSV format
        /// </summary>
        /// <param name="LogFile">Contains the path where the log will be</param>
        /// <param name="A">A1</param>
        /// <param name="B">B1</param>
        /// <param name="C">C1</param>
        /// <param name="D">D1</param>
        public void SaveLog(string LogFile, double A, double B, double C, double D)
        {
            string LogName = "";
            string LogLine = "";

            // Scan backwards for the end of the path
            for (int ix = LogFile.Length - 1; ix > 0; ix--)
            {
                // This the end of the path?
                if (LogFile.Substring(ix, 1) == "\\")
                {
                    // Yes, append the file name and break out of the loop
                    LogName = LogFile.Substring(0, ix) + "\\LogFile.txt";
                    break;
                }
            }
            // Open an append file stream
            StreamWriter F = new StreamWriter(LogName, true);

            // Build output record and write to file
            LogLine = A.ToString() + "," +
                      B.ToString() + "," +
                      C.ToString() + "," +
                      D.ToString() + "," +
                      IScale.ToString() + "," +
                      Name1 + "," + LogFile;
            F.WriteLine(LogLine);
            F.Flush();
            F.Close();
            //F.Dispose();
        }
    }

    /// <summary>
    /// Defines a given attaractor, and the information to create and 
    /// scale it properly. The data from the file PredefinedAttractors.json
    /// is loaded into this class (expressed as a list object)
    /// </summary>
    public class Attractor
    {
        public int id;
        public string name;
        public double lyap;
        public double a;
        public double b;
        public double c;
        public double d;
        public string mut;
        public int scale;
    }
}
