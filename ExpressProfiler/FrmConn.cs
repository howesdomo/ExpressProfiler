using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ExpressProfiler
{
    public partial class FrmConn : Form
    {
        private Dal dal = new Dal();

        private List<ChangeDataBaseEventArgs> result { get; set; }

        public FrmConn()
        {
            InitializeComponent();
            this.initUI();
            this.result = new List<ChangeDataBaseEventArgs>();

            this.initEvent();
            this.initData();
        }

        #region initUI

        private void initUI()
        {
            this.Text = "连接到服务器";
        }

        #endregion

        private void initData()
        {
            this.result = this.dal.GetAll(Program.FullName);
            this.bindUI_treeView();
        }

        private void bindUI_treeView()
        {
            treeView1.Nodes.Clear();

            TreeNode root = new TreeNode();
            root.Text = "SQL Server";

            foreach (ChangeDataBaseEventArgs item in result.OrderBy(i => i.Alias))
            {
                TreeNode tmp = new TreeNode();
                tmp.Tag = item;

                if (string.IsNullOrEmpty(item.Alias))
                {
                    tmp.Text = string.Format("{0}, {2}({1})",
                        item.ServerName,
                        item.Authentication == "Windows Authentication" ? "Windows验证" : item.UserName,
                        item.Database
                        );
                }
                else
                {
                    tmp.Text = string.Format("{0}, {2}({1})",
                        item.Alias,
                        item.Authentication == "Windows Authentication" ? "Windows验证" : item.UserName,
                        item.Database
                        );
                }

                if (string.IsNullOrEmpty(item.Alias) == false)
                {
                    if (item.Alias.Contains("正式")) 
                    {
                        tmp.BackColor = Color.FromArgb(252, 162, 131);
                    }
                }

                root.Nodes.Add(tmp);
            }

            root.Expand();
            treeView1.Nodes.Add(root);
        }

        private void initEvent()
        {
            this.btnAdd.Click += btnAdd_Click;
            this.btnEdit.Click += btnEdit_Click;
            this.btnDel.Click += btnDel_Click;
            this.treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;

        }



        void btnAdd_Click(object sender, EventArgs e)
        {
            FrmAddOrEditConn frm = new FrmAddOrEditConn();
            frm.Owner = this;
            frm.Text = "Add Conn";
            frm.ShowDialog();

            if (frm.c_Model != null)
            {
                result.Add(frm.c_Model);
                dal.Save(result, Program.FullName);
            }
            this.bindUI_treeView();
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is ChangeDataBaseEventArgs)
            {
                ChangeDataBaseEventArgs toEdit = treeView1.SelectedNode.Tag as ChangeDataBaseEventArgs;
                FrmAddOrEditConn frm = new FrmAddOrEditConn(toEdit);
                frm.Owner = this;
                frm.Text = "Edit Conn";
                frm.ShowDialog();

                if (frm.c_Model != null)
                {
                    result.Remove(toEdit);
                    result.Add(frm.c_Model);
                    dal.Save(result, Program.FullName);
                }
                dal.Save(result, Program.FullName);
                this.bindUI_treeView();
            }
            else
            {
                MessageBox.Show("选择需要编辑的Conn", "错误");
            }
        }

        void btnDel_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is ChangeDataBaseEventArgs)
            {
                var q = MessageBox.Show(this, "确认删除?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (q == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
                ChangeDataBaseEventArgs toDel = treeView1.SelectedNode.Tag as ChangeDataBaseEventArgs;
                result.Remove(toDel);
                dal.Save(result, Program.FullName);
                this.bindUI_treeView();
            }
            else
            {
                MessageBox.Show("选择需要删除的Conn", "错误");
            }
        }

        void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode tn = e.Node;
            object treeNodeTag = tn.Tag;
            if (treeNodeTag != null && treeNodeTag is ChangeDataBaseEventArgs)
            {
                this.OnChangeDataBase(treeNodeTag as ChangeDataBaseEventArgs);
            }

        }

        public event EventHandler<ChangeDataBaseEventArgs> ChangeDataBase;

        private void OnChangeDataBase(ChangeDataBaseEventArgs args)
        {
            if (this.ChangeDataBase != null)
            {
                this.ChangeDataBase.Invoke(this, args);
            }
        }


    }

    public class Dal
    {
        public List<ChangeDataBaseEventArgs> GetAll(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    List<ChangeDataBaseEventArgs> result = Howe.Helper.SerializationHelper.Load(typeof(List<ChangeDataBaseEventArgs>), path) as List<ChangeDataBaseEventArgs>;
                    foreach (ChangeDataBaseEventArgs item in result)
                    {
                        item.Password = Howe.Util.SecurityUtil.Decode(item.Password);
                    }
                    return result;
                }
                else
                {
                    return new List<ChangeDataBaseEventArgs>();
                }
            }
            catch (Exception)
            {
                return new List<ChangeDataBaseEventArgs>();
            }
        }

        public void Save(List<ChangeDataBaseEventArgs> toSave, string path)
        {
            List<ChangeDataBaseEventArgs> final = new List<ChangeDataBaseEventArgs>();
            foreach (ChangeDataBaseEventArgs item in toSave)
            {
                ChangeDataBaseEventArgs tmp = new ChangeDataBaseEventArgs();
                tmp.Authentication = item.Authentication;
                tmp.ServerName = item.ServerName;
                tmp.UserName = item.UserName;
                tmp.Password = Howe.Util.SecurityUtil.Encode(item.Password);
                tmp.Database = item.Database;
                tmp.Alias = item.Alias;
                final.Add(tmp);
            }
            Howe.Helper.SerializationHelper.Save(final, path);
        }
    }

    [Serializable]
    public class ChangeDataBaseEventArgs : EventArgs
    {
        public ChangeDataBaseEventArgs()
        {

        }

        public string Alias { get; set; }

        public string ServerName { get; set; }

        public string Authentication { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }
    }
}
