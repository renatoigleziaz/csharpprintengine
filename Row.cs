using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PrintingCore
{
    public class Row
    {
        public ListView lst;
        public bool backgroundcolor = false;
        public int bk_r = 230;
        public int bk_g = 231;
        public int bk_b = 232;
        public bool circle = false;

        public Row()
        {
            lst = new ListView();
            lst.Columns.Clear();
            lst.Columns.Add("percent");
            lst.Columns.Add("type");
            lst.Columns.Add("align");
            lst.Columns.Add("font");
            lst.Columns.Add("size");
            lst.Columns.Add("bold");
            lst.Columns.Add("text");
            lst.Columns.Add("colorr");
            lst.Columns.Add("colorg");
            lst.Columns.Add("colorb");
            lst.Columns.Add("cuttext");
        }

        public string Item(int posrow, string field)
        {
            string value = "";
            ListViewItem row = lst.Items[posrow];

            for (int i = 0; i < lst.Columns.Count; i++)
            {
                if (field.ToLower() == lst.Columns[i].Text.ToLower())
                {
                    if (i == 0)
                        value = row.Text;
                    else
                        value = row.SubItems[i].Text;

                    break;
                }
            }

            row = null;

            return value;
        }

        public void AddCol(int percent, 
                           string type, 
                           string align, 
                           string font, 
                           int size, 
                           bool bold, 
                           string text, 
                           int r, 
                           int g, 
                           int b,
                           bool cuttext = false)
        {
            ListViewItem col = lst.Items.Add(percent.ToString());
            col.SubItems.Add(type);
            col.SubItems.Add(align);
            col.SubItems.Add(font);
            col.SubItems.Add(size.ToString());
            if (bold)
                col.SubItems.Add("1");
            else
                col.SubItems.Add("0");
            col.SubItems.Add(text);
            col.SubItems.Add(r.ToString());
            col.SubItems.Add(g.ToString());
            col.SubItems.Add(b.ToString());
            col.SubItems.Add(cuttext.ToString());
            col = null;
        }

    }
}
