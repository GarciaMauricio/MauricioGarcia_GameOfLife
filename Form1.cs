﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MauricioGarcia_GameOfLife
{
    public partial class Form1 : Form
    {
        //UniSize
        static int width = Properties.Settings.Default.Width;
        static int height = Properties.Settings.Default.Height;

        // The universe array
        bool[,] universe = new bool[width, height];

        //Created ScratchPad here...
        bool[,] ScratchPad = new bool[width, height];

        //Seed
        int value = Properties.Settings.Default.Seed;

        // Drawing colors
        Color panelColor = Properties.Settings.Default.BackgroundColor;
        Color LineX10Color = Properties.Settings.Default.GridX10Color;
        Color gridColor = Properties.Settings.Default.GridColor;
        Color cellColor = Properties.Settings.Default.CellColor;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = Properties.Settings.Default.Timer; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer not running

            //Millisecond Dysplay
            toolStripStatusInterval.Text = "Interval = " + timer.Interval.ToString();
            //Seed Display
            toolStripStatusSeed.Text = "Seed = " + value.ToString();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            //Live cell count
            int live = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //get neighbor count for each living cell, option
                    int count = CountNeighborsFinite(x, y); // default
                    if (finiteToolStripMenuItem.Checked == true)
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    else if (torisToolStripMenuItem.Checked == true)
                    {
                        count = CountNeighborsToroidal(x, y);
                    }

                    //Apply the rules here
                    if (universe[x, y] == true)
                    {
                        if (count < 2)
                        {
                            ScratchPad[x, y] = false;
                        }
                        else if (count > 3)
                        {
                            ScratchPad[x, y] = false;
                        }
                        else if (count == 2 || count == 3)
                        {
                            ScratchPad[x, y] = true;
                        }
                    }
                    else
                    {
                        if (count == 3)
                        {
                            ScratchPad[x, y] = true;
                        }
                    }
                }
            }


            //Copy what is in the ScratchPad to the universe
            //( Swap them )
            bool[,] hold = universe;
            universe = ScratchPad;
            ScratchPad = hold;
            //2nd time the NextGeneration executes just clear out anything in the ScratchPad that shouldn't be turned on
            bool[,] Empty = new bool[width, height];
            ScratchPad = Empty;

            // Increment generation count
            generations++;

            // Update status strips
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusInterval.Text = "Interval = " + timer.Interval.ToString();

            //Real time Update Live Cell Count
            for (int yPosition = 0; yPosition < universe.GetLength(1); yPosition++)
            {
                for (int xPosition = 0; xPosition < universe.GetLength(0); xPosition++)
                {
                    if (universe[xPosition, yPosition] == true)
                    {
                        live++;
                    }
                }
            }
            toolStripStatusLabelAlive.Text = "Alive = " + live.ToString();

            //Invalidate graphicsPanel1
            graphicsPanel1.Invalidate();
        }



        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            //FLOATS MAKE PROGRAM MUCH BETTER( REPLACE INTS ) 
            //CONVERT TO FLOATS!!!
            //( Calculating the width and the height of each window )
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);


            Pen boldPen = new Pen(LineX10Color, 3);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            //Painting BackColor of the Panel
            graphicsPanel1.BackColor = panelColor;

            // Iterate through the universe in the y, top to bottom
            //started with y top then x on bottom, not necessary, this just moves through it the way westerns read
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //Use RectangleF!!! after replacing cellWidth and cellHeight with floats 
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    if (x % 10 == 0)
                    {
                        int _y = universe.GetLength(1);
                        Point point1 = new Point((int)(x * cellWidth), 0);
                        Point point2 = new Point((int)(x * cellWidth), (int)((_y + 1) * (cellHeight)));
                        e.Graphics.DrawLine(boldPen, point1, point2);
                    }
                    if (y % 10 == 0)
                    {
                        int _x = universe.GetLength(0);
                        Point point1 = new Point(0, (int)(y * cellHeight));
                        Point point2 = new Point((int)((_x + 1) * (cellWidth)), (int)(y * cellHeight));
                        e.Graphics.DrawLine(boldPen, point1, point2);
                    }
                }
            }

            //HUD


            // Cleaning up pens and brushes
            gridPen.Dispose();
            boldPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                //Convert to FLOATS!!! AS WELL!!!
                // Calculate the width and height of each cell in pixels
                float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)(e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)(e.Y / cellHeight);

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                //Live cell count
                int live = 0;
                for (int yPosition = 0; yPosition < universe.GetLength(1); yPosition++)
                {
                    // Iterate through the universe in the x, left to right
                    for (int xPosition = 0; xPosition < universe.GetLength(0); xPosition++)
                    {
                        if (universe[xPosition, yPosition] == true)
                        {
                            live++;
                        }
                    }
                }
                toolStripStatusLabelAlive.Text = "Alive = " + live.ToString();

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();

                //CALL INVALIDATE WHEN UPDATING, CLICKING, TURNING ON OR OFF THINGS
                //NEVER PLACE INVALIDATE INSIDE YOUR PAINT
            }
        }



        /*____________________PANEL BAR____________________*/
        //New Click
        private void New_Click(object sender, EventArgs e)
        {
            int live = 0;
            timer.Enabled = false;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelAlive.Text = "Alive = " + live.ToString();
            graphicsPanel1.Invalidate();
        }


        //Open Click
        //Save Click


        //Start timer button( starts the timer to true, Line 35 )
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }


        //Pause timer button( disables the time to false, Line 35 )
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }


        //Next timer button( calls nextgeneration once, Line 39 )
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }



        /*____________________FILE TAB____________________*/
        //File New button( repeats nested for loops, Line 93 )
        //( Inside both nested loops set universe[x,y] = false; inside function Invalidate GraphicsPanel!!!)
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int live = 0;
            timer.Enabled = false;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelAlive.Text = "Alive = " + live.ToString();
            graphicsPanel1.Invalidate();
        }



        //Open Button
        //Import Button
        //Save Button


        //Exit Button
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        /*____________________VIEW TAB____________________*/
        //HUD View
        
        //Neighbor Count View

        //Grid View
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == false)
            {
                gridToolStripMenuItem1.Checked = false;
                gridColor = Color.Transparent;
                LineX10Color = Color.Transparent;
            }
            else
            {
                gridToolStripMenuItem1.Checked = true;
                LineX10Color = Color.Black;
                gridColor = Color.Black;
            }
            graphicsPanel1.Invalidate();
        }

        //Finite View
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (finiteToolStripMenuItem.Checked == true)
            {
                finiteToolStripMenuItemRight.Checked = true;
                torisToolStripMenuItem.Checked = false;
                toroidalToolStripMenuItemRight.Checked = false;
            }
            else
            {
                finiteToolStripMenuItemRight.Checked = false;
                toroidalToolStripMenuItemRight.Checked = true;
                torisToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }


        //Toroidal View
        private void torisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (torisToolStripMenuItem.Checked == true)
            {
                toroidalToolStripMenuItemRight.Checked = true;
                finiteToolStripMenuItem.Checked = false;
                finiteToolStripMenuItemRight.Checked = false;
            }
            else
            {
                toroidalToolStripMenuItemRight.Checked = false;
                finiteToolStripMenuItem.Checked = true;
                finiteToolStripMenuItemRight.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }



        /*____________________RANDOMIZE TAB____________________*/
        //Seed
        private void seedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Seed sd = new Seed();

            sd.SetSeed(value);

            //Created Dialog Box, Instanciated it
            if (DialogResult.OK == sd.ShowDialog()/*Show dialog box, ( modeless/tool window ) use only: Show*/)
            {
                value = sd.GetSeed();

                Random newseed = new Random(value);

                //Remember to always invalidate
                for (int y1 = 0; y1 < universe.GetLength(1); y1++)
                {
                    for (int x1 = 0; x1 < universe.GetLength(0); x1++)
                    {
                        //Randomize ( NOT COMPLETED )
                        
                        if (value == 0)
                        {
                            universe[x1, y1] = true;
                        }
                        else
                        {
                            universe[x1, y1] = false;
                        }
                    }
                }
                toolStripStatusSeed.Text = "Seed = " + value.ToString();
            }
            graphicsPanel1.Invalidate();
        }


        //Current Seed



        //Time



        /*____________________SETTINGS TAB____________________*/
        //Changing Background Color
        private void backgroundToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Creating Instance of the Dialog Box
            ColorDialog BkGrnd = new ColorDialog();

            //Show Current Color of the object being customized ( need property )
            //To get Property and set it to the object color:
            BkGrnd.Color = panelColor;

            //Most important thing to know about the box, is to know how the user closed it, whether it was an okay change of a cancel
            //The resolve:
            if (DialogResult.OK == BkGrnd.ShowDialog()/*Showing the Dialog Box*/)
            {
                panelColor = BkGrnd.Color;
                graphicsPanel1.Invalidate();
            }
            BkGrnd.Dispose();
        }

        //Changing Cell Color
        private void cellsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog cell = new ColorDialog();

            cell.Color = cellColor;

            if (DialogResult.OK == cell.ShowDialog())
            {
                cellColor = cell.Color;
                graphicsPanel1.Invalidate();
            }
            cell.Dispose();
        }

        //Changing Grid Color
        private void gridToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ColorDialog grid = new ColorDialog();

            grid.Color = gridColor;

            if (DialogResult.OK == grid.ShowDialog())
            {
                gridColor = grid.Color;
                graphicsPanel1.Invalidate();
            }
            grid.Dispose();
        }

        //Changing Grid x10 Color
        private void gridX10ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog Gx10 = new ColorDialog();

            Gx10.Color = LineX10Color;

            if (DialogResult.OK == Gx10.ShowDialog())
            {
                LineX10Color = Gx10.Color;
                graphicsPanel1.Invalidate();
            }
            Gx10.Dispose();
        }

        //Options Tab
        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Options opns = new Options();

            opns.SetMilliSec(timer.Interval);
            opns.SetWidth(width);
            opns.SetHeight(height);

            if (DialogResult.OK == opns.ShowDialog())
            {
                if (opns.GetMilliSec() != timer.Interval)
                {
                    timer.Interval = opns.GetMilliSec();
                    toolStripStatusInterval.Text = "Interval = " + timer.Interval.ToString();
                }
                if (opns.GetWidth() != width || opns.GetHeight() != height)
                {
                    width = opns.GetWidth();
                    height = opns.GetHeight();
                    universe = new bool[width, height];
                    ScratchPad = new bool[width, height];
                }
            }
            graphicsPanel1.Invalidate();
        }

        //Reset
        //Reload



        /*________________________________________RIGHT CLICK________________________________________*/



        /*____________________VIEW TAB____________________*/
        //HUD


        //Neighbor Count


        //Grid
        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem1.Checked == false)
            {
                gridToolStripMenuItem.Checked = false;
                gridColor = Color.Transparent;
                LineX10Color = Color.Transparent;
            }
            else
            {
                gridToolStripMenuItem.Checked = true;
                LineX10Color = Color.Black;
                gridColor = Color.Black;
            }
            graphicsPanel1.Invalidate();
        }

        //Finite
        private void finiteToolStripMenuItemRight_Click(object sender, EventArgs e)
        {
            if (finiteToolStripMenuItemRight.Checked == true)
            {
                finiteToolStripMenuItem.Checked = true;
                toroidalToolStripMenuItemRight.Checked = false;
                torisToolStripMenuItem.Checked = false;
            }
            else
            {
                finiteToolStripMenuItem.Checked = false;
                torisToolStripMenuItem.Checked = true;
                toroidalToolStripMenuItemRight.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        //Toroidal
        private void toroidalToolStripMenuItemRight_Click(object sender, EventArgs e)
        {
            if (toroidalToolStripMenuItemRight.Checked == true)
            {
                torisToolStripMenuItem.Checked = true;
                finiteToolStripMenuItem.Checked = false;
                finiteToolStripMenuItemRight.Checked = false;
            }
            else
            {
                torisToolStripMenuItem.Checked = false;
                finiteToolStripMenuItem.Checked = true;
                finiteToolStripMenuItemRight.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }



        /*____________________COLORS TAB____________________*/
        //BackGround Color


        //Cells


        //Grid


        //Grid x10


    }
}
