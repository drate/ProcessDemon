using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Demon.ViewModel;
using System.Diagnostics;
using System.Windows;


namespace Demon.Model
{
    public class AppUnitModel
    {
        public enum unitState
        {
            enExecuting,
            enStoping,
            enNoFile
        };

        public enum enUnits
        {
            Program1,
            Program2,
            Program3,
            Program4
        };

        public object myUI
        { get; set; }        

        public string sFileName
        { get; set; }
        
        public string sPath
        { get; set; }
              

        public string sFullPath
        {
            get; set;
        }


        public String sTxtFilePath
        { get; set; }

        public DateTime dtLastEndTime
        {
            get; set;
        }

        public unitState enNowState
        {
            get; set;
        }
        public unitState enLastState
        {
            get; set;
        }

        public enUnits enMyIdentity
        {
            get; set;
        }


        public System.Windows.Media.ImageSource imgIcon
        { get; set; }

        public AppUnitModel(string exePath, string txtPath, enUnits enIdentity)
        {
            imgIcon = GetFileIcon(exePath);

            Init(exePath);

            sTxtFilePath = txtPath;
            enMyIdentity = enIdentity;
            dtLastEndTime = new DateTime();
            dtLastEndTime = DateTime.Now;                       
        }

        public void Init(string exePath)
        {
            sFullPath = exePath;

            if (exePath != null && exePath != "")
            {
                sFileName = System.IO.Path.GetFileName(exePath);
                sPath = System.IO.Path.GetDirectoryName(exePath);
                RefreshIcon();
            }
            else
            {
                sFileName = string.Empty;
                sPath = string.Empty;
            }
        }

        public bool bIsAutoExcute
        {
            get; set;
        }            

        
        public void AppExcute()
        {
            //kym            
            if (System.IO.File.Exists(sPath + @"\" + sFileName))
            {
                try
                {
                    System.Diagnostics.Process ps = new System.Diagnostics.Process();
                    ps.StartInfo.FileName = sFileName;
                    ps.StartInfo.WorkingDirectory = sPath;
                    ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    ps.Start();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                
            } 
            else
            {
                MessageBox.Show("Check File Path.");
            }
            
        }

        public unitState GetUnitState()
        {
            string filePath = sPath + @"\" + sFileName;
            if(System.IO.File.Exists(filePath))
            {
                Process[] allProc = Process.GetProcesses();

                foreach (Process p in allProc)
                {
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(sFileName);
                    if (p.ProcessName.Contains(fileNameWithoutExtension))
                    {                        
                        return unitState.enExecuting;
                    }
                }

                return unitState.enStoping;
            }
            else
            {
                return unitState.enNoFile;
            }
        }
        //Get Image From .EXE file and Set Image to icon
        
        public void RefreshIcon()
        {
            imgIcon = GetFileIcon(sFullPath);
        }

        private System.Windows.Media.ImageSource GetFileIcon(string sFilePath)
        {
            if (System.IO.File.Exists(sFilePath) == false) return null;

            System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(sFilePath);
            System.Windows.Media.ImageSource icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            sysicon.Handle,
                            System.Windows.Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            
            return icon;
        }
    }
}
