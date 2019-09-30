using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpToFuWuQi
{
    public partial class UpAndRead : Form
    {
        public UpAndRead()
        {
            InitializeComponent();
        }
        public byte[] imageBytes;
        string name;
        public DbHelper.SqlHelper helper;
        private void Select_Click(object sender, EventArgs e)
        {
            OpenFileDialog openF = new OpenFileDialog();

            //获取用户打开的路径然转换成二进制存入数据库
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*jpg|*.*|*.GIF|*.GIF|*.JPG|*.BMP";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;//路径
                FileStream fs = new FileStream(filePath, FileMode.Open);
                imageBytes = new byte[fs.Length];
                BinaryReader br = new BinaryReader(fs);
                imageBytes = br.ReadBytes(Convert.ToInt32(fs.Length));//转换成二进制流
                name = ofd.FileName.Split('\\')[ofd.FileName.Split('\\').Length - 1];
                label1.Text = name;
                
                
            }
        }

        private void Up_Click(object sender, EventArgs e)
        {
            string strSql = $"insert into A0_RIGHTMODEL(GUID,CODE,NAME) values('{name}','" + imageBytes + "','name')";
            string result =  helper.ExecSql(strSql);
            MessageBox.Show(result);
        }
        

        private void UpAndRead_Load(object sender, EventArgs e)
        {
            helper = new DbHelper.SqlHelper("Oracle", "User Id=dbo;Password=romens;Data Source=192.168.100.9:1521/NewStddata;");
        }

        private void DownLoad_Click(object sender, EventArgs e)
        {
            imageBytes = null;

            //打开数据库


            string strSql = "select GUID,CODE,NAME from A0_RIGHTMODEL where name = '{name}'";
            DataTable ds =  helper.GetDataTable(strSql);
            imageBytes = (byte[])ds.Rows[0]["CODE"];
            MemoryStream ms = new MemoryStream(imageBytes);

            Bitmap bmpt = new Bitmap(ms);

            pictureBox1.Image = bmpt;
        }
    }
}
