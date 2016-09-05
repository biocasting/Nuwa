using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using System.IO;

namespace Nuwa
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 获取程序运行时的相对路径
        /// </summary>
        /// <returns></returns>
            public static string getPrjPath()
            {
            return Application.StartupPath + "\\"+"CommConfig"+"\\";
            }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        public void loadmessage()
        {
            string ConfigPath = getPrjPath() + "config.xml";
            bool isExit = System.IO.File.Exists(ConfigPath);
            if (isExit == true)//文件存在这读取配置信息
            {
                ReadXML(ConfigPath);
            }
            else
            {
                CreateXML(ConfigPath);//文件不存在则创建一个XML文档
            }
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="ConfigPath"></param>
        public void ReadXML(string ConfigPath)
        {
            cXmlIO XmlConfig = new cXmlIO(ConfigPath);
            DataView DV = new DataView();
            DV = XmlConfig.GetData("descendant::config");
            {
                if (DV == null)
                {
                    return;
                }
                else
                {
                    this.skinTextBox1.Text = DV[0].Row["test1"].ToString();
                    this.skinTextBox2.Text = DV[0].Row["test2"].ToString();
                    this.skinTextBox3.Text = DV[0].Row["test3"].ToString();
                    this.skinTextBox4.Text = DV[0].Row["test4"].ToString();
                }
            }
        }

        /// <summary>
        /// 建立一个固定格式的XML文档
        /// </summary>
        /// <param name="XMLpath">路径</param>
        public void CreateXML(string XMLpath)
        {
            cXmlIO xmlConfig = new cXmlIO();
            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<CommConfig>" + "</CommConfig>";
            xmlConfig.NewXmlFile(XMLpath, strXml);
            xmlConfig = null;
        }

        /// <summary>
        /// 插入配置信息
        /// </summary>
        /// <param name="ConfigPath"></param>
        public void CreateConfig(string ConfigPath)
        {
            cXmlIO XMLconfig = new cXmlIO(ConfigPath);
            XMLconfig.DeleteNode("config");
            string strXML = "<test1>" + this.skinTextBox1.Text.Trim() + "</test1>" +
                "<test2>" + this.skinTextBox2.Text.Trim() + "</test2>" +
                "<test3>" + this.skinTextBox3.Text.Trim() + "</test3>" +
                "<test4>" + this.skinTextBox4.Text.Trim() + "</test4>";
            XMLconfig.InsertElement("CommConfig","config", strXML);
            XMLconfig.Save();
            XMLconfig = null;
            MessageBox.Show("保存成功", "百廷三维 系统信息", MessageBoxButtons.OK);

        }
        /// <summary>
        /// 修改保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinButton1_Click(object sender, EventArgs e)
        {
            string configPath= getPrjPath() + "config.xml";
            if (!System.IO.File.Exists(configPath))
            {
                DialogResult dr;
                dr = MessageBox.Show("配置文件尚未建立，是否现在创建？", "百廷三维 系统信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.OK)
                {
                    CreateXML(configPath);
                    CreateConfig(configPath);
                }
                else
                {
                    return;
                }
            }
            else
            {
                CreateConfig(configPath);
            }
               
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
