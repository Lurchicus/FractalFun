using Newtonsoft.Json;
using System;
using System.Media;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

/// <summary>
/// FractalFun.Attractory
/// 
/// Creates Pickford attractors based on the code described in the book 
/// "Chaos In Wonderland" by Clifford A. Pickover, 1994, pg 267 M.2,
/// ISBN 0-312-10743-9 as well as adhoc or original predefined attractors
/// 
/// Fractal Fun (Attractor Exploder)
/// Copyright(C) 2019 by Dan Rhea
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
/// GNU General Public License for more details.
///
/// You should have received a copy of the GNU General Public License
/// along with this program. If not, see<https://www.gnu.org/licenses/>.
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
/// 1.1.15.0      For looping mode added a folder dialog to set where we
///               we are going to save files
/// 4/15/2019 DWR Expanded the comments in much of the program 
/// 1.1.17.0
/// 4/17/2019 DWR Allow looping without a file save in the path is null 
/// 1.1.18.0      (path dialog canceled) and added more comments where
///               I felt it was appropriate. Added a todo/bug list
/// 4/20/2019 DWR Locked buttons to the bottom of the form and added a 
/// 1.1.19.0      checkbox to set a break all renders (with the default
///               being break current render) when the "Break" button is
///               checked. Added a property to follow the break mode.
///               - Resolves issue 1
/// 4/20/2019 DWR I added an event handler to all of the editable text
/// 1.1.20.0      boxes. It triggers on Focus leaving the control so 
///               for the moment, no help screen needed for the "Reset"
///               button now. The readonly text boxes can be safely
///               ignored.
///               - Resolves issue 2
///               I also added code to validate the textbox contents... 
///               using CheckD (double check) and CheckI (integer check)
/// 4/20/2019 DWR Issue 3, added the GNU3 license text file to the 
/// 1.1.21.0      project. Getting started on a new form to display the
///               license text in a modal dialog. Finished up the file
///               viewer form and text load using its constructor.
///               Moved the Edit button to the bottom of the UI and 
///               added the "License" button to the lower part of the
///               UI and added a click event handler to show the file
///               viewer form as a modal dialog.
///               - Resolves issue 3
/// 4/30/2019 DWR - After I did some research I discovered that the 
/// 1.1.22.0      attractors this visualizer creates are all Pickover
///               attractors so I'm changing "strange attractor" 
///               references to "Pickover attractors". Credit where
///               credit is due! "Pickover! I read your book!" :)
///               - I darkened the settings panel and I'm working on a
///               dark mode that will draw the attractors on a black
///               background with dark gray through white pixels.
///               - Dark mode is now implemented, just like light mode,
///               only darker
///               - I'm also doing some experimentation with converting
///               our ARGB colors to HSV and looping through the hues
///               rather than the current gray scale each time I hit a
///               lit pixel. Eventually I will expand out to more 
///               visualizers for other attractors and introduce a few
///               - alternate Pickover attractor variants added with a 
///               drop down list to select them
///               
///                 xnew=cos(y*b)+c*sin(x*b)
///                 ynew=cos(x*a)+d*sin(y*a)
/// 
///               - Also working on adding automated unit tests to the
///               project... not that a small project like this really
///               needs this level of testing, but I need to master this
///               for my real job (#DevOps)
/// 5/09/2019 DWR - Fixed a bug that left us in dark mode no matter what
/// 1.1.23.0      the checkbox was set to.
/// 11/24/2020 DWR - Brightened up the UI a bit
/// 1.1.24.0       - Did some clean-up and optimizations.
///                - Set better colors for the editor and file viewer
///                - Added version to text log
///                - Got rid of minor "issue" warnings
/// 02/04/2021 DWR - Added code that lets me pan the image some. This 
/// 1.1.25.0       needs a lot more work since viewporting does not seem
///                to be a thing for standard .NET WinForms. Looking for
///                an open source graphics package that supports it. I 
///                know Leadtools can do this sort of thing but I'm 
///                looking for an open source solution.
///                - For now fixed the image size to 1050x1680. This makes
///                it larger than the panel it's housed in. I will add a
///                JSON file with various resolutions that can be picked
///                from a drop-down (not done yet).
///                - Added "StampMode" that when true lets me put some of
///                the information about the attractor in the upper left
///                corner of the image. Added a checkbox to control the mode.
///                - Changed the renderer to update the screen from every
///                1000 iterations to every 10,000. The display is a tiny
///                bit choppier, but way faster. I tried 100,000 but it
///                didn't speed up much and made the image look like a
///                slideshow.
///                - Redid the "splash" text displayed in the log text 
///                box to include official license wording (Gnu GPL3).
///                - For the log text box added batch render start and
///                stop times as well as the parameters for each render.
///                - Added file information to thelog text box. It's not
///                pretty, but it's useful enough to include anyway.
/// 02/05/2020 DWR - The almost required even release number to go fix
/// 1.1.26.0       comments, formatting and other "papercut" type items
///                introduced in the previous release.
///                - Flipped Height and Width UI elements so they match 
///                the order of MinX, MaxX, MinY and MaxY (much less
///                confusing this way).
///               - Defaulted "Break stops all renders" to true. 
/// 04/16/2021 DWR - Corrected references to Clifford Pickover's name...
/// 1.1.27.0      I read his book and still got his last name wrong. It's
///               not like I follow him on Twitter and Facebook either (I
///               do follow him on both of course). 
///               I Feel pretty silly about it.
/// 04/17/2021 DWR - I reworked the loop mode so it can work on (WIP)
/// 1.1.28.0      multiple ranges and not stop until all loops hit end.
///               This actually greatly simplified the batch loop code.
/// 04/17/2021 DWR - Added a "current" column for A through D on the UI
/// 1.1.29.0      So it's easier to track looping progress. Tweaked the
///               code and comments a bit.
/// 
/// Open/ToDo issues:
///               - I'm also doing some experimentation with converting
///               our ARGB colors to HSV and looping through the hues
///               rather than the current gray scale each time I hit a
///               lit pixel. Eventually I will expand out to more 
///               visualizers for other attractors and introduce a few
///               predefined gradients. Hacking away at this.
///               - Thinking about adding a checkbox to hide (minimize)
///               the left panel during render to maximize the visible
///               area for the image panel.
///               - There is an issue with initializing the "paper". It
///               works correctly when creating multiple renders but not
///               for single renders. I'm doing something in one path 
///               that I'm not doing in the other.
///               - I need to rework the loop mode so it can work on
///               multiple ranges and not stop until all loops hit end.
///               There's no reason this shouldn't work. Focus initially
///               on the switch statements to remove the limitations.
/// 
/// Resolved issues:
/// 1. Determine if a break during looping kills the whole
///    series or just the current render. Currently it just
///    affects the current render. Maybe a checkbox to set
///    the break option? TBD
/// 2. There is a need for a help screen... for example, we
///    need to inform the user that after any UI changes, 
///    they should hit reset... on the other hand, if I quit
///    being lazy and add an event handler for all the 
///    changeable UI elements, I could handle this internally.
/// 3. Need to add the GNU license and a simple viewer so the
///    user can read it if they want (form with a read only
///    text box on it).
/// 4. WIP: I need to rework the loop mode so it can work on
///    multiple ranges and not stop until all loops hit end.
///    There's no reason this shouldn't work. Focus initially
///    on the switch statements to remove the limitations.
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

        private bool ALooping = false;          // A is looping
        private bool BLooping = false;          // B is looping
        private bool CLooping = false;          // C is looping
        private bool DLooping = false;          // D is looping

        private double ACurr = 0.0;             // A loop value
        private double BCurr = 0.0;             // B loop value
        private double CCurr = 0.0;             // C loop value
        private double DCurr = 0.0;             // D loop value

        private bool ADone = false;             // A loop done flag
        private bool BDone = false;             // B loop done flag
        private bool CDone = false;             // C loop done flag
        private bool DDone = false;             // D loop done flag

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
        private bool BreakAll_ = true;          // If true, the break button stops all renders
        private bool DarkMode_ = false;         // If true, plot white pixels on black
        private bool StampMode_ = false;        // If true, stamp rendered image with attractor info
        private int Mode_ = 0;                  // Attractor mode

        private string PredefinesFile = "PredefinedAttractors.json";    // Name of the predefines file
        private string ViewFilename_ = "gnu_gpl3.txt";                   // GNU3 license

        private Point MouseDownLocation;
        private bool WhatADrag;
        private Image SnapShot;
        private Rectangle Frame;

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

        public bool ALooping1 { get => ALooping; set => ALooping = value; }
        public bool BLooping1 { get => BLooping; set => BLooping = value; }
        public bool CLooping1 { get => CLooping; set => CLooping = value; }
        public bool DLooping1 { get => DLooping; set => DLooping = value; }

        public double ACurr1 { get => ACurr; set => ACurr = value; }
        public double BCurr1 { get => BCurr; set => BCurr = value; }
        public double CCurr1 { get => CCurr; set => CCurr = value; }
        public double DCurr1 { get => DCurr; set => DCurr = value; }

        public bool ADone1 { get => ADone; set => ADone = value; }
        public bool BDone1 { get => BDone; set => BDone = value; }
        public bool CDone1 { get => CDone; set => CDone = value; }
        public bool DDone1 { get => DDone; set => DDone = value; }

        public int IScale { get => Scale_; set => Scale_ = value; }
        public string Name1 { get => Name_; set => Name_ = value; }
        public bool ICanHazResize { get => CanHazResize; set => CanHazResize = value; }
        public bool HitTheBrakes { get => HitTheBrakes_; set => HitTheBrakes_ = value; }
        public bool IsLooping { get => looping_; set => looping_ = value; }
        public string BasePath1 { get => BasePath; set => BasePath = value; }
        public string PredefinesFile1 { get => PredefinesFile; set => PredefinesFile = value; }
        public bool BreakAll { get => BreakAll_; set => BreakAll_ = value; }
        public bool DarkMode { get => DarkMode_; set => DarkMode_ = value; }
        public bool StampMode { get => StampMode_; set => StampMode_ = value; }
        public int Mode { get => Mode_; set => Mode_ = value; }
        public string ViewFilename { get => ViewFilename_; set => ViewFilename_ = value; }
        public string Ver { get; set; }

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

            ACurr1 = A1;
            BCurr1 = B1;
            CCurr1 = C1;
            DCurr1 = D1;

            IsLooping = false;
            IScale = 300;
            Name1 = "";

            BreakAll = false;
            CBXBreakMode.Checked = BreakAll;    // Defaults to off

            LoadPredefines();

            // Load attractor type drop down
            DroopMode.Items.Insert(0, "Pickover attractor");
            DroopMode.Items.Insert(1, "Pickover attractor (sin,cos)");
            DroopMode.Items.Insert(2, "Pickover attractor (cos)");
            DroopMode.SelectedItem = 0;
            Mode = 0;

            //
            string nl = Environment.NewLine;

            // GPL stuff
            Ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            TxtLog.AppendText("Fractal Fun (Attractor Exploder) v" + Ver + nl);
            TxtLog.AppendText("Copyright 2019, 2020, 2021 Dan Rhea" + nl + nl);
            TxtLog.AppendText("This program is free software: you can redistribute it and/or " + 
                "modify it under the terms of the GNU General Public License " + 
                "as published by the Free Software Foundation, either version " + 
                "3 of the License, or (at your option) any later version." + nl + nl);
            TxtLog.AppendText("This program is distributed in the hope that it will be " + 
                "useful, but WITHOUT ANY WARRANTY; without even the " + 
                "implied warranty of MERCHANTABILITY or FITNESS FOR " + 
                "A PARTICULAR PURPOSE. See the GNU General Public License for more details." + nl + nl);
            TxtLog.AppendText("You should have received a copy of the GNU General Public " + 
                "License along with this program. Click the license button " + 
                "for the license." + nl + nl);

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
            Display.Refresh();
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
        private void BtnSave_Click(object sender, EventArgs e)
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
            int NewID;
            NewID = DroopDown.SelectedIndex;
            TxtName.Text = Attractors[NewID].name;
            TxtScale.Text = Attractors[NewID].scale.ToString();
            TxtA.Text = Attractors[NewID].a.ToString();
            TxtB.Text = Attractors[NewID].b.ToString();
            TxtC.Text = Attractors[NewID].c.ToString();
            TxtD.Text = Attractors[NewID].d.ToString();
            // Also clear any range leftovers that may remain
            AEnd1 = 0.0; TxtAEnd.Text = AEnd1.ToString();
            BEnd1 = 0.0; TxtBEnd.Text = BEnd1.ToString();
            CEnd1 = 0.0; TxtCEnd.Text = CEnd1.ToString();
            DEnd1 = 0.0; TxtDEnd.Text = DEnd1.ToString();
            AStep1 = 0.0; TxtAStep.Text = AStep1.ToString();
            BStep1 = 0.0; TxtBStep.Text = BStep1.ToString();
            CStep1 = 0.0; TxtCStep.Text = CStep1.ToString();
            DStep1 = 0.0; TxtDStep.Text = DStep1.ToString();
            // Allow the UI to update
            DoReset();
            Application.DoEvents();
        }

        private void DroopMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Mode = DroopMode.SelectedIndex;
            // Allow the UI to update
            DoReset();
            Application.DoEvents();
        }

        /// <summary>
        /// Load the predefines editor
        /// </summary>
        /// <param name="sender">Edit button</param>
        /// <param name="e">Event args</param>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            EditPredef EditForm = new EditPredef(PredefinesFile1);
            EditForm.ShowDialog();
            LoadPredefines();
        }

        /// <summary>
        /// Load the file viewer
        /// </summary>
        /// <param name="sender">License button</param>
        /// <param name="e">Event args</param>
        private void BtnLicense_Click(object sender, EventArgs e)
        {
            FileViewer ViewForm = new FileViewer(ViewFilename);
            ViewForm.ShowDialog();
        }

        /// <summary>
        /// Handle the break mode checkbox
        /// </summary>
        /// <param name="sender">Break mode checkbox</param>
        /// <param name="e">Event args</param>
        private void CBXBreakMode_CheckedChanged(object sender, EventArgs e)
        {
            if (CBXBreakMode.Checked)
            {
                BreakAll = true;
            }
            else
            {
                BreakAll = false;
            }
        }

        /// <summary>
        /// Dark mode checkbox change handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBXDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (CBXDarkMode.Checked)
            {
                DarkMode = true;
            }
            else
            {
                DarkMode = false;
            }

        }

        /// <summary>
        /// Stamp Mode checkbox change handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBXStampMode_CheckedChanged(object sender, EventArgs e)
        {
            if (CBXStampMode.Checked)
            {
                StampMode = true;
            }
            else
            {
                StampMode = false;
            }
        }

        /// <summary>
        /// Load the predefines file (Json) and load the drop down list
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
                DroopDown.Items.Add(item: new { item.name, value = Convert.ToString(item.id) });
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
            TxtHeight.Text = "1050"; // Display.Height.ToString();
            TxtWidth.Text = "1680"; // Display.Width.ToString();
            H1 = 1050; // (double)Display.Height;
            W1 = 1680; // (double)Display.Width;
            MinY = 1050; // (double)Display.Height;
            MinX = 1680; // (double)Display.Width;

            // Update the read only min/max values...
            // these will contain real values during render
            MaxX = 0.0;
            MaxY = 0.0;
            TxtMinX.Text = MinX.ToString();
            TxtMaxX.Text = MaxX.ToString();
            TxtMinY.Text = MinY.ToString();
            TxtMaxY.Text = MaxY.ToString();
            //TxtFrame.Text = "";

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
            if (!BreakAll) HitTheBrakes = false;
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
                TxtHeight.Text = "1050"; // Display.Height.ToString();
                TxtWidth.Text = "1680"; // Display.Width.ToString();
                H1 = 1680; //(double)Display.Height;
                W1 = 1050; //(double)Display.Width;
                MinX = 1680;  //(double)Display.Width;
                MinY = 1050;  //(double)Display.Height;
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
            return (A1 != 0.0 && B1 != 0.0);
        }

        /// <summary>
        /// See if one of the parameters is set for looping
        /// </summary>
        /// <returns>true if looping is set</returns>
        public bool CanLoop()
        {
            if (AEnd1 != 0.0 && AStep1 != 0.0) { ALooping1 = true; ACurr = A; }
            if (BEnd1 != 0.0 && BStep1 != 0.0) { BLooping1 = true; BCurr = B; }
            if (CEnd1 != 0.0 && CStep1 != 0.0) { CLooping1 = true; CCurr = C; }
            if (DEnd1 != 0.0 && DStep1 != 0.0) { DLooping1 = true; DCurr = D; }

            return (ALooping1 || BLooping1 || CLooping1 || DLooping1);     
        }

        /// <summary>
        /// See if there are any parameter loops to do
        /// </summary>
        public void SetLooping()
        {
            if (AEnd != 0.0 && AStep != 0.0) { ALooping = true; ADone = false; }
            if (BEnd != 0.0 && BStep != 0.0) { BLooping = true; BDone = false; }
            if (CEnd != 0.0 && CStep != 0.0) { CLooping = true; CDone = false; }
            if (DEnd != 0.0 && DStep != 0.0) { DLooping = true; DDone = false; }
        }

        /// <summary>
        /// Increment looping parameters if looping and not done
        /// </summary>
        public void DoLoops()
        {
            if (ALooping && !ADone) { ACurr += AStep; }
            if (BLooping && !BDone) { BCurr += BStep; }
            if (CLooping && !CDone) { CCurr += CStep; }
            if (DLooping && !DDone) { DCurr += DStep; }
            TxtACurr.Text = (ALooping) ? ACurr.ToString() : A1.ToString();
            TxtBCurr.Text = (ALooping) ? BCurr.ToString() : B1.ToString();
            TxtCCurr.Text = (ALooping) ? CCurr.ToString() : C1.ToString();
            TxtDCurr.Text = (ALooping) ? DCurr.ToString() : D1.ToString();
            Application.DoEvents();
        }

        /// <summary>
        /// Determine if looping parameters are done or not
        /// </summary>
        /// <returns>bool: True if ALL looping parameters are done looping</returns>
        public bool AllDone()
        {
            int LoopCount = 0;
            int DoneCount = 0;
            if (ALooping) {
                LoopCount++;
                if (AStep > 0)
                {
                    if (ACurr >= AEnd) { ADone = true; DoneCount++; }
                }
                else
                {
                    if (ACurr <= AEnd) { ADone = true; DoneCount++; }
                }
            }
            if (BLooping)
            {
                LoopCount++;
                if (BStep > 0)
                {
                    if (BCurr >= BEnd) { BDone = true; DoneCount++; }
                }
                else
                {
                    if (BCurr <= BEnd) { BDone = true; DoneCount++; }
                }
            }
            if (CLooping)
            {
                LoopCount++;
                if (CStep > 0)
                {
                    if (CCurr >= CEnd) { CDone = true; DoneCount++; }
                }
                else
                {
                    if (CCurr <= CEnd) { CDone = true; DoneCount++; }
                }
            }
            if (DLooping)
            {
                LoopCount++;
                if (DStep > 0)
                {
                    if (DCurr >= DEnd) { DDone = true; DoneCount++; }
                }
                else
                {
                    if (DCurr <= DEnd) { DDone = true; DoneCount++; }
                }
            }
            return (LoopCount == DoneCount);
        }

        /// <summary>
        /// Loop through all renders... this actually means iterate one of
        /// the four parameters through a UI defined range, incrementing (or 
        /// decrementing) by the UI supplied step value. If more than one
        /// range is supplied, we will use the first range found and ignore 
        /// any others (really should have called this DoRangeRender)
        /// </summary>
        /// <param name="A">A1</param>
        /// <param name="B">B1</param>
        /// <param name="C">C1</param>
        /// <param name="D">D1</param>
        public void DoLoopRender(double A, double B, double C, double D)
        {
            //int IFrame = 0;
            //double Begin = GetBegin();
            //double End = GetEnd();
            //double Step = GetStep();

            String Trace = "Batch render start: " + DateTime.Now.ToString() + Environment.NewLine;
            TxtLog.AppendText(Trace);

            // Find out where we want to save image files
            SetSaveFolder.Description = "Set file save path";
            if (SetSaveFolder.ShowDialog() == DialogResult.OK)
            {
                BasePath1 = SetSaveFolder.SelectedPath;
            }
            else
            {
                // Skip logging and file save
                BasePath1 = null;
            }

            SetLooping();
            IsLooping = CanLoop();
            int Frame = 1;

            // Show the initial current values
            TxtACurr.Text = (ALooping) ? ACurr.ToString() : A1.ToString();
            TxtBCurr.Text = (BLooping) ? BCurr.ToString() : B1.ToString();
            TxtCCurr.Text = (CLooping) ? CCurr.ToString() : C1.ToString();
            TxtDCurr.Text = (DLooping) ? DCurr.ToString() : D1.ToString();
            Application.DoEvents();

            while (!AllDone() && IsLooping)
            {
                TxtFrame.Text = Frame.ToString();
                if (ALooping) { A = ACurr; }
                if (BLooping) { B = BCurr; }
                if (CLooping) { C = CCurr; }
                if (DLooping) { D = DCurr; }

                DoRender((ALooping) ? ACurr : A, (BLooping) ? BCurr : B,
                         (CLooping) ? CCurr : C, (DLooping) ? DCurr : D);

                DoSaveFile(Frame, 
                        (ALooping) ? ACurr : A, (BLooping) ? BCurr : B,
                        (CLooping) ? CCurr : C, (DLooping) ? DCurr : D);

                DoReset();
                DoLoops();
                Frame++;
                if (HitTheBrakes) { break; }
            }

            // Reset the brakes and finish up
            HitTheBrakes = false;
            Trace = "Batch render done: " + DateTime.Now.ToString() + Environment.NewLine;
            TxtLog.AppendText(Trace);
            Application.DoEvents();
        }

        /// <summary>
        /// Render an image with the current parameters, if looping, I.E.
        /// stepping through a parameter range, one  of the parameters 
        /// will be the current loop value
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

                string Trace = "A:" + A.ToString() + " B:" + B.ToString() +
                    " C:" + C.ToString() + "D:" + D.ToString() + Environment.NewLine;
                TxtLog.AppendText(Trace);
                Application.DoEvents();

                // Create a 32 bit ARGB bitmap to draw on
                if (Display.HasChildren)
                {
                    Display.Image.Dispose();
                }
                Display.Refresh();
                Bitmap Paper = MakePaper();

                // Set up the paper...
                for (int ix = 0; ix < (int)W1; ix++)
                {
                    for (int iy = 0; iy < (int)H1; iy++)
                    {
                        if (DarkMode)
                        {
                            Paper.SetPixel(ix, iy, Color.Black);
                        }
                        else
                        {
                            Paper.SetPixel(ix, iy, Color.White);
                        }
                    }
                }

                // Update the UI image with the clean sheet of paper
                Display.Image = Paper;
                //SnapShot = Paper;
                Application.DoEvents();

                // Init a few working variables
                double x = 0.1; // x coordinate
                double y = 0.1; // y coordinate
                double xn;      // new x coordinate
                double yn;      // new y coordinate
                double xs;      // scaled x coordinate
                double ys;      // scaled y coordinate
                double i = 0.0; // iteration counter

                // Render 10 million iterations
                for (Int64 n = 1; n <= 10000000; n++)
                {
                    i = (double)n;

                    // Calculate new XY coordinates (of note, the rendered 
                    // images from the calculation below appear to be 
                    // rotated 90 degrees and mirrored when compared to the
                    // images in Pickover's "Chaos In Wonderland" book)
                    switch (Mode)
                    {
                        case 0:
                            xn = Math.Sin(y * B) + C * Math.Sin(x * B);
                            yn = Math.Sin(x * A) + D * Math.Sin(y * A);
                            break;
                        case 1:
                            xn = Math.Sin(y * B) + C * Math.Cos(x * B);
                            yn = Math.Cos(x * A) + D * Math.Sin(y * A);
                            break;
                        case 2:
                            xn = Math.Cos(y * B) + C * Math.Cos(x * B);
                            yn = Math.Cos(x * A) + D * Math.Cos(y * A);
                            break;
                        default:
                            xn = Math.Sin(y * B) + C * Math.Sin(x * B);
                            yn = Math.Sin(x * A) + D * Math.Sin(y * A);
                            break;
                    }
                    // Update the coordinates
                    x = xn;
                    y = yn;

                    // Scale the coordinates (scale then center). The scaling
                    // is a little odd since I don't know Max Width 1 until 
                    // the render has completed. As a result, I apply a 
                    // predefined scale factor for x and y (determined from a 
                    // previous render) and then center the result on our
                    // 1025 by 1337 default bitmap (varies based on maximized
                    // screen size). The scale factor comes from the UI or the
                    // predefines JSON file.
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
                        // As we plot, check for a previous pixel and color the 
                        // new pixel appropriatrely
                        Paper.SetPixel((int)xs, (int)ys, GetColor(xs, ys, Paper));

                        // Every 10,000 iterations, update the UI and bitmap display
                        if ((int)n % 10000 == 0)
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
                // Final post render UI and bitmap display update
                TxtIteration.Text = i.ToString();
                Display.Image = Paper;
                if (StampMode)
                {
                    Stampy(TxtName.Text, A, B, C, D);
                }
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
            // Only reset the brakes if we are not looping, otherwise let
            // the looping process (DoLoopRender) detect and clear the brake
            if (!IsLooping) { HitTheBrakes = false; }
        }

        /// <summary>
        /// Create a bitmap for us to draw on
        /// </summary>
        /// <returns>An image bitmap</returns>
        public Bitmap MakePaper()
        {
            return new Bitmap((int)W1, (int)H1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //return new Bitmap((int)Wid, (int)Hei, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Set first time pixels to light gray and then darken them each
        /// time they are revisited until they are black
        /// Or... if DarkMode is true
        /// Set first time pixels to dark gray and then lighten them each
        /// time they are revisited until they are white
        /// 
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
            Color Dg = Color.FromArgb(255, (int)0x1F, (int)0x1F, (int)0x1F); // Very dark gray
            Color Wh = Color.White;

            // Grab the color of the current pixel
            Color got = image.GetPixel((int)x, (int)y);

            // Break out the current pixel color info
            byte r = got.R; // Red
            byte g = got.G; // Blue
            byte b = got.B; // Green
            byte a = got.A; // Alpha channel

            if (DarkMode)
            {
                // If we are darker than white, lighten
                if (r < Wh.R) { r++; }
                if (g < Wh.G) { g++; }
                if (b < Wh.B) { b++; }

                // If we are darker than really dark gray, bump to really dark gray
                if (r < Dg.R) { r = Dg.R; }
                if (g < Dg.G) { g = Dg.G; }
                if (b < Dg.B) { b = Dg.B; }

                // Clamp at White
                if (r >= Wh.R) { r = Wh.R; }
                if (g >= Wh.G) { g = Wh.G; }
                if (b >= Wh.B) { b = Wh.B; }
            }
            else
            {
                // If we are brighter than black, darken
                if (r > Lo.R) { r--; }
                if (g > Lo.G) { g--; }
                if (b > Lo.B) { b--; }

                // If we are brighter than lighter gray (0xFFE8E8E8), bump to lighter gray
                if (r > Hi.R) { r = Hi.R; }
                if (g > Hi.G) { g = Hi.G; }
                if (b > Hi.B) { b = Hi.B; }

                // Clamp at black
                if (r <= Lo.R) { r = Lo.R; }
                if (g <= Lo.G) { g = Lo.G; }
                if (b <= Lo.B) { b = Lo.B; }
            }

            // Reassemble the RGB and return the new color
            return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// The Stampy function will stamp the upper left corner of the rendered image with 
        /// the attractor name as well as the values for a, b, c and d. This should be 
        /// controlled via a UI checkbox (check the checkbox before calling Stampy.
        /// Thank you to Gon Coding of Stackoverflow for the example of how to do this. 
        /// You rock!
        /// </summary>
        /// <param name="name">The name of the attractor preset in use</param>
        /// <param name="a">Value of double A</param>
        /// <param name="b">Value of double B</param>
        /// <param name="c">Value of double C</param>
        /// <param name="d">Value of double D</param>
        public void Stampy(string name, double a, double b, double c, double d)
        {
            Bitmap Velum = (Bitmap)Display.Image;
            RectangleF WreckAndTangle = new RectangleF(0, 0, Velum.Width, Velum.Height);
            Graphics Overlay = Graphics.FromImage(Velum);
            Overlay.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Overlay.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Overlay.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            StringFormat Floormat = new StringFormat()
            {
                Alignment = StringAlignment.Near, // Left
                LineAlignment = StringAlignment.Near // Top
            };
            string Mess = " " + name + "-A:" + a.ToString() + " B:" + b.ToString() + " C:" + c.ToString() + " D:" + d.ToString();
            if (DarkMode)
            {
                Overlay.DrawString(Mess, new Font("Tahoma", 9), Brushes.White, WreckAndTangle, Floormat);
            }
            else
            {
                Overlay.DrawString(Mess, new Font("Tahoma", 9), Brushes.Black, WreckAndTangle, Floormat);
            }
            Overlay.Flush();
            Display.Image = Velum;
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
            string SaveFile;

            // Get the current date and time and format so we can use it in a 
            // filename
            DateTime NowLocal = DateTime.Now;
            string Frac2 = FormatDT(NowLocal);

            if (!IsLooping)
            {
                // If we are not looping, use a normal save file dialog
                // - Build the rest of the fileme and prep the dialog
                SaveFractal.FileName = "Attractor " + TxtName.Text + " " + Frac2 + ".png";
                SaveFractal.Title = "Save image to file";
                if (SaveFractal.ShowDialog() == DialogResult.OK)
                {
                    SaveFile = SaveFractal.FileName;
                    TxtLog.AppendText("File:" + SaveFile + Environment.NewLine);
                }
                else
                {
                    return;
                }
            }
            else
            {
                // Skip file and log save if a path was not supplied
                if (BasePath1 == null) { return; }

                // Otherwise assemble the filename
                SaveFile = BasePath1 + "\\" + "Attractor " + TxtName.Text + "-F" + Frame.ToString() + " " + Frac2 + ".png";
                TxtLog.AppendText("File:" + SaveFile + Environment.NewLine);
            }
            // Save the bitmap as a 32 bit PNG file and update the save log
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
            string LogLine = A.ToString() + "," +
                      B.ToString() + "," +
                      C.ToString() + "," +
                      D.ToString() + "," +
                      IScale.ToString() + "," +
                      Name1 + "," + LogFile;
            F.WriteLine(LogLine);
            F.Flush();
            F.Close();
        }

        /// <summary>
        /// Try to convert the string passed to a double. If it's okay, pass
        /// the value back, otherwise beep and pass back "0.0"
        /// </summary>
        /// <param name="Value">a string</param>
        /// <returns>s string</returns>
        public string CheckD(string Value)
        {
            try
            {
                double D = Convert.ToDouble(Value);
                return Value;
            }
            catch {
                SystemSounds.Beep.Play();
                return "0.0";
            }
        }

        /// <summary>
        /// Try to convert the string passed to an integer. If it's okay, pass
        /// the value back, otherwise beep and pass back "0.0"
        /// </summary>
        /// <param name="Value">a string</param>
        /// <returns>s string</returns>
        public string CheckI(string Value)
        {
            try
            {
                double D = Convert.ToInt32(Value);
                return Value;
            }
            catch
            {
                SystemSounds.Beep.Play();
                return "0";
            }
        }

        private void TxtA_Leave(object sender, EventArgs e)
        {
            TxtA.Text = CheckD(TxtA.Text);
            DoReset();
        }

        private void TxtB_Leave(object sender, EventArgs e)
        {
            TxtB.Text = CheckD(TxtB.Text);
            DoReset();
        }

        private void TxtC_Leave(object sender, EventArgs e)
        {
            TxtC.Text = CheckD(TxtC.Text);
            DoReset();
        }

        private void TxtD_Leave(object sender, EventArgs e)
        {
            TxtD.Text = CheckD(TxtD.Text);
            DoReset();
        }

        private void TxtAEnd_Leave(object sender, EventArgs e)
        {
            TxtAEnd.Text = CheckD(TxtAEnd.Text);
            DoReset();
        }

        private void TxtBEnd_Leave(object sender, EventArgs e)
        {
            TxtBEnd.Text = CheckD(TxtBEnd.Text);
            DoReset();
        }

        private void TxtCEnd_Leave(object sender, EventArgs e)
        {
            TxtCEnd.Text = CheckD(TxtCEnd.Text);
            DoReset();
        }

        private void TxtDEnd_Leave(object sender, EventArgs e)
        {
            TxtDEnd.Text = CheckD(TxtDEnd.Text);
            DoReset();
        }

        private void TxtAStep_Leave(object sender, EventArgs e)
        {
            TxtAStep.Text = CheckD(TxtAStep.Text);
            DoReset();
        }

        private void TxtBStep_Leave(object sender, EventArgs e)
        {
            TxtBStep.Text = CheckD(TxtBStep.Text);
            DoReset();
        }

        private void TxtCStep_Leave(object sender, EventArgs e)
        {
            TxtCStep.Text = CheckD(TxtCStep.Text);
            DoReset();
        }

        private void TxtDStep_Leave(object sender, EventArgs e)
        {
            TxtDStep.Text = CheckD(TxtDStep.Text);
            DoReset();
        }

        private void TxtScale_Leave(object sender, EventArgs e)
        {
            TxtScale.Text = CheckI(TxtScale.Text);
            DoReset();
        }

        private void TxtName_Leave(object sender, EventArgs e)
        {
            DoReset();
        }

        private void Display_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(sender is Control)) return;
            WhatADrag = false;
        }

        private void Display_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SnapShot = Display.Image;
                WhatADrag = true;
                MouseDownLocation = e.Location;
            }
        }

        private void Display_MouseMove(object sender, MouseEventArgs e)
        {
            if (WhatADrag && sender is Control && SnapShot != null)
            {
                Frame = new Rectangle(-(MouseDownLocation.X - e.X), -(MouseDownLocation.Y - e.Y), SnapShot.Width, SnapShot.Height);
                Display.Invalidate();
            }
        }

        private void Display_Paint(object sender, PaintEventArgs e)
        {
            if (SnapShot != null)
            {
                e.Graphics.DrawImage(SnapShot, Frame);
            }
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
