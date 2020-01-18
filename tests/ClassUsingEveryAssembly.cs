#if NET35
extern alias net20;
extern alias net35;
#endif

extern alias aero;
extern alias classic;
extern alias luna;
extern alias royale;

#pragma warning disable CS0618, IDE0049 // Type or member is obsolete, Name can be simplified

// ReSharper disable BuiltInTypeReferenceStyle, UnusedMember.Global, CommentTypo

public static class ClassUsingEveryAssembly
{
    public static System.Type[] TypeFromEachAssembly =
    {
        typeof(Accessibility.IAccessible), // Accessibility
#if NET35
        typeof(Microsoft.Aspnet.Snapin.IAspNetManagementUtility), // AspNetMMCExt
        typeof(Microsoft.CSharp.Compiler), // cscompmgd
#endif
        typeof(System.Runtime.InteropServices.CustomMarshalers.EnumerableToDispatchMarshaler), // CustomMarshalers
#if NET35
        typeof(IEHost.Execute.IEExecuteRemote), // IEExecRemote
        typeof(Microsoft.IE.Manager), // IEHost
        typeof(Microsoft.IE.IHostEx), // IIEHost
#endif
        typeof(System.Diagnostics.SymbolStore.SymDocument), // ISymWrapper
#if NET35
        typeof(Microsoft.Build.Conversion.ProjectFileConverter), // Microsoft.Build.Conversion.v3.5
#endif
        typeof(Microsoft.Build.BuildEngine.Engine), // Microsoft.Build.Engine
        typeof(Microsoft.Build.Framework.ITask), // Microsoft.Build.Framework
#if NET35
        typeof(net20::Microsoft.Build.Tasks.Exec), // Microsoft.Build.Tasks
        typeof(net35::Microsoft.Build.Tasks.Exec), // Microsoft.Build.Tasks.v3.5
        typeof(net20::Microsoft.Build.Utilities.Task), // Microsoft.Build.Utilities
        typeof(net35::Microsoft.Build.Utilities.Task), // Microsoft.Build.Utilities.v3.5
        typeof(Microsoft.Data.Entity.Build.Tasks.EntityDeploy), // Microsoft.Data.Entity.Build.Tasks
#endif
        typeof(Microsoft.JScript.JSObject), // Microsoft.JScript
        typeof(Microsoft.VisualBasic.Compatibility.VB6.IDataFormat), // Microsoft.VisualBasic.Compatibility.Data
        typeof(Microsoft.VisualBasic.Compatibility.VB6.ButtonArray), // Microsoft.VisualBasic.Compatibility
        typeof(Microsoft.VisualBasic.Constants), // Microsoft.VisualBasic
#if NET35
        typeof(Microsoft.VisualBasic.Vsa.VsaEngine), // Microsoft.VisualBasic.Vsa
#endif
        typeof(Microsoft.VisualC.IsConstModifier), // Microsoft.VisualC
        typeof(Microsoft.VisualC.StlClr.IList<>), // Microsoft.VisualC.STLCLR
#if NET35
        typeof(Microsoft.Vsa.IVsaEngine), // Microsoft.Vsa
        typeof(Microsoft.Vsa.Vb.CodeDOM.CodeDOMProcessor), // Microsoft.Vsa.Vb.CodeDOMProcessor
        typeof(Microsoft_VsaVb.VsaEngine), // Microsoft_VsaVb
#endif
        typeof(System.Object), // mscorlib
        typeof(Microsoft.Build.Tasks.Windows.ResourcesGenerator), // PresentationBuildTasks
        typeof(System.Windows.ContentElement), // PresentationCore
        typeof(aero::Microsoft.Windows.Themes.SystemDropShadowChrome), // PresentationFramework.Aero
        typeof(classic::Microsoft.Windows.Themes.SystemDropShadowChrome), // PresentationFramework.Classic
        typeof(System.Windows.Application), // PresentationFramework
        typeof(luna::Microsoft.Windows.Themes.SystemDropShadowChrome), // PresentationFramework.Luna
        typeof(royale::Microsoft.Windows.Themes.SystemDropShadowChrome), // PresentationFramework.Royale
        typeof(System.Windows.Xps.Packaging.XpsDocument), // ReachFramework
        // Sentinel.v3.5Client has no public types
        typeof(System.Globalization.CultureAndRegionInfoBuilder), // sysglobl
        typeof(System.AddIn.Contract.IContract), // System.AddIn.Contract
        typeof(System.AddIn.AddInAttribute), // System.AddIn
        typeof(System.ComponentModel.DataAnnotations.DataTypeAttribute), // System.ComponentModel.DataAnnotations
        typeof(System.Configuration.Configuration), // System.Configuration
        typeof(System.Configuration.Install.Installer), // System.Configuration.Install
        typeof(System.Action), // System.Core
        typeof(System.Data.DataTableExtensions), // System.Data.DataSetExtensions
        typeof(System.Data.DataSet), // System.Data
        typeof(System.Data.Entity.Design.EntityClassGenerator), // System.Data.Entity.Design
        typeof(System.Data.EntityClient.EntityProviderFactory), // System.Data.Entity
        typeof(System.Data.Linq.ITable), // System.Data.Linq
        typeof(System.Data.OracleClient.OracleClientFactory), // System.Data.OracleClient
        typeof(System.Data.Services.Client.DataServiceQuery), // System.Data.Services.Client
        typeof(System.Data.Services.Design.EntityClassGenerator), // System.Data.Services.Design
        typeof(System.Data.Services.DataService<>), // System.Data.Services
        typeof(System.Xml.Xsl.Runtime.XmlQueryRuntime), // System.Data.SqlXml
        typeof(System.Deployment.Application.ApplicationDeployment), // System.Deployment
        typeof(System.ComponentModel.Design.ComponentDesigner), // System.Design
        typeof(System.DirectoryServices.AccountManagement.Principal), // System.DirectoryServices.AccountManagement
        typeof(System.DirectoryServices.DirectoryEntry), // System.DirectoryServices
        typeof(System.DirectoryServices.Protocols.DirectoryOperation), // System.DirectoryServices.Protocols
        typeof(System.Uri), // System
        typeof(System.Drawing.Design.ColorEditor), // System.Drawing.Design
        typeof(System.Drawing.Graphics), // System.Drawing
        // System.EnterpriseServices cannot be referenced without requiring System.EnterpriseServices.Wrapper.
        typeof(System.IdentityModel.Claims.Claim), // System.IdentityModel
        typeof(System.IdentityModel.Selectors.CardSpaceSelector), // System.IdentityModel.Selectors
        typeof(System.IO.Log.LogStore), // System.IO.Log
        typeof(System.Management.ManagementObject), // System.Management
        typeof(System.Management.Instrumentation.InstrumentationManager), // System.Management.Instrumentation
        typeof(System.Messaging.Message), // System.Messaging
        typeof(System.Net.IPEndPointCollection), // System.Net
        typeof(System.Printing.PrintSystemObjects), // System.Printing
        typeof(System.Runtime.Remoting.Services.RemotingService), // System.Runtime.Remoting
        typeof(System.Runtime.Serialization.DataContractSerializer), // System.Runtime.Serialization
        typeof(System.Runtime.Serialization.Formatters.Soap.SoapFormatter), // System.Runtime.Serialization.Formatters.Soap
        typeof(System.Security.Cryptography.ProtectedData), // System.Security
        typeof(System.ServiceModel.ServiceHost), // System.ServiceModel
        typeof(System.ServiceModel.Web.WebServiceHost), // System.ServiceModel.Web
        typeof(System.ServiceProcess.ServiceBase), // System.ServiceProcess
        typeof(System.Speech.Synthesis.SpeechSynthesizer), // System.Speech
        typeof(System.Transactions.Transaction), // System.Transactions
        typeof(System.Web.HttpContextBase), // System.Web.Abstractions
        typeof(System.Web.HttpContext), // System.Web
        typeof(System.Web.DynamicData.Design.DynamicDataManagerDesigner), // System.Web.DynamicData.Design
        typeof(System.Web.DynamicData.DynamicDataManager), // System.Web.DynamicData
        typeof(System.Web.UI.Design.WebControls.EntityDataSourceDesigner), // System.Web.Entity.Design
        typeof(System.Web.UI.WebControls.EntityDataSource), // System.Web.Entity
        typeof(System.Web.UI.Design.ExtenderControlDesigner), // System.Web.Extensions.Design
        typeof(System.Web.UI.ExtenderControl), // System.Web.Extensions
        typeof(System.Web.Mobile.MobileCapabilities), // System.Web.Mobile
        typeof(System.Web.RegularExpressions.AspCodeRegex), // System.Web.RegularExpressions
        typeof(System.Web.Routing.Route), // System.Web.Routing
        typeof(System.Web.Services.WebService), // System.Web.Services
        typeof(System.Windows.Forms.Form),// System.Windows.Forms
        typeof(System.Windows.Threading.DispatcherExtensions), // System.Windows.Presentation
        typeof(System.Workflow.Activities.IEventActivity), // System.Workflow.Activities
        typeof(System.Workflow.ComponentModel.Activity), // System.Workflow.ComponentModel
        typeof(System.Workflow.Runtime.WorkflowRuntime), // System.Workflow.Runtime
        typeof(System.ServiceModel.WorkflowServiceHost), // System.WorkflowServices
        typeof(System.Xml.XmlDocument), // System.Xml
        typeof(System.Xml.Linq.XDocument), // System.Xml.Linq
        typeof(System.Windows.Automation.Automation), // UIAutomationClient
        typeof(UIAutomationClientsideProviders.UIAutomationClientSideProviders), // UIAutomationClientsideProviders
        typeof(System.Windows.Automation.Provider.AutomationInteropProvider), // UIAutomationProvider
        typeof(System.Windows.Automation.AutomationIdentifier), // UIAutomationTypes
        typeof(System.Windows.DependencyObject), // WindowsBase
        typeof(System.Windows.Forms.Integration.WindowsFormsHost), // WindowsFormsIntegration
    };
}
