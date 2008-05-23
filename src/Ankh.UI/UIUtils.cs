using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Ankh.UI
{
    /// <summary>
    /// This class contains utility methods for the UI.
    /// </summary>
    class UIUtils
    {
        /// <summary>
        /// Resizes a <code>System.Windows.Forms.ComboBox</code>'s DropDownWidth
        /// based on the longest string in the list.
        /// </summary>
        public static void ResizeDropDownForLongestEntry(ComboBox comboBox)
        {
            int width = comboBox.DropDownWidth;
            Graphics g = comboBox.CreateGraphics();
            Font font = comboBox.Font;
            int vertScrollBarWidth = (comboBox.Items.Count > comboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;
            int newWidth;

            foreach (string s in comboBox.Items)
            {
                newWidth = (int)g.MeasureString(s, font).Width
                    + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            comboBox.DropDownWidth = width;
        }
    }
}
