using System.Configuration;
using System.Data;
using System.Windows;
using OfficeOpenXml;



namespace Space
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       
        public App()
        {
            
            InitializeEpplusLicense();
        }
        private void InitializeEpplusLicense()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Анастасия");
        }
    }

}
