using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Agent;
using DataManagement;
using DijkstraClass;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;


namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class FrmSetEmergencyResourceDemand : Form
    {
        IFeatureLayer pFeatureLayerEmergency = null;
        ComboBox[] combobox;
        Label[] label;
        TextBox[] textbox;
        public FrmSetEmergencyResourceDemand()
        {
            InitializeComponent();
        }

        //改变应急处置空间位置，获取其信息
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Enabled == false)
                {
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    textBox6.Enabled = true;
                    textBox7.Enabled = true;
                }
                else
                {

                    groupBox2.Controls.Clear();
                    textBox7.Text = "";
                    groupBox2.Controls.Add(label5);
                    label5.Enabled = false;
                    groupBox2.Controls.Add(label6);
                    label6.Enabled = false;
                    groupBox2.Controls.Add(comboBox3);
                    comboBox3.Enabled = false;
                    groupBox2.Controls.Add(label3);
                    label3.Enabled = false;
                    groupBox2.Controls.Add(textBox1);
                    textBox1.Enabled = false;

                }
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox2.Text) - 1);
                IFeatureClass pFeatureClassEmergency = pFeatureLayerEmergency.FeatureClass;

                IFeatureCursor pFeatureCursorEmergency = pFeatureClassEmergency.Search(pQueryFilter, false);
                IFeature pFeatureEmergency = pFeatureCursorEmergency.NextFeature();
                while (pFeatureEmergency != null)
                {
                    string V = Convert.ToString(pFeatureEmergency.get_Value(pFeatureEmergency.Fields.FindField("V")));
                    string Q = Convert.ToString(pFeatureEmergency.get_Value(pFeatureEmergency.Fields.FindField("Q")));
                    string L = Convert.ToString(pFeatureEmergency.get_Value(pFeatureEmergency.Fields.FindField("L")));
                    string C = Convert.ToString(pFeatureEmergency.get_Value(pFeatureEmergency.Fields.FindField("C")));
                    string S = Convert.ToString(pFeatureEmergency.get_Value(pFeatureEmergency.Fields.FindField("S")));
                    textBox2.Text = V;
                    textBox3.Text = Q;
                    textBox4.Text = L;
                    textBox5.Text = C;
                    textBox6.Text = S;
                    pFeatureEmergency = pFeatureCursorEmergency.NextFeature();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void FrmSetEmergencyResourceDemand_Load(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            label2.Enabled = false;
            label3.Enabled = false;
            label5.Enabled = false;
            label6.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            
            
        }

        //根据应急资源需求类型，创建设置资源量的textbox
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text == "")
                {
                    MessageBox.Show("请资源需求类型", "输入资源需求类型", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    comboBox3.Items.Clear();
                    pFeatureLayerEmergency = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    //读取输入应急处置点图层的字段
                    IFeatureClass pFeatureClassField = pFeatureLayerEmergency.FeatureClass;
                    IFields pFields = pFeatureClassField.Fields;
                    IField pField;
                    for (int j = 0; j < pFields.FieldCount; j++)
                    {
                        pField = pFields.get_Field(j);
                        comboBox3.Items.Add(pField.Name.ToString());
                    }

                    textBox1.Enabled = true;
                    label3.Enabled = true;
                    label5.Enabled = true;
                    label6.Enabled = true;
                    comboBox3.Enabled = true;
                    int number = Convert.ToInt32(textBox7.Text) - 1;
                    combobox = new ComboBox[number];
                    label = new Label[number];
                    textbox = new TextBox[number];
                    for (int i = 0; i < combobox.Length; i++)
                    {
                        if (i < 2)
                        {
                            combobox[i] = new ComboBox();
                            combobox[i].Size = comboBox3.Size;
                            combobox[i].DropDownStyle = comboBox3.DropDownStyle;
                            combobox[i].Location = new System.Drawing.Point(comboBox3.Location.X, comboBox3.Location.Y + (i + 1) * 25);
                            for (int k = 0; k < pFields.FieldCount; k++)
                            {
                                pField = pFields.get_Field(k);
                                combobox[i].Items.Add(pField.Name.ToString());
                            }
                            groupBox2.Controls.Add(combobox[i]);
                            label[i] = new Label();
                            label[i].Size = label3.Size;
                            label[i].Text = "：";
                            label[i].Location = new System.Drawing.Point(label3.Location.X, label3.Location.Y + (i + 1) * 25);
                            groupBox2.Controls.Add(label[i]);
                            textbox[i] = new TextBox();
                            textbox[i].Size = textBox1.Size;
                            textbox[i].Location = new System.Drawing.Point(textBox1.Location.X, textBox1.Location.Y + (i + 1) * 25);
                            groupBox2.Controls.Add(textbox[i]);

                        }
                        else if (1 < i && i < 5)
                        {
                            if (i == 2)
                            {
                                Label pLabel1 = new Label();
                                pLabel1.Size = label5.Size;
                                pLabel1.Text = label5.Text;
                                pLabel1.Location = new System.Drawing.Point(label5.Location.X + 250, label5.Location.Y);
                                groupBox2.Controls.Add(pLabel1);
                                Label pLabel2 = new Label();
                                pLabel2.Size = label6.Size;
                                pLabel2.Text = label6.Text;
                                pLabel2.Location = new System.Drawing.Point(label6.Location.X + 250, label6.Location.Y);
                                groupBox2.Controls.Add(pLabel2);
                            }

                            combobox[i] = new ComboBox();
                            combobox[i].Size = comboBox3.Size;
                            combobox[i].DropDownStyle = comboBox3.DropDownStyle;
                            combobox[i].Location = new System.Drawing.Point(comboBox3.Location.X + 250, comboBox3.Location.Y + (i - 2) * 25);
                            for (int k = 0; k < pFields.FieldCount; k++)
                            {
                                pField = pFields.get_Field(k);
                                combobox[i].Items.Add(pField.Name.ToString());
                            }
                            groupBox2.Controls.Add(combobox[i]);
                            label[i] = new Label();
                            label[i].Size = label3.Size;
                            label[i].Text = "：";
                            label[i].Location = new System.Drawing.Point(label3.Location.X + 250, label3.Location.Y + (i - 2) * 25);
                            groupBox2.Controls.Add(label[i]);
                            textbox[i] = new TextBox();
                            textbox[i].Size = textBox1.Size;
                            textbox[i].Location = new System.Drawing.Point(textBox1.Location.X + 250, textBox1.Location.Y + (i - 2) * 25);
                            groupBox2.Controls.Add(textbox[i]);

                        }

                    }
                    comboBox3.SelectedIndex = 0;
                    for (int l = 0; l < number; l++)
                    {
                        combobox[l].SelectedIndex = 0;
                        combobox[l].SelectedIndexChanged += new EventHandler(FrmSetEmergencyResourceDemand_SelectedIndexChanged);
                    }

                    IFeatureClass pFeatureClassIndex = pFeatureLayerEmergency.FeatureClass;
                    IQueryFilter pQueryFilterIndex = new QueryFilterClass();
                    pQueryFilterIndex.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox2.Text) - 1);
                    IFeatureCursor pFeatureCursorIndex = pFeatureClassIndex.Search(pQueryFilterIndex, false);
                    IFeature pFeatureIndex = pFeatureCursorIndex.NextFeature();
                    while (pFeatureIndex != null)
                    {
                        for (int i = 0; i < combobox.Length; i++)
                        {
                            textbox[i].Text = Convert.ToString(pFeatureIndex.get_Value(pFeatureIndex.Fields.FindField(combobox[i].Text)));
                        }
                        pFeatureIndex = pFeatureCursorIndex.NextFeature();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }  
        }

        //选择应急资源种类，使得其值也发生变化
        private void FrmSetEmergencyResourceDemand_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                IFeatureClass pFeatureClassIndex = pFeatureLayerEmergency.FeatureClass;
                IQueryFilter pQueryFilterIndex = new QueryFilterClass();
                pQueryFilterIndex.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox2.Text) - 1);           
                IFeatureCursor pFeatureCursorIndex = pFeatureClassIndex.Search(pQueryFilterIndex, false);     
                IFeature pFeatureIndex = pFeatureCursorIndex.NextFeature();   
                while (pFeatureIndex != null)           
                {               
                    for (int i = 0; i < combobox.Length; i++)           
                    {              
                        textbox[i].Text = Convert.ToString(pFeatureIndex.get_Value(pFeatureIndex.Fields.FindField(combobox[i].Text)));              
                    }               
                    pFeatureIndex = pFeatureCursorIndex.NextFeature();                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                IFeatureClass pFeatureClassSave = pFeatureLayerEmergency.FeatureClass;           
                IQueryFilter pQueryFilterSave = new QueryFilterClass();            
                pQueryFilterSave.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox2.Text) - 1);         
                IFeatureCursor pFeatureCursorSave = pFeatureClassSave.Update(pQueryFilterSave, false);                      
                IFeature pFeatureSave = pFeatureCursorSave.NextFeature();         
                int number=Convert.ToInt32(textBox7.Text);          
                int[] EmergencyResource = new int[number];           
                EmergencyResource[0] = pFeatureCursorSave.FindField(comboBox3.Text.ToString());          
                for (int i = 1; i < number; i++)         
                {           
                    EmergencyResource[i] = pFeatureCursorSave.FindField(combobox[i - 1].Text.ToString());          
                }         
                while (pFeatureSave != null)           
                {               
                    pFeatureSave.set_Value(EmergencyResource[0], double.Parse(textBox1.Text));              
                    for (int j = 1; j < number; j++)              
                    {                
                        pFeatureSave.set_Value(EmergencyResource[j], double.Parse(textbox[j - 1].Text));            
                    }             
                    pFeatureCursorSave.UpdateFeature(pFeatureSave);              
                    pFeatureSave = pFeatureCursorSave.NextFeature();                           
                }     
                MessageBox.Show("保存完成！");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message+"\n"+ex.Message.ToString(),"异常");
            }
            
        }

        //取消该功能
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            //IsNumber的作用是判断输入按键是否为数字
            //(char)8是退格键的键值，可允许用户敲退格键对输入的数字进行修改
            //(char)46是小数点的键值，允许用户输入小数
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
            {
                e.Handled = true;//经过判断为数字，可以输入
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入备选应急处置空间位置数据";
                openDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                openDialog.Multiselect = false;
                openDialog.RestoreDirectory = false;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox1.Text = openDialog.FileName.ToString();
                }
                else
                {
                    return;
                }
                label4.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label4.Visible = false;
            }
        }

        //获取应急处置空间位置的FID
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入应急处置空间备选数据", "输入应急处置空间备选数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    label2.Enabled = true;
                    comboBox2.Enabled = true;
                    string currentFieldName = "FID";
                    pFeatureLayerEmergency = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureClass pFeatureClassEmergency = pFeatureLayerEmergency.FeatureClass;
                    IFeatureCursor pFeatureCursorEmergency = pFeatureClassEmergency.Search(null, false);
                    IDataStatistics pDataStatistics = new DataStatistics();
                    pDataStatistics.Field = currentFieldName;
                    pDataStatistics.Cursor = pFeatureCursorEmergency as ICursor;
                    System.Collections.IEnumerator pEnumerator = pDataStatistics.UniqueValues;
                    while (pEnumerator.MoveNext())
                    {
                        comboBox2.Items.Add(Convert.ToInt32(pEnumerator.Current.ToString()) + 1);
                    }

                    comboBox2.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
          
            
        }

        //获取应急资源物资的种类
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                IFeatureClass pFeatureClassIndex = pFeatureLayerEmergency.FeatureClass;
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox2.Text) - 1);
                IFeatureCursor pFeatureCursorIndex = pFeatureClassIndex.Search(pQueryFilter, false);
                IFeature pFeatureIndex = pFeatureCursorIndex.NextFeature();
                while (pFeatureIndex != null)
                {
                    textBox1.Text = Convert.ToString(pFeatureIndex.get_Value(pFeatureIndex.Fields.FindField(comboBox3.Text)));
                    pFeatureIndex = pFeatureCursorIndex.NextFeature();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }
    }
}
