using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Demon.ViewModel;
using Demon.Model;

namespace Demon.View
{
    /// <summary>
    /// AppUnit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AppUnit : UserControl
    {
        public AppUnitModel myData
        { get; set; }

        public AppUnit()
        {
            InitializeComponent();          
        }

        public AppUnit(AppUnitModel unitModel, MainWindowViewModel mainModel)
        {
            InitializeComponent();
            myData = unitModel;
            
            AppUnitViewModel appUnitViewModel = new AppUnitViewModel(this, unitModel, mainModel);
            this.DataContext = appUnitViewModel;
        }
    }
}
