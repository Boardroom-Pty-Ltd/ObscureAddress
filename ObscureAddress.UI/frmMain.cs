using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace ObscureAddress.UI
{
    public partial class frmMain : Form
    {
        //string filePath = @"D:\Temp\data.txt";
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //if (!File.Exists(filePath))
            //    File.Create(filePath).Close();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            var obj = JsonConvert.DeserializeObject<InvestorData>(txtRequest.Text);
            //var response = RandomDataGenerateHelper.Transform(filePath, obj);
            var response = DataObfuscator.Transform(obj);
            txtResponse.Text = string.Empty;
            txtResponse.Text = JsonConvert.SerializeObject(response);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRequest.Text = txtResponse.Text = string.Empty;
        }
    }
}