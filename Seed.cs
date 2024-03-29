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
    public partial class Seed : Form
    {
        public Seed()
        {
            InitializeComponent();
        }

        //Get the number and set the number inside the NumericUpDown Box
        public int GetSeed()
        {
            return (int)numericUpDown1.Value;
        }

        public void SetSeed(int SeedValue)
        {
            numericUpDown1.Value = SeedValue /* num = seed number */ ;
        }

        private void RandomizeButton_Click(object sender, EventArgs e)
        {
            Random NewSeed = new Random();
            int ReplaceSeed = NewSeed.Next(int.MinValue, int.MaxValue);
            SetSeed(ReplaceSeed);
        }
    }
}
