using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Demon.Model;
using Demon.View;
using System.IO;

namespace Demon.ViewModel
{
    class AppUnitViewModel : ViewModelBase
    {
        AppUnit myAppUnit;
        public AppUnitModel myAppData
        {get; set;}

        MainWindowViewModel mainViewModel
        { get; set; }

        public AppUnitViewModel(AppUnit appUnit, AppUnitModel unitModel, MainWindowViewModel mainModel)
        {
            myAppUnit = appUnit;
            myAppData = unitModel;
            mainViewModel = mainModel;
            Entrance();
        }

        void Entrance()
        {             
            myAppUnit.lbFilePath.Content = myAppData.sFileName;
            
            myAppUnit.lbEndTime.Content = myAppData.dtLastEndTime.ToString();
            myAppUnit.imgAppIcon.Source = myAppData.imgIcon;
            
            switch (myAppData.enMyIdentity)
            {
                case AppUnitModel.enUnits.Program1:
                    myAppUnit.lbAppTitle.Content = "Program1";
                    break;

                case AppUnitModel.enUnits.Program2:
                    myAppUnit.lbAppTitle.Content = "Program2";
                    break;

                case AppUnitModel.enUnits.Program3:
                    myAppUnit.lbAppTitle.Content = "Program3";
                    break;

                case AppUnitModel.enUnits.Program4:
                    myAppUnit.lbAppTitle.Content = "Program4";
                    break;

                default:
                    break;
            }
        }

        private bool isChecked;
        private ICommand checkCommand;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                myAppData.bIsAutoExcute = IsChecked;
            }
        }

        public ICommand CheckCommand
        {
            get
            {
                if (checkCommand == null)
                    checkCommand = new DelegateCommand(i => Checkprocess(i), null);
                return checkCommand;
            }
            set
            {
                checkCommand = value;                
            }
        }
        public void Checkprocess(object sender)
        {            
            //this DOES react when the checkbox is checked or unchecked
        }


        ICommand btnFileDialog_Click;
        public ICommand BtnFileDialog_Click
        {
            get
            {
                if(btnFileDialog_Click == null)
                {
                    btnFileDialog_Click = new DelegateCommand(obj =>
                    {
                        string sNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(myAppData.sFileName);
                        //string docuPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string drivePath = System.IO.Path.GetPathRoot(Environment.SystemDirectory); ;

                        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                        openFileDialog.Multiselect = false;
                        openFileDialog.Filter = sNameWithoutExtension + "(*.exe) | *.exe";
                        openFileDialog.InitialDirectory = drivePath;

                        if ((bool)openFileDialog.ShowDialog())
                        {
                            //do save txt                            
                            
                            myAppData.Init(openFileDialog.FileName);
                            myAppData.RefreshIcon();

                            myAppUnit.imgAppIcon.Source = myAppData.imgIcon;
                            myAppUnit.lbFilePath.Content = myAppData.sFileName;

                            if(File.Exists(myAppData.sTxtFilePath))
                            {
                                System.IO.File.WriteAllText(myAppData.sTxtFilePath, string.Empty);
                            }

                            foreach(AppUnit unitControl in mainViewModel.lstAppUnits)
                            {
                                AppUnitViewModel unitModel = unitControl.DataContext as AppUnitViewModel;
                                System.IO.File.AppendAllText(unitModel.myAppData.sTxtFilePath, unitModel.myAppData.sFullPath + Environment.NewLine);
                            }


                            
                        }
                    });
                }
                return btnFileDialog_Click;
            }
        }

        ICommand btnExcute_Click;
        public ICommand BtnExcute_Click
        {
            get
            {
                if (btnExcute_Click == null)
                {
                    btnExcute_Click = new DelegateCommand(obj =>
                    {
                        myAppData.AppExcute();
                    });
                }
                return btnExcute_Click;
            }
        }

        public  void SetPath(string sPath)
        {

            myAppData.Init(sPath);
            myAppData.RefreshIcon();

            myAppUnit.imgAppIcon.Source = myAppData.imgIcon;
            myAppUnit.lbFilePath.Content = myAppData.sFileName;
        }
             

        public void SetState()
        {
            if(myAppData.enNowState == AppUnitModel.unitState.enExecuting)
            {
                myAppUnit.grdBack.Background = System.Windows.Media.Brushes.Orange;
            }
            else if(myAppData.enNowState == AppUnitModel.unitState.enStoping)
            {
                myAppUnit.grdBack.Background = System.Windows.Media.Brushes.LightGray;
            }
            else if(myAppData.enNowState == AppUnitModel.unitState.enNoFile)
            {
                myAppUnit.grdBack.Background = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
