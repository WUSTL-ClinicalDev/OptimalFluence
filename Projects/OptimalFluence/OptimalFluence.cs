using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Controls;
using OptimalFluence.View;
using OptimalFluence.ViewModel;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext scriptContext, System.Windows.Window mainWindow, ScriptEnvironment environment)
        {
            // TODO : Add here the code that is called when the script is launched from Eclipse.
            Run(scriptContext.CurrentUser,
        scriptContext.Patient,
        scriptContext.Image,
        scriptContext.StructureSet,
        scriptContext.PlanSetup,
        scriptContext.PlansInScope,
        scriptContext.PlanSumsInScope,
        mainWindow);
        }
        public void Run(
User user,
Patient patient,
Common.Model.API.Image image,
StructureSet structureSet,
PlanSetup planSetup,
IEnumerable<PlanSetup> planSetupsInScope,
IEnumerable<PlanSum> planSumsInScope,
Window mainWindow)
        {
            // Your main code now goes here

            UserControl optimalFluenceView = new OptimalFluenceView();
            optimalFluenceView.DataContext = new OptimalFluenceViewModel(planSetup.Beams);
            mainWindow.Width = 400;
            mainWindow.Height = 600;
            mainWindow.Content = optimalFluenceView;

        }
    }
}
