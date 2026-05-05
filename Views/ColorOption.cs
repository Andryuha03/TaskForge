using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace TaskForge.Views
{
    internal class ColorOption
    {
        public string Name { get; set; }
        public string HexCode { get; set; }
        public SolidColorBrush Brush => new SolidColorBrush((Color)ColorConverter.ConvertFromString(HexCode));
    }
}
