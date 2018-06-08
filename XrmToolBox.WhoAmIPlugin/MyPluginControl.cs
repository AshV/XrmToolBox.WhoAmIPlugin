using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;

namespace XrmToolBox.WhoAmIPlugin
{
    public partial class MyPluginControl : PluginControlBase
    {


        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
        }

        private void btn_WhoAmI_Click(object sender, EventArgs e)
        {
            // calling WhoAmI() from ExecuteMethod so connection will be smooth
            ExecuteMethod(WhoAmI);
        }

        private void WhoAmI()
        {
            WorkAsync(new WorkAsyncInfo
            {
                // Showing message until background work is completed
                Message = "Retrieving WhoAmI Information",

                // Main task which will be executed asynchronously
                Work = (worker, args) =>
                {
                    // making WhoAmIRequest
                    var whoAmIResponse = (WhoAmIResponse)Service.Execute(new WhoAmIRequest());

                    // retrieving details of current user
                    var user = Service.Retrieve("systemuser", whoAmIResponse.UserId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                    // placing results to args, which will be sent to PostWorkCallBack to display to user
                    var userData = new List<string>();
                    foreach (var data in user.Attributes)
                        userData.Add($"{data.Key} : {data.Value}");
                    args.Result = userData;
                },

                // Work is completed, results can be shown to user
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        lst_UserData.DataSource = args.Result;
                }
            });
        }
    }
}