using EclipsePlugInRunner.Scripting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS;
using VMS.TPS.Common.Model.API;

namespace OptimalFluence.Runner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            PlanSetup ps;
            ScriptRunner.Run(new Script());
        }
    }
}
