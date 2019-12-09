using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MdiMvvm.ValueObjects
{
    public class MdiWindowPosition
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        
        public MdiWindowPosition()
        {
            Left = 0;
            Top = 0;
        }
    }
}
