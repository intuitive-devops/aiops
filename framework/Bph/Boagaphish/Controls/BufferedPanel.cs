//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2022, all rights reserved.
//
using System.Windows.Forms;

namespace Boagaphish.Controls
{
    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }
    }
}
