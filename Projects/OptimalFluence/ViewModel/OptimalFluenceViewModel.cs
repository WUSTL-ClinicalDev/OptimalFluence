using OptimalFluence.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OptimalFluence.ViewModel
{
    public class OptimalFluenceViewModel:BindableBase
    {
        private IEnumerable<Beam> beams;
        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        public ObservableCollection<OptimalFluenceModel> OptimalFluences { get; private set; }
        public DelegateCommand PrintCommand { get; private set; }

        public OptimalFluenceViewModel(IEnumerable<Beam> beams)
        {
            this.beams = beams;
            Title = $"Optimal Fluences for {beams.First().Plan.Id}";
            OptimalFluences = new ObservableCollection<OptimalFluenceModel>();
            FillOptimalFluences();
            PrintCommand = new DelegateCommand(OnPrint, CanPrint);
        }

        private void OnPrint()
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                FlowDocument document = new FlowDocument();
                document.Blocks.Add(new Paragraph(new Run($"Optimal Fluence for {beams.First().Plan.Id}"))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                });
                foreach (Beam b in beams.Where(x => !x.IsSetupField))
                {
                    Fluence f = b.GetOptimalFluence();
                    if (f != null)
                    {
                        StackPanel sp = new StackPanel();
                        sp.Children.Add(new TextBlock() { Text = b.Id, TextAlignment= TextAlignment.Center });
                        sp.Children.Add(new System.Windows.Controls.Image()
                        {
                            Height = 100,
                            Width = 100,
                            Source = DrawImage(f)
                        });
                        document.Blocks.Add(new BlockUIContainer()
                        {
                            Child = sp
                        });
                    }
                }
                document.PageHeight = pd.PrintableAreaHeight;
                document.PageWidth = pd.PrintableAreaWidth;
                document.PagePadding = new Thickness(50);
                document.ColumnGap = 0;
                document.ColumnWidth = pd.PrintableAreaWidth;
                IDocumentPaginatorSource dps = document;
                pd.PrintDocument(dps.DocumentPaginator, "FluencePrint");
            }
        }

        private bool CanPrint()
        {
            return OptimalFluences.Count() != null;
        }

        //constant
        double mm_to_inch = 0.03937;
        private void FillOptimalFluences()
        {
           
            foreach(Beam b in beams.Where(x=>!x.IsSetupField))
            {
                Fluence fluence = b.GetOptimalFluence();
                if (fluence != null)
                {
                    float[,] pixels = fluence.GetPixels();

                    var SizeX = fluence.XSizePixel;
                    var SizeY = fluence.YSizePixel;
                    int stride = SizeY * 3 + (SizeY % 4);
                    BitmapSource bms = DrawImage(fluence);
                    OptimalFluences.Add(new OptimalFluenceModel
                    {
                        FieldId = b.Id,
                        OptimalFluenceMap = bms
                        
                    });
                }

            }
        }
        private BitmapSource DrawImage(Fluence f)
        {
            float[,] pixels = f.GetPixels();
            float image_min = 1000;
            float image_max = -1;
            float[] image_pixels = new float[f.XSizePixel * f.YSizePixel];
            for (int j = 0; j < f.YSizePixel; j++)
            {
                for (int k = 0; k < f.XSizePixel; k++)
                {
                    image_pixels[k + j * f.XSizePixel] = pixels[j, k];
                    if(pixels[j,k] < image_min) { image_min = pixels[j, k]; }
                    if (pixels[j, k] > image_max) { image_max = pixels[j, k]; }
                }
            }
            //byte[] image_bytes = new byte[image_pixels.Length * sizeof(UInt16)];
            //Buffer.BlockCopy(image_pixels, 0, image_bytes, 0, image_bytes.Length);
            PixelFormat format = PixelFormats.Rgb24;
            //get min and max of the data.
            //iamge min now comes from the method input parameter image_min
            //image_min = image_pixels.Min();//this is the red value. I.e. 255,0,0
            //image_max = image_pixels.Max();//this is the blue value. i.e. 0,0,255.
            double image_med = (image_max + image_min) / 2;
            int stride = (f.XSizePixel * format.BitsPerPixel + 7) / 8;
            byte[] image_bytes = new byte[stride * f.XSizePixel];
            //copy data to image bytes 
            for (int j = 0; j < image_pixels.Length; j++)
            {
                float value = image_pixels[j];
                System.Windows.Media.Color c = new System.Windows.Media.Color();
                if (value < image_min)
                {
                    c.B = 0;
                    c.R = 0;
                    c.G = 0;
                }
                else if (value < image_med)
                {
                    c.R = 0;
                    c.B = Convert.ToByte(255 - (255 * (value - image_min) / (image_med - image_min)));
                    c.G = Convert.ToByte(255 - (255 * (image_med - value) / (image_med - image_min)));
                }
                else if (value <= image_max)
                {
                    c.B = 0;
                    c.R = Convert.ToByte(255 - (255 * (image_max - value) / (image_max - image_med)));
                    c.G = Convert.ToByte(255 - (255 * (value - image_med) / (image_max - image_med)));
                }
                else if (value > image_max)
                {
                    c.R = 0;
                    c.B = 0;
                    c.G = 0;
                }
                image_bytes[j * 3] = c.R;
                image_bytes[j * 3 + 1] = c.G;
                image_bytes[j * 3 + 2] = c.B;

            }
            BitmapSource bmp = BitmapSource.Create(f.XSizePixel,
                f.YSizePixel,
                (f.XSizePixel/f.XSizeMM)/mm_to_inch,
                (f.YSizePixel / f.YSizeMM) / mm_to_inch,                                                
                format,
                null,
                image_bytes,
                stride
                );
            return bmp;
        }
    }
}
