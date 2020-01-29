using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using DataManagement;
using EmergencyResourceSchedule;
using DijkstraClass;


namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class FrmEmergencyDispositionPointInformation : Form
    {
        public FrmEmergencyDispositionPointInformation()
        {
            InitializeComponent();
        }

        private void FrmEmergencyDispositionPointInformation_Load(object sender, EventArgs e)
        {
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            button7.Enabled = false;
            button9.Enabled = false;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "请输入可供选择的应急处置空间位置的数据：";
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
                label2.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label2.Visible = false;
            }
        }

        //通过模拟数据获取应急处置空间位置的信息
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    groupBox2.Enabled = true;
                    button7.Enabled = true;
                }
                else
                {
                    groupBox2.Enabled = false;
                    button7.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }

            
        }

        //通过实时监测的数据获取各个应急处置空间位置的信息
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox2.Checked == true)
                {
                    groupBox3.Enabled = true;
                    button9.Enabled = true;

                    //将应急处置点序号添加进去
                    string currentFieldName = "FID";
                    IFeatureLayer pFeatureLayerEmergencypoint = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureClass pFeatureClassEmergencypoint = pFeatureLayerEmergencypoint.FeatureClass;
                    IFeatureCursor pFeatureCursorEmergencypoint = pFeatureClassEmergencypoint.Search(null, false);
                    IDataStatistics pDataStatisticsEmergencypoint = new DataStatistics();
                    pDataStatisticsEmergencypoint.Field = currentFieldName;
                    pDataStatisticsEmergencypoint.Cursor = pFeatureCursorEmergencypoint as ICursor;
                    System.Collections.IEnumerator pEnumerator = pDataStatisticsEmergencypoint.UniqueValues;
                    while (pEnumerator.MoveNext())
                    {
                        comboBox7.Items.Add(Convert.ToInt32(pEnumerator.Current.ToString()) + 1);
                    }
                    comboBox7.SelectedIndex = 0;
                }
                else
                {
                    groupBox3.Enabled = false;
                    button9.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "IMAGINE Image(*.img)|*.img";
                openFileDialog.Title = "请输入河流流速的数据：";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox2.Text = openFileDialog.FileName;
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

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "IMAGINE Image(*.img)|*.img";
                openFileDialog.Title = "请输入河流流量的数据：";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox3.Text = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
                label6.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label6.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "IMAGINE Image(*.img)|*.img";
                openFileDialog.Title = "请输入河流宽度的数据：";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox4.Text = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
                label8.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label8.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "IMAGINE Image(*.img)|*.img";
                openFileDialog.Title = "请输入河流污染物浓度的数据：";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox5.Text = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
                label10.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label10.Visible = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "IMAGINE Image(*.img)|*.img";
                openFileDialog.Title = "请输入河流污染物总量的数据：";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox6.Text = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
                label12.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label12.Visible = false;
            }
        }

        //保存应急处置空间位置的信息
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入可供选择的应急处置空间位置的数据", "输入可供选择的应急处置空间位置的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入河流流速的数据", "输入河流流速的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请输入河流流量的数据", "输入河流流量的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox4.Text == "")
                {
                    MessageBox.Show("请输入河流宽度的数据", "输入河流宽度的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox5.Text == "")
                {
                    MessageBox.Show("请输入河流污染物浓度的数据", "输入河流污染物浓度的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox6.Text == "")
                {
                    MessageBox.Show("请输入河流总量的数据", "输入河流污染物总量的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    IRasterLayer pVRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox2.Text);
                    IRasterLayer pQRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox3.Text);
                    IRasterLayer pLRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox4.Text);
                    IRasterLayer pCRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox5.Text);
                    IRasterLayer pSRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox6.Text);
                    IFeatureLayer pFeatureLayerSave = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureClass pFeatureClassSave = pFeatureLayerSave.FeatureClass;
                    IFeatureCursor pFeatureCursorSave = pFeatureClassSave.Update(null, false);
                    IFeature pFeatureSave = pFeatureCursorSave.NextFeature();
                    IPoint point = new PointClass();
                    double X = 0.0, Y = 0.0;
                    int column = 0, row = 0;
                    while (pFeatureSave != null)
                    {
                        point = pFeatureSave.Shape as IPoint;
                        X = point.X;
                        Y = point.Y;
                        RasterManagement.XYconvertNumber(pVRasterLayer, X, Y, ref column, ref row);
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("V"), Math.Round(Convert.ToDouble(RasterManagement.GetPixelValue(pVRasterLayer, 0, column, row)), 4));

                        RasterManagement.XYconvertNumber(pQRasterLayer, X, Y, ref column, ref row);
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("Q"), Math.Round(Convert.ToDouble(RasterManagement.GetPixelValue(pQRasterLayer, 0, column, row)), 4));

                        RasterManagement.XYconvertNumber(pLRasterLayer, X, Y, ref column, ref row);
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("L"), Math.Round(Convert.ToDouble(RasterManagement.GetPixelValue(pLRasterLayer, 0, column, row)), 4));

                        RasterManagement.XYconvertNumber(pCRasterLayer, X, Y, ref column, ref row);
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("C"), Math.Round(Convert.ToDouble(RasterManagement.GetPixelValue(pCRasterLayer, 0, column, row)), 4));

                        RasterManagement.XYconvertNumber(pSRasterLayer, X, Y, ref column, ref row);
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("S"), Math.Round(Convert.ToDouble(RasterManagement.GetPixelValue(pSRasterLayer, 0, column, row)), 4));

                        pFeatureCursorSave.UpdateFeature(pFeatureSave);
                        pFeatureSave = pFeatureCursorSave.NextFeature();
                    }

                    MessageBox.Show("程序运行完成！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
          
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入可供选择的应急处置空间位置的数据", "输入可供选择的应急处置空间位置的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox2.Text == "")
                {
                    MessageBox.Show("请输入河流流速的数据", "输入河流流速的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox3.Text == "")
                {
                    MessageBox.Show("请输入河流流量的数据", "输入河流流量的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox4.Text == "")
                {
                    MessageBox.Show("请输入河流宽度的数据", "输入河流宽度的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox5.Text == "")
                {
                    MessageBox.Show("请输入河流污染物浓度的数据", "输入河流污染物浓度的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox6.Text == "")
                {
                    MessageBox.Show("请输入河流总量的数据", "输入河流污染物总量的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    IFeatureLayer pFeatureLayerSave = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureClass pFeatureClassSave = pFeatureLayerSave.FeatureClass;
                    IQueryFilter pQueryFilterSave = new QueryFilterClass();
                    pQueryFilterSave.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox7.Text) - 1);
                    IFeatureCursor pFeatureCursorSave = pFeatureClassSave.Update(pQueryFilterSave, false);
                    IFeature pFeatureSave = pFeatureCursorSave.NextFeature();
                    while (pFeatureSave != null)
                    {
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("V"), Convert.ToDouble(textBox2.Text));
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("Q"), Convert.ToDouble(textBox3.Text));
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("L"), Convert.ToDouble(textBox4.Text));
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("C"), Convert.ToDouble(textBox5.Text));
                        pFeatureSave.set_Value(pFeatureSave.Fields.FindField("S"), Convert.ToDouble(textBox6.Text));
                        pFeatureCursorSave.UpdateFeature(pFeatureSave);
                        pFeatureSave = pFeatureCursorSave.NextFeature();
                    }
                    MessageBox.Show("程序运行结束!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }

       //取消该功能
        private void button8_Click(object sender, EventArgs e)
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

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            IFeatureLayer pFeatureLayerEmergencypoint = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
            IFeatureClass pFeatureClassEmergencypoint = pFeatureLayerEmergencypoint.FeatureClass;
            IQueryFilter pQueryFilter=new QueryFilterClass();
            pQueryFilter.WhereClause = "FID="  + Convert.ToString(Convert.ToInt32(comboBox7.Text) - 1) ;
            
            IFeatureCursor pFeatureCursoEmergencypoint = pFeatureClassEmergencypoint.Search(pQueryFilter, false);
            IFeature pFeatureEmergencypoint=pFeatureCursoEmergencypoint.NextFeature();
            IFields pFields = pFeatureClassEmergencypoint.Fields;
            IField pField;
            while (pFeatureEmergencypoint != null)
            {
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    pField = pFields.get_Field(i);
                    if (pField.Name.ToString() == "V")
                    {                                                 
                        textBox2.Text = Convert.ToString(pFeatureEmergencypoint.get_Value(pFeatureEmergencypoint.Fields.FindField("V")));                       
                    }
                    else if (pField.Name.ToString() == "Q")
                    {                             
                        textBox3.Text = Convert.ToString(pFeatureEmergencypoint.get_Value(pFeatureEmergencypoint.Fields.FindField("Q")));
                    }
                    else if (pField.Name.ToString() == "L")
                    { 
                        textBox4.Text = Convert.ToString(pFeatureEmergencypoint.get_Value(pFeatureEmergencypoint.Fields.FindField("L")));
                    }
                    else if (pField.Name.ToString() == "C")
                    {     
                        textBox5.Text = Convert.ToString(pFeatureEmergencypoint.get_Value(pFeatureEmergencypoint.Fields.FindField("C")));                   
                    }
                    else if (pField.Name.ToString() == "S")
                    { 
                        textBox6.Text = Convert.ToString(pFeatureEmergencypoint.get_Value(pFeatureEmergencypoint.Fields.FindField("S")));
                    }

                }
                pFeatureEmergencypoint = pFeatureCursoEmergencypoint.NextFeature();
            }

            if (textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "")
            {
                MessageBox.Show("可选择应急处置空间位置图层，缺少流速、流量、宽度、浓度或总量的字段，请新建该字段！");
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
            }
        }

        //输入的流速数据只能是数值型，而不能是字符等
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
                // 按下的为数字键、回车键或退格键（backspace）时允许输入
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)13 || e.KeyChar == (Char)8)
                    e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //输入的污染物浓度数据只能是数值型，而不能是字符等
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
                // 按下的为数字键、回车键或退格键（backspace）时允许输入
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)13 || e.KeyChar == (Char)8)
                    e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //输入的河宽数据只能是数值型，而不能是字符等类型
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
                // 按下的为数字键、回车键或退格键（backspace）时允许输入
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)13 || e.KeyChar == (Char)8)
                    e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //输入的流量数据只能是数值型，而不能是字符等类型
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
                // 按下的为数字键、回车键或退格键（backspace）时允许输入
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)13 || e.KeyChar == (Char)8)
                    e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //输入的污染物总量数据只能是数值型，而不能是字符等类型
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
                // 按下的为数字键、回车键或退格键（backspace）时允许输入
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)13 || e.KeyChar == (Char)8)
                    e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

    
      
    }
}
