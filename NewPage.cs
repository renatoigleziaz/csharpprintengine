using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrintingCore;
using System.Drawing;
using System.Drawing.Printing;

namespace PrintingCore
{
    public class NewPage : IPrintable
    {
        public void Print(PrintElement element)
        {
            element.NewPage();
        }
    }
}
