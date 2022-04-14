using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace amogus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool flipped = false;
        CrewmateController crewmateController;

        public MainWindow()
        {
            InitializeComponent();

            crewmate.RenderTransformOrigin = new Point(0.5, 0.5);            

            //Task.Run(FlipCrewmate);
        }

        async Task FlipCrewmate()
        {
            await Task.Delay(2000);

            Dispatcher.Invoke(() =>
            {
                flipped = !flipped;

                ScaleTransform flipTrans = new ScaleTransform();
                flipTrans.ScaleX = flipped ? -1 : 1;
                //flipTrans.ScaleY = -1;
                crewmate.RenderTransform = flipTrans;
            });

            await FlipCrewmate();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //crewmate.Margin = new Thickness(0, ActualHeight - crewmate.Height, 0, 0);

            if (crewmateController == null)
            {
                crewmateController = new CrewmateController(crewmate, this);
            }
        }
    }
}
