using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Demon.View;
using Demon.Model;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls;

namespace Demon.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public List<AppUnit> lstAppUnits;
        System.Windows.Forms.NotifyIcon trayIcon;
        MainWindow mainWindow;
        Thread thdChkAppIsExcute;

        public MainWindowViewModel(MainWindow mainWin)
        {
            Entrance(mainWin);

            thdChkAppIsExcute.Start();
        }

        //초기화
        private void Entrance(MainWindow mainWin)
        {
            mainWindow = mainWin;

            mainWindow.Closing += MainWindow_Closing;

            thdChkAppIsExcute = new Thread(new ThreadStart(chkAppLive));        
            
            //trayIcon Init
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Visible = true;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;
            trayIcon.Icon = ProcessDemon.Properties.Resources.Icon1;

            
            //tryIcon Context Menu Init
            System.Windows.Forms.ContextMenu contxtMenuTray = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem contxtMenuitem_Open = new System.Windows.Forms.MenuItem();
            contxtMenuitem_Open.Index = 0;
            contxtMenuitem_Open.Text = "Demon 열기";
            contxtMenuitem_Open.Click += (o, e) =>
            {
                mainWin.Show();
            };
            contxtMenuTray.MenuItems.Add(contxtMenuitem_Open);

            //나눔선
            Separator separaLine = new Separator();
            
            

            contxtMenuTray.MenuItems.Add("-");

            System.Windows.Forms.MenuItem contxtMenuitem_Close = new System.Windows.Forms.MenuItem();
            contxtMenuitem_Close.Index = 1;
            contxtMenuitem_Close.Text = "Demon 종료";
            contxtMenuitem_Close.Click += (o, e) =>
            {
                Environment.Exit(0);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                mainWin.Close();
            };

            contxtMenuTray.MenuItems.Add(contxtMenuitem_Close);
            trayIcon.ContextMenu = contxtMenuTray;

            //AppUnit Init
            lstAppUnits = new List<AppUnit>();


            string txtFilePath = string.Empty;
            string exeFilePath = string.Empty;
            string docPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            txtFilePath = docPath + @"\ProcessDemon\Program_Path.txt";

            if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtFilePath)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(txtFilePath));
            }

            if (!File.Exists(txtFilePath))
            {
                File.Create(txtFilePath);
            }


            AppUnitModel appViewerUnitModel1 = new AppUnitModel(exeFilePath, txtFilePath, AppUnitModel.enUnits.Program1);
            AppUnit appViewerUnit1 = new AppUnit(appViewerUnitModel1, this);
            lstAppUnits.Add(appViewerUnit1);

            AppUnitModel appViewerUnitModel2 = new AppUnitModel(exeFilePath, txtFilePath, AppUnitModel.enUnits.Program2);
            AppUnit appViewerUnit2 = new AppUnit(appViewerUnitModel2, this);
            lstAppUnits.Add(appViewerUnit2);

            AppUnitModel appViewerUnitModel3 = new AppUnitModel(exeFilePath, txtFilePath, AppUnitModel.enUnits.Program3);
            AppUnit appViewerUnit3 = new AppUnit(appViewerUnitModel3, this);
            lstAppUnits.Add(appViewerUnit3);

            AppUnitModel appViewerUnitModel4 = new AppUnitModel(exeFilePath, txtFilePath, AppUnitModel.enUnits.Program4);
            AppUnit appViewerUnit4 = new AppUnit(appViewerUnitModel4, this);
            lstAppUnits.Add(appViewerUnit4);
            
           

            using (System.IO.StreamReader sr = new System.IO.StreamReader(txtFilePath))
            {
                string line = string.Empty;
                int num = 0;
     
                while ((line = sr.ReadLine()) != null || num < 4)
                {
                    exeFilePath = line;
                    AppUnit appViewerUnit = lstAppUnits[num];
                    AppUnitViewModel appViewerUnitModel = appViewerUnit.DataContext as AppUnitViewModel;
                    appViewerUnitModel.SetPath(exeFilePath);

                    num++;
                }
            }



            //Create Unit List 
            foreach (AppUnit unit in lstAppUnits)
            {
                mainWin.stkPanelAppUnits.Children.Add(unit);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.Hide();
            e.Cancel = true;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            mainWindow.Show();
            mainWindow.WindowState = System.Windows.WindowState.Normal;
        }
        
        private void ChangeAllAppUnit_AutoExcuteState()
        {

        }

        private void threadChkAppIsExcute_Stop()
        {
        }


        //Process Check Process
        private void chkAppLive()
        {
            while (true)
            {   
                foreach (AppUnit unit in lstAppUnits)
                {
                    unit.grdBack.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                        {

                            //현재 프로그램의 상태를 반영하고
                            //프로그램이 마지막으로 꺼진 시간을 위해 프로그램의 바로 전상태를 저장해놓는다.
                            if (unit.myData.GetUnitState() == AppUnitModel.unitState.enExecuting)
                            {
                                unit.grdBack.Background = System.Windows.Media.Brushes.Orange;

                                //프로그램이 마지막으로 꺼진시간을 저장하기위해 프로그램에 마지막 상태를 저장한다.
                                unit.myData.enLastState = AppUnitModel.unitState.enExecuting;
                            }
                            else if (unit.myData.GetUnitState() == AppUnitModel.unitState.enStoping)
                            {
                                unit.grdBack.Background = System.Windows.Media.Brushes.LightGray;
                                if(unit.myData.bIsAutoExcute)
                                {
                                    unit.myData.AppExcute();
                                }


                                //마지막으로 꺼진시간을 저장하기 위해 프로그램의 마지막상태를 저장한다.
                                if(unit.myData.enLastState == AppUnitModel.unitState.enExecuting)
                                {
                                    unit.myData.dtLastEndTime = DateTime.Now;
                                    unit.lbEndTime.Content = unit.myData.dtLastEndTime.ToString();
                                    unit.myData.enLastState = AppUnitModel.unitState.enStoping;
                                }

                            }
                            else if (unit.myData.GetUnitState() == AppUnitModel.unitState.enNoFile)
                            {
                                unit.grdBack.Background = System.Windows.Media.Brushes.Gray;

                                //위에 설명과 같다.
                                unit.myData.enLastState = AppUnitModel.unitState.enNoFile;
                            }
                        }));
                }

                Thread.Sleep(1000);
            }
        }


        ICommand allAppUnitCheck_Click;
        public ICommand AllAppUnitCheck_Click
        {
            get
            {
                if(allAppUnitCheck_Click == null)
                {
                    allAppUnitCheck_Click = new DelegateCommand(obj =>
                    {
                        foreach(AppUnit unit in lstAppUnits)
                        {
                            unit.chkboxAutoExcute.IsChecked = true;
                            unit.myData.bIsAutoExcute = true;
                        }
                    });
                }
                return allAppUnitCheck_Click;
            }
        }


    }
}
