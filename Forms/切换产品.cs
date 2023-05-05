﻿using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WY_App.Utility;

namespace WY_App
{
    public partial class 切换产品 : Form
    {
        public delegate void TransfDelegate(String value);
        public event TransfDelegate TransfEvent;
        List<ProductKind> userList = new List<ProductKind>();
        class ProductKind
        {
            private string name;
            /// <summary>
            /// 用户名
            /// </summary>
            public string Name
            {
                get { return name; }
                set { name = value; }
            }           
        }
        public 切换产品()
        {
            InitializeComponent();
        }

        private void 切换产品_Load(object sender, EventArgs e)
        {
            //加载指定路径的xml文件
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true; //忽略文档里面的注释
            XmlReader reader = XmlReader.Create("Parameter/ProductList.xml");
            xmlDoc.Load(reader);
            //得到根节点
            XmlNode xn = xmlDoc.SelectSingleNode("Products");
            //得到根节点的所有子节点
            XmlNodeList xnl = xn.ChildNodes;

            foreach (XmlNode item in xnl)
            {
                ProductKind productKind = new ProductKind();
                //将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)item;
                //得到Name和Password两个属性的属性值
                XmlNodeList xmlnl = xe.ChildNodes;
                productKind.Name = xmlnl.Item(0).InnerText;
                cmb_ProductList.Items.Add(productKind.Name);                
                userList.Add(productKind);
            }
            cmb_ProductList.SelectedItem=Parameter.commministion.productName;
            reader.Close(); //读取完数据后需关闭

        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void 保存_Click(object sender, EventArgs e)
        {
            Parameter.commministion.productName = cmb_ProductList.Text;
            TransfEvent(Parameter.commministion.productName);
            XMLHelper.serialize<Parameter.Commministion>(Parameter.commministion, "Parameter/Commministion.xml");
            try
            {
                Parameter.specifications = XMLHelper.BackSerialize<Parameter.Specifications>(Parameter.commministion.productName + "/Specifications.xml");
            }
            catch
            {
                Parameter.specifications = new Parameter.Specifications();
                XMLHelper.serialize<Parameter.Specifications>(Parameter.specifications, Parameter.commministion.productName + "/Specifications.xml");
            }
            try
            {
                Parameter.counts = XMLHelper.BackSerialize<Parameter.Counts>(Parameter.commministion.productName + "/CountsParams.xml");
            }
            catch
            {
                Parameter.counts = new Parameter.Counts();
                XMLHelper.serialize<Parameter.Counts>(Parameter.counts, Parameter.commministion.productName + "/CountsParams.xml");
            }
            try
            {
                Parameter.cameraParam = XMLHelper.BackSerialize<Parameter.CameraParam>(Parameter.commministion.productName + "/CameraParam.xml");
            }
            catch
            {
                Parameter.cameraParam = new Parameter.CameraParam();
                XMLHelper.serialize<Parameter.CameraParam>(Parameter.cameraParam, Parameter.commministion.productName + "/CameraParam.xml");
            }

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmb_ProductList.Items.Contains(txt_NewProductName.Text))
            {
                MessageBox.Show("产品型号已存在，请勿重复创建！", "温馨提示");
                return;
            }
            string productName = txt_NewProductName.Text.Trim();
            //加载文件并选出根节点
            XmlDocument doc = new XmlDocument();
            doc.Load("Parameter/ProductList.xml");
            XmlNode root = doc.SelectSingleNode("Products");
            //创建一个结点，并设置结点的名称
            XmlElement xelKey = doc.CreateElement("Product");
            //创建子结点
            XmlElement xelUser = doc.CreateElement("Name");
            xelUser.InnerText = productName;
            //将子结点挂靠在相应的父节点
            xelKey.AppendChild(xelUser);
            //最后把book结点挂接在跟结点上，并保存整个文件
            root.AppendChild(xelKey);
            doc.Save("Parameter/ProductList.xml");
            GetFilesAndDirs(Parameter.commministion.productName, productName);
            MessageBox.Show("保存成功！", "温馨提示");
            this.Close();
        }
        private void GetFilesAndDirs(string srcDir, string destDir)
        {
            if (!Directory.Exists(destDir))//若目标文件夹不存在
            {
                string newPath;
                FileInfo fileInfo;
                Directory.CreateDirectory(destDir);//创建目标文件夹                                                  
                string[] files = Directory.GetFiles(srcDir);//获取源文件夹中的所有文件完整路径
                foreach (string path in files)          //遍历文件     
                {
                    fileInfo = new FileInfo(path);
                    newPath = destDir +"/"+ fileInfo.Name;
                    File.Copy(path, newPath, true);
                }
                string[] dirs = Directory.GetDirectories(srcDir);
                foreach (string path in dirs)        //遍历文件夹
                {
                    DirectoryInfo directory = new DirectoryInfo(path);
                    string newDir = destDir + "/" + directory.Name;
                    GetFilesAndDirs(path + "\\", newDir + "\\");
                }
            }
        }
    }
}
