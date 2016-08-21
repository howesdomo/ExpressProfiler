using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExpressProfiler
{
    public partial class FrmAddOrEditConn : Form
    {
        public ChangeDataBaseEventArgs c_Model { get; set; }

        public bool CanSave { get; set; }

        public FrmAddOrEditConn()
        {
            InitializeComponent();
            this.initEvent();
            this.initUI();
        }



        public FrmAddOrEditConn(ChangeDataBaseEventArgs args)
        {
            InitializeComponent();
            this.c_Model = args;
            this.initEvent();
            this.initUI();

            this.txtServerName .Text= args.ServerName;
            if (args.Authentication == "Windows Authentication")
            {
                this.cbAuthentication.SelectedIndex = 0;
            }
            else
            {
                this.cbAuthentication.SelectedIndex = 1;                
            }            
            this.txtUserName.Text = args.UserName;
            this.txtPassword.Text = args.Password;
            this.txtDatabase.Text = args.Database;
            this.txtAlias.Text = args.Alias;
        }


        private void initEvent()
        {
            this.txtServerName.TextChanged += txtInputs_TextChanged;
            this.cbAuthentication.SelectedIndexChanged += cbAuthentication_SelectedIndexChanged;
            this.txtUserName.TextChanged += txtInputs_TextChanged;
            this.txtPassword.TextChanged += txtInputs_TextChanged;
            this.txtDatabase.TextChanged += txtInputs_TextChanged;

            this.btnTest.Click += btnTest_Click;
            this.btnSave.Click += btnSave_Click;
        }

        private void initUI()
        {
            this.cbAuthentication.SelectedIndex = 0;
            this.MaximizeBox = false;
            this.bindUI_cbAuth();    
        }

        private void bindUI_cbAuth()
        {
            if (this.cbAuthentication.SelectedIndex == 0)
            {
                this.txtUserName.Text = string.Empty;
                this.txtUserName.Enabled = false;
                this.txtPassword.Text = string.Empty;
                this.txtPassword.Enabled = false;
            }
            else
            {
                this.txtUserName.Enabled = true;
                this.txtPassword.Enabled = true;
            }
        }

        void cbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.bindUI_cbAuth();
            if (this.btnSave.Enabled)
            {
                this.btnSave.Enabled = false;
            }
        }

        void txtInputs_TextChanged(object sender, EventArgs e)
        {
            this.CanSave = false;
            if (this.btnSave.Enabled)
            {
                this.btnSave.Enabled = false;
            }
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            this.CanSave = true;
            this.c_Model = this.getArgs();
            var q = MessageBox.Show(this, "保存并关闭本窗口?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (q == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
        }

        private ChangeDataBaseEventArgs getArgs()
        {
            ChangeDataBaseEventArgs args = new ChangeDataBaseEventArgs();
            args.ServerName = this.txtServerName.Text.Trim();
            args.Authentication = this.cbAuthentication.SelectedItem.ToString().Trim();
            args.UserName = this.txtUserName.Text.Trim();
            args.Password = this.txtPassword.Text.Trim();
            args.Database = this.txtDatabase.Text.Trim();
            args.Alias = this.txtAlias.Text.Trim();

            return args;
        }


        void btnTest_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ChangeDataBaseEventArgs args = this.getArgs();
            string errorMsg = this.testConn(args.ServerName, args.Authentication, args.UserName, args.Password, args.Database);
            if (string.IsNullOrEmpty(errorMsg))
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("测试成功。", "提示");
                this.btnSave.Enabled = true;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(errorMsg, "错误");
                this.btnSave.Enabled = false;
            }
        }


        private string testConn(string serverName, string authentication, string username, string password, string database)
        {
            string errorMsg = string.Empty;
            string connStr = string.Empty;
            if (authentication == "Windows Authentication")
            {
                connStr = string.Format(@"Data Source={0};Initial Catalog={1};Trusted_Connection=Yes;",
                        serverName, database, username, password
                        );
            }
            else
            {
                connStr = string.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};",
                        serverName, database, username, password
                        );
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return errorMsg;
        }

    }
}
