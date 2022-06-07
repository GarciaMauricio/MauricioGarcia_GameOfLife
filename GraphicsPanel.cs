using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Change the namespace to your project's namespace.
namespace MauricioGarcia_GameOfLife
{
    class GraphicsPanel : Panel
    {
        // Default constructor
        public GraphicsPanel()
        {
            // Turn on double buffering.
            //CAN'T TURN ON IN REGULAR PANEL
            //Have to turn it on in a derived class that implements it because it's protected
            this.DoubleBuffered = true;

            // Allow repainting when the window is resized.
            //More efficient way to render when rendering IS based on the size of the window
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
    }
}
