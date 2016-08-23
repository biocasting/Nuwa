using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nuwa
{
    static class cTextIO
    {
       
        //对richtextbox重载
        public static void AppendText(this CCWin.SkinControl.RtfRichTextBox rtBox, string text, Color color, bool addNewLine = true)
        {
            if (addNewLine)
            {
                rtBox.ForeColor = Color.Gray;
                text += Environment.NewLine;
            }
            rtBox.SelectionStart = rtBox.TextLength;
            rtBox.SelectionLength = 0;
            rtBox.SelectionColor = color;
            rtBox.AppendText(text);
            rtBox.ScrollToCaret();
        }
    }
}
