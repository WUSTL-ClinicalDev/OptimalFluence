using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OptimalFluence.Model
{
    public class OptimalFluenceModel
    {
        public string FieldId { get; set; }
        public BitmapSource OptimalFluenceMap { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }
}
