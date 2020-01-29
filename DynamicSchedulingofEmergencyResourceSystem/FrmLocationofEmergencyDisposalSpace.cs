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
    public partial class FrmLocationofEmergencyDisposalSpace : Form
    {
        IFeatureLayer pFeatureLayerDept = null;
        IFeatureLayer pFeatureLayerDepot = null;
        Label[] pLabel;
        CheckBox[] pCheckBox;
        List<EmergencyDepotResource> pEmergencyDepotResource = null;
        List<EmergencyDeptResource> pEmergencyDeptResource = null;

        public FrmLocationofEmergencyDisposalSpace()
        {
            InitializeComponent();
        }


        private void FrmLocationofEmergencyDisposalSpace_Load(object sender, EventArgs e)
        {
            label7.Enabled = false;
            label9.Enabled = false;
            comboBox4.Enabled = false;
            checkBox1.Enabled = false;
            textBox1.Enabled = false;
            groupBox4.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入可供选择的应急空间处置数据";
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


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入应急资源仓库数据";
                openDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                openDialog.Multiselect = false;
                openDialog.RestoreDirectory = false;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox2.Text = openDialog.FileName.ToString();
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
                openFileDialog.Title = "输入污染扩散模拟栅格数据";
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


        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                saveFileDialog.Title = "输出应急资源调度的仓库数据";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog.FileName.ToString() == "")
                    {
                        MessageBox.Show("请输入图层名称！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    }
                    else
                    {
                        comboBox5.Text = saveFileDialog.FileName.ToString();
                    }
                    label10.Visible = true;
                }
                else
                {
                    return;
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
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                saveFileDialog.Title = "输出应急处置空间位置的数据";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog.FileName.ToString() == "")
                    {
                        MessageBox.Show("请输入图层名称！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    }
                    else
                    {
                        comboBox6.Text = saveFileDialog.FileName.ToString();
                    }
                    label13.Visible = true;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入可供选择应急空间处置数据", "输入可供选择应急空间处置数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    label7.Enabled = true;
                    comboBox4.Enabled = true;
                    checkBox1.Enabled = true;
                    label9.Enabled = true;
                    textBox1.Enabled = true;

                    string currentFieldName = "FID";
                    pFeatureLayerDept = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureClass pFeatureClassDept = pFeatureLayerDept.FeatureClass;
                    IFeatureCursor pFeatureCursorEmergency = pFeatureClassDept.Search(null, false);
                    IDataStatistics pDataStatistics = new DataStatistics();
                    pDataStatistics.Field = currentFieldName;
                    pDataStatistics.Cursor = pFeatureCursorEmergency as ICursor;
                    System.Collections.IEnumerator pEnumerator = pDataStatistics.UniqueValues;
                    while (pEnumerator.MoveNext())
                    {
                        comboBox4.Items.Add(Convert.ToInt32(pEnumerator.Current.ToString()) + 1);
                    }
                    comboBox4.SelectedIndex = 0;      
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
                                     
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入应急资源仓库数据", "输入应急资源仓库数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请输入污染物扩散模拟数据", "输入污染物扩散模拟数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    //获取各个应急资源仓库到达应急处置空间位置的时间
                    pFeatureLayerDepot = CDataImport.ImportFeatureLayerFromControltext(comboBox2.Text);
                    String pFeatureFID = "FID=" + Convert.ToString(Convert.ToInt32(comboBox4.Text) - 1);
                    EmergencySpaceSelectionDijkstra pEmergencySpaceSelection = new EmergencySpaceSelectionDijkstra();
                    List<Result> resultlist = new List<Result>();
                    resultlist = pEmergencySpaceSelection.DepotToDest(pFeatureLayerDepot, pFeatureLayerDept, pFeatureFID);

                    //判断应急资源需求的种类
                    int EmergencyNumber = 0;
                    if (checkBox1.Checked == true)
                    {
                        EmergencyNumber++;
                    }
                    for (int i = 0; i < pCheckBox.Length; i++)
                    {
                        if (pCheckBox[i].Checked == true)
                        {
                            EmergencyNumber++;
                        }
                    }

                    //获取应急资需求的名称
                    string[] EmergencyVarietyName = new string[EmergencyNumber];
                    int EmergencyVarietyNameIndex = 0;
                    if (checkBox1.Checked == true)
                    {
                        int EmergencyVarietyIndex = label8.Text.LastIndexOf("：");
                        EmergencyVarietyName[EmergencyVarietyNameIndex] = label8.Text.Substring(0, EmergencyVarietyIndex);
                        EmergencyVarietyNameIndex++;
                    }
                    for (int j = 0; j < pLabel.Length; j++)
                    {
                        if (pCheckBox[j].Checked == true)
                        {
                            int EmergencyVarietyIndex = pLabel[j].Text.LastIndexOf("：");
                            EmergencyVarietyName[EmergencyVarietyNameIndex] = pLabel[j].Text.Substring(0, EmergencyVarietyIndex);
                            EmergencyVarietyNameIndex++;
                        }
                    }

                    //获取应急处置空间位置应急资源需求 
                    pEmergencyDeptResource = new List<EmergencyDeptResource>();
                    int EmergencyDemandResourceNumber = 0;
                    if (checkBox1.Checked == true)
                    {
                        EmergencyDeptResource emergencydeptresource = new EmergencyDeptResource();
                        int EmergencyDemandResourceIndex = label8.Text.LastIndexOf("：");
                        emergencydeptresource.EmergencyDemandResourceName = EmergencyVarietyName[EmergencyDemandResourceNumber];
                        emergencydeptresource.EmergencyDemandResourceNumber = Convert.ToDouble(label8.Text.Substring(EmergencyDemandResourceIndex + 1));
                        pEmergencyDeptResource.Add(emergencydeptresource);
                        EmergencyDemandResourceNumber++;
                    }
                    for (int k = 0; k < pLabel.Length; k++)
                    {
                        if (pCheckBox[k].Checked == true)
                        {
                            EmergencyDeptResource emergencydeptresource = new EmergencyDeptResource();
                            int EmergencyDemandResourceIndex = pLabel[k].Text.LastIndexOf("：");
                            emergencydeptresource.EmergencyDemandResourceName = EmergencyVarietyName[EmergencyDemandResourceNumber];
                            emergencydeptresource.EmergencyDemandResourceNumber = Convert.ToDouble(pLabel[k].Text.Substring(EmergencyDemandResourceIndex + 1));
                            pEmergencyDeptResource.Add(emergencydeptresource);
                            EmergencyDemandResourceNumber++;
                        }
                    }


                    //获取应急资源仓库存储的的应急资源          
                    IFeatureClass pFeatureClassDepot = pFeatureLayerDepot.FeatureClass;
                    int DepotNumber = pFeatureClassDepot.FeatureCount(null);
                    IFeatureCursor pFeatureCursorDepot = pFeatureClassDepot.Search(null, false);
                    IFeature pFeatureDepot = pFeatureCursorDepot.NextFeature();
                    int DepotNumberArriveDept = 0;
                    pEmergencyDepotResource = new List<EmergencyDepotResource>();
                    while (pFeatureDepot != null)
                    {
                        EmergencyDepotResource emergencydepotresource = new EmergencyDepotResource();
                        emergencydepotresource.DepotName = Convert.ToString(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField("Name")));
                        emergencydepotresource.DepotArriveDeptTime = Math.Round(Convert.ToDouble(resultlist[DepotNumberArriveDept].WeightTime), 4);
                        DepotNumberArriveDept++;
                        emergencydepotresource.EmergencyVarietyNameNumber = new double[EmergencyNumber];
                        emergencydepotresource.EmergencyVarietyName = new string[EmergencyNumber];
                        for (int k = 0; k < EmergencyNumber; k++)
                        {
                            emergencydepotresource.EmergencyVarietyName[k] = EmergencyVarietyName[k];
                            emergencydepotresource.EmergencyVarietyNameNumber[k] = Convert.ToDouble(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField(EmergencyVarietyName[k])));
                        }
                        pEmergencyDepotResource.Add(emergencydepotresource);
                        pFeatureDepot = pFeatureCursorDepot.NextFeature();
                    }



                    //获取污染物扩散到达应急处置空间位置的时间
                    double pollutionArriveTime = 0.0;
                    IFeatureClass pFeatureClassDept = pFeatureLayerDept.FeatureClass;
                    IQueryFilter pQueryFilterDept = new QueryFilterClass();
                    pQueryFilterDept.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox4.Text) - 1);
                    IFeatureCursor pFeatureCursorDept = pFeatureClassDept.Search(pQueryFilterDept, false);
                    IFeature pFeatureDept = pFeatureCursorDept.NextFeature();
                    IPoint point = new PointClass();
                    while (pFeatureDept != null)
                    {
                        point = pFeatureDept.Shape as IPoint;
                        pFeatureDept = pFeatureCursorDept.NextFeature();
                    }
                    double X = 0.0, Y = 0.0;
                    X = Math.Round(point.X, 4);
                    Y = Math.Round(point.Y, 4);
                    int column = 0, row = 0;
                    IRasterLayer pRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox3.Text);
                    RasterManagement.XYconvertNumber(pRasterLayer, X, Y, ref column, ref row);
                    pollutionArriveTime = Convert.ToDouble(RasterManagement.GetPixelValue(pRasterLayer, 0, column, row));

                    //读取构架应急工程措施的时间
                    double SetProjectTime = Math.Round(Convert.ToDouble(textBox1.Text), 4);

                    //对各个应急资源仓库到达应急处置空间位置的时间进行排序
                    BubbleSort(pEmergencyDepotResource);

                    //进行应急资源调度
                    List<EmergencyResourceResult> pEergencyResourceResult = new List<EmergencyResourceResult>();
                    for (int k = 0; k < DepotNumber; k++)//按照到达应急处置空间位置的应急仓库进行排序
                    {

                        if (pEmergencyDepotResource[k].DepotArriveDeptTime + SetProjectTime < pollutionArriveTime)//判断到达应急处置空间位置与构建工程时间之和小于污染物扩散到达时间
                        {
                            //创建应急资源调度的结果
                            EmergencyResourceResult emergencyresourceresult = new EmergencyResourceResult();
                            emergencyresourceresult.EmergencyResourceName = new string[pEmergencyDepotResource[k].EmergencyVarietyName.Length];
                            emergencyresourceresult.EmergencyResourceNumber = new double[pEmergencyDepotResource[k].EmergencyVarietyNameNumber.Length];
                            for (int l = 0; l < pEmergencyDeptResource.Count; l++)//应急处置空间位置需要的应急资源种类
                            {
                                for (int h = 0; h < pEmergencyDepotResource[k].EmergencyVarietyName.Length; h++)//对应急资源仓库的应急资源名称进行查找和应急处置空间位置名称相同
                                {
                                    if (pEmergencyDepotResource[k].EmergencyVarietyName[h] == pEmergencyDeptResource[l].EmergencyDemandResourceName && pEmergencyDepotResource[k].EmergencyVarietyNameNumber[h] > 0)
                                    {
                                        emergencyresourceresult.DepotName = pEmergencyDepotResource[k].DepotName;
                                        emergencyresourceresult.EmergencyResourceArriveTime = pEmergencyDepotResource[k].DepotArriveDeptTime;
                                        if (pEmergencyDeptResource[l].EmergencyDemandResourceNumber <= pEmergencyDepotResource[k].EmergencyVarietyNameNumber[h])
                                        {
                                            emergencyresourceresult.EmergencyResourceName[l] = pEmergencyDeptResource[l].EmergencyDemandResourceName;
                                            emergencyresourceresult.EmergencyResourceNumber[l] = pEmergencyDeptResource[l].EmergencyDemandResourceNumber;
                                            pEmergencyDepotResource[k].EmergencyVarietyNameNumber[h] -= pEmergencyDeptResource[l].EmergencyDemandResourceNumber;
                                            pEmergencyDepotResource[k].EmergencyDepotBool = true;
                                            pEmergencyDeptResource[l].EmergencyDemandResourceNumber -= pEmergencyDeptResource[l].EmergencyDemandResourceNumber;
                                        }
                                        else
                                        {
                                            emergencyresourceresult.EmergencyResourceName[l] = pEmergencyDeptResource[l].EmergencyDemandResourceName;
                                            emergencyresourceresult.EmergencyResourceNumber[l] = pEmergencyDepotResource[k].EmergencyVarietyNameNumber[h];
                                            pEmergencyDeptResource[l].EmergencyDemandResourceNumber -= pEmergencyDepotResource[k].EmergencyVarietyNameNumber[l];
                                            pEmergencyDepotResource[k].EmergencyVarietyNameNumber[l] = 0.0;
                                            pEmergencyDepotResource[k].EmergencyDepotBool = true;
                                        }

                                    }
                                }
                            }


                            //将有应急资源仓库调度资源的显示在ListBox中
                            for (int f = 0; f < emergencyresourceresult.EmergencyResourceNumber.Length; f++)
                            {
                                if (emergencyresourceresult.EmergencyResourceNumber[f] > 0)
                                {
                                    listBox1.Items.Add(pEmergencyDepotResource[k].DepotName);
                                    break;
                                }
                            }


                            //将应急资源调度的结果显示在ListBox
                            for (int a = 0; a < emergencyresourceresult.EmergencyResourceNumber.Length; a++)
                            {
                                if (emergencyresourceresult.EmergencyResourceNumber[a] > 0)
                                {
                                    listBox1.Items.Add(emergencyresourceresult.EmergencyResourceName[a] + "：" + Convert.ToString(emergencyresourceresult.EmergencyResourceNumber[a]));
                                }
                            }
                        }
                        else
                        {
                            //listBox1.Items.Add("其他的应急资源仓库的应急资源无法到达应急处置空间位置！");
                            break;
                        }

                        //如果应急处置空间位置的应急资源满足，则跳出应急资源调度过程
                        int DeptVarietyNumber = 0;
                        for (int b = 0; b < pEmergencyDeptResource.Count; b++)
                        {
                            if (pEmergencyDeptResource[b].EmergencyDemandResourceNumber == 0)
                            {
                                DeptVarietyNumber++;
                                continue;
                            }
                        }
                        if (DeptVarietyNumber == pEmergencyDeptResource.Count)
                        {
                            break;
                        }
                    }

                    //刷新应急处置点信息情况
                    for (int c = 0; c < pEmergencyDeptResource.Count; c++)
                    {
                        for (int d = 0; d < pLabel.Length; d++)
                        {
                            string VarietyName = pLabel[d].Text.Substring(0, pLabel[d].Text.LastIndexOf("："));
                            if (VarietyName == pEmergencyDeptResource[c].EmergencyDemandResourceName)
                            {
                                pLabel[d].Text = pEmergencyDeptResource[c].EmergencyDemandResourceName + "：" + pEmergencyDeptResource[c].EmergencyDemandResourceNumber.ToString();
                                break;
                            }
                        }
                    }
                    groupBox4.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                //保存应急处置空间位置应急资源调度后
                IFeatureClass pFeatureClassDeptResult = pFeatureLayerDept.FeatureClass;
                IQueryFilter pQueryFilterDeptResult = new QueryFilterClass();
                pQueryFilterDeptResult.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox4.Text) - 1);
                IFeatureCursor pFeatureCursorDeptResult = pFeatureClassDeptResult.Update(pQueryFilterDeptResult, false);
                IFeature pFeatureDeptResult = pFeatureCursorDeptResult.NextFeature();

                //保存应急处置空间位置的图层
                List<IPoint> DeptpointList = new List<IPoint>();
                DeptpointList.Add(pFeatureDeptResult.Shape as IPoint);
                SaveVector.pointtoFeatureLayer(comboBox6.Text, DeptpointList, pFeatureLayerDept);

                //保存应急处置空间位置应急资源调度后
                while (pFeatureDeptResult != null)
                {
                    for (int i = 0; i < pEmergencyDeptResource.Count; i++)
                    {
                        pFeatureDeptResult.set_Value(pFeatureDeptResult.Fields.FindField(pEmergencyDeptResource[i].EmergencyDemandResourceName), pEmergencyDeptResource[i].EmergencyDemandResourceNumber);
                    }
                    pFeatureCursorDeptResult.UpdateFeature(pFeatureDeptResult);
                    pFeatureDeptResult = pFeatureCursorDeptResult.NextFeature();
                }

                //保存应急资源仓库应急资源数量
                IFeatureClass pFeatureClassDepotResult = pFeatureLayerDepot.FeatureClass;

                //定义应急资源仓库点的List
                List<IPoint> DepotpointList = new List<IPoint>();

                for (int j = 0; j < pEmergencyDepotResource.Count; j++)
                {
                    if (pEmergencyDepotResource[j].EmergencyDepotBool == true)
                    {
                        IQueryFilter pQueryFilterDepotResult = new QueryFilterClass();
                        pQueryFilterDepotResult.WhereClause = "Name=" + "'" + Convert.ToString(pEmergencyDepotResource[j].DepotName) + "'";
                        IFeatureCursor pFeatureCursorDepotResult = pFeatureClassDepotResult.Update(pQueryFilterDepotResult, false);
                        IFeature pFeauteDepotResult = pFeatureCursorDepotResult.NextFeature();

                        //将调度出的应急资源仓库点加载到List中
                        DepotpointList.Add(pFeauteDepotResult.Shape as IPoint);

                        while (pFeauteDepotResult != null)
                        {
                            for (int k = 0; k < pEmergencyDepotResource[j].EmergencyVarietyName.Length; k++)
                            {
                                pFeauteDepotResult.set_Value(pFeauteDepotResult.Fields.FindField(pEmergencyDepotResource[j].EmergencyVarietyName[k]), pEmergencyDepotResource[j].EmergencyVarietyNameNumber[k]);

                            }
                            pFeatureCursorDepotResult.UpdateFeature(pFeauteDepotResult);
                            pFeauteDepotResult = pFeatureCursorDepotResult.NextFeature();

                        }
                    }
                }
                //保存应急资源仓库数据
                SaveVector.pointtoFeatureLayer(comboBox5.Text, DepotpointList, pFeatureLayerDepot);
                MessageBox.Show("运行结束！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void button7_Click(object sender, EventArgs e)
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

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (groupBox2.Controls.Count > 7)
                {
                    for (int k = 0; k < pLabel.Length; k++)
                    {
                        groupBox2.Controls.Remove(pLabel[k]);
                        groupBox2.Controls.Remove(pCheckBox[k]);
                    }
                    checkBox1.Checked = false;
                }
                IFeatureClass pFeatureClassDept = pFeatureLayerDept.FeatureClass;
                IQueryFilter pQueryFilterDept = new QueryFilterClass();
                pQueryFilterDept.WhereClause = "FID=" + Convert.ToString(Convert.ToInt32(comboBox4.Text) - 1);
                IFeatureCursor pFeatureCursorDept = pFeatureClassDept.Search(pQueryFilterDept, false);
                IFeature pFeatureDept = pFeatureCursorDept.NextFeature();
                IFields pFields = pFeatureClassDept.Fields;
                IField pField;
                pField = pFields.get_Field(2);
                label8.Text = pField.Name.ToString() + "：" + Convert.ToString(pFeatureDept.get_Value(pFeatureDept.Fields.FindField(pField.Name.ToString())));
                pLabel = new Label[pFields.FieldCount - 3];
                pCheckBox = new CheckBox[pFields.FieldCount - 3];
                for (int i = 3; i < pFields.FieldCount; i++)
                {
                    pField = pFields.get_Field(i);
                    pLabel[i - 3] = new Label();
                    pCheckBox[i - 3] = new CheckBox();
                    if (2 < i && i < 6)
                    {
                        //pLabel[i - 3].Size = label8.Size;
                        pLabel[i - 3].Location = new System.Drawing.Point(label8.Location.X, label8.Location.Y + (i - 2) * 25);
                        pLabel[i - 3].Text = pField.Name.ToString() + "：" + Convert.ToString(pFeatureDept.get_Value(pFeatureDept.Fields.FindField(pField.Name.ToString())));
                        groupBox2.Controls.Add(pLabel[i - 3]);
                        pCheckBox[i - 3].Size = checkBox1.Size;
                        pCheckBox[i - 3].Location = new System.Drawing.Point(checkBox1.Location.X, checkBox1.Location.Y + (i - 2) * 25);
                        groupBox2.Controls.Add(pCheckBox[i - 3]);

                    }
                    else if (5 < i && i < 10)
                    {
                        //pLabel[i - 3].Size = label8.Size;
                        pLabel[i - 3].Location = new System.Drawing.Point(label8.Location.X + 150, label8.Location.Y + (i - 6) * 25);
                        pLabel[i - 3].Text = pField.Name.ToString() + "：" + Convert.ToString(pFeatureDept.get_Value(pFeatureDept.Fields.FindField(pField.Name.ToString())));
                        groupBox2.Controls.Add(pLabel[i - 3]);
                        pCheckBox[i - 3].Size = checkBox1.Size;
                        pCheckBox[i - 3].Location = new System.Drawing.Point(checkBox1.Location.X + 150, checkBox1.Location.Y + (i - 6) * 25);
                        groupBox2.Controls.Add(pCheckBox[i - 3]);

                    }
                    else if (9 < i && i < 14)
                    {
                        //pLabel[i - 3].Size = label8.Size;
                        pLabel[i - 3].Location = new System.Drawing.Point(label8.Location.X + 300, label8.Location.Y + (i - 10) * 25);
                        pLabel[i - 3].Text = pField.Name.ToString() + "：" + Convert.ToString(pFeatureDept.get_Value(pFeatureDept.Fields.FindField(pField.Name.ToString())));
                        groupBox2.Controls.Add(pLabel[i - 3]);
                        pCheckBox[i - 3].Size = checkBox1.Size;
                        pCheckBox[i - 3].Location = new System.Drawing.Point(checkBox1.Location.X + 300, checkBox1.Location.Y + (i - 10) * 25);
                        groupBox2.Controls.Add(pCheckBox[i - 3]);
                    }
                    else if (13 < i && i < 18)
                    {
                        //pLabel[i - 3].Size = label8.Size;
                        pLabel[i - 3].Location = new System.Drawing.Point(label8.Location.X + 450, label8.Location.Y + (i - 14) * 25);
                        pLabel[i - 3].Text = pField.Name.ToString() + "：" + Convert.ToString(pFeatureDept.get_Value(pFeatureDept.Fields.FindField(pField.Name.ToString())));
                        groupBox2.Controls.Add(pLabel[i - 3]);
                        pCheckBox[i - 3].Size = checkBox1.Size;
                        pCheckBox[i - 3].Location = new System.Drawing.Point(checkBox1.Location.X + 450, checkBox1.Location.Y + (i - 14) * 25);
                        groupBox2.Controls.Add(pCheckBox[i - 3]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }             
       
        //应急资源到达各个应急处置空间位置的时间进行冒泡法排序
        public void BubbleSort(List<EmergencyDepotResource> emergencydepotresource)
        {
            try
            {
                for (int m = emergencydepotresource.Count - 1; m > 0; m--)
                {
                    for (int n = 0; n < m; n++)
                    {
                        if (emergencydepotresource[n].DepotArriveDeptTime > emergencydepotresource[n + 1].DepotArriveDeptTime)
                        {
                            EmergencyDepotResource pEmergencyDepotResource = emergencydepotresource[n];
                            emergencydepotresource[n] = emergencydepotresource[n + 1];
                            emergencydepotresource[n + 1] = pEmergencyDepotResource;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                comboBox4.SelectedIndex = 0;

                pEmergencyDepotResource = null;
                pEmergencyDeptResource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }

       
    }
}
