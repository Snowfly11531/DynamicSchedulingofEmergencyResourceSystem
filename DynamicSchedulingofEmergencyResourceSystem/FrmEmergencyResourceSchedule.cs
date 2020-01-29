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
    public partial class FrmEmergencyResourceSchedule : Form
    {               
        public FrmEmergencyResourceSchedule()
        {          
            InitializeComponent();
        }

        //输入应急资源仓库数据
        private void button1_Click(object sender, EventArgs e)
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

        //输入应急处置空间位置数据
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入应急处置空间位置数据";
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
                label5.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label5.Visible = false;
            }
        }

        //输入污染扩散模拟栅格数据
        private void button4_Click(object sender, EventArgs e)
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

        //进行应急资源调度模型的计算
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入应急资源仓库数据", "输入应急资源仓库数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入应急处置空间位置数据", "输入应急处置空间位置数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请输入污染物扩散模拟过程数据", "输入污染物扩散模拟过程数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    //初始化应急处置空间位置应急资源调度到达时间
                    IFeatureLayer pFeatureLayerDepot = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);//获取应急仓库图层
                    IFeatureClass pFeatureClassDepot = pFeatureLayerDepot.FeatureClass;
                    int DepotCount = pFeatureClassDepot.FeatureCount(null);
                    int DepotFieldCount = pFeatureClassDepot.Fields.FieldCount;
                    IFeatureLayer pFeatureLayerDest = CDataImport.ImportFeatureLayerFromControltext(comboBox2.Text); //获取应急处置空间位置图层
                    IFeatureClass pFeatureClassDest = pFeatureLayerDest.FeatureClass;
                    int DestCount = pFeatureClassDest.FeatureCount(null);
                    int DestFieldCount = pFeatureClassDest.Fields.FieldCount;
                    DijkstraDepotToDestMethod path = new DijkstraDepotToDestMethod();
                    List<Result> resultlist = new List<Result>();
                    resultlist = path.DepotToDest(pFeatureLayerDepot, pFeatureLayerDest);
                    //获取应急资源仓库到达应急处置空间位置的时间
                    double[,] materialArrivetime = new double[DepotCount, DestCount];
                    int count = 0;
                    for (int m = 0; m < DepotCount; m++)
                    {
                        for (int n = 0; n < DestCount; n++)
                        {
                            materialArrivetime[m, n] = resultlist[count].WeightTime;
                            count++;
                        }
                    }

                    double[,] materialdemand = new double[DestCount, DestFieldCount - 4];
                    List<DestProperty> pMaterialdemand = new List<DestProperty>();
                    string[] pFieldName = new string[DestFieldCount - 4];
                    //获取应急资源名称
                    int namecount = 0;
                    for (int i = 0; i < DestFieldCount; i++)
                    {
                        IField pField = new FieldClass();
                        pField = pFeatureLayerDest.FeatureClass.Fields.get_Field(i);
                        if (pField.Name.ToUpper() == "FID" || pField.Name.ToUpper() == "SHAPE" || pField.Name.ToUpper() == "NAME" || pField.Name.ToUpper() == "PROJECT")
                        {
                            continue;
                        }
                        else
                        {
                            pFieldName[namecount] = pField.Name;
                            namecount++;
                        }

                    }
                    double[,] materialstore = new double[DepotCount, pFieldName.Length];
                    List<DepotProperty> pERDepot = new List<DepotProperty>();
                    IFeatureCursor pFeatureCursorDepot = pFeatureClassDepot.Search(null, false);
                    IFeature pFeatureDepot = pFeatureCursorDepot.NextFeature();
                    int pdepotcount = 0;
                    while (pFeatureDepot != null)
                    {
                        for (int i = 0; i < pFieldName.Length; i++)
                        {
                            materialstore[pdepotcount, i] = Convert.ToDouble(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField(pFieldName[i])));
                            DepotProperty erdepot = new DepotProperty();
                            erdepot.Resourcename = pFieldName[i];
                            erdepot.Depotname = Convert.ToString(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField("Name")));
                            erdepot.Row = pdepotcount;
                            erdepot.Column = i;
                            erdepot.Number = Convert.ToDouble(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField(pFieldName[i])));
                            pERDepot.Add(erdepot);
                        }
                        pdepotcount++;
                        pFeatureDepot = pFeatureCursorDepot.NextFeature();
                    }
                    //污染物扩散过程到达应急处置空间位置的时间
                    IRasterLayer pRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox3.Text);
                    IFeatureCursor pFeatureCursorDest = pFeatureClassDest.Search(null, false);
                    IFeature pFeatureDest = pFeatureCursorDest.NextFeature();
                    IPoint pointDest = new PointClass();
                    List<PollutionArriveDestProperty> PollutionArriveTime = new List<PollutionArriveDestProperty>();
                    List<PollutionArriveDestProperty> ProjectTime = new List<PollutionArriveDestProperty>();
                    int pdestcount = 0;
                    while (pFeatureDest != null)
                    {
                        //获取污染物到达应急处置空间位置的时间，即为应急处置的最高时间
                        string destname = Convert.ToString(pFeatureDest.get_Value(pFeatureDest.Fields.FindField("Name")));
                        double X = 0.0, Y = 0.0;
                        int column = 0, row = 0;
                        pointDest = pFeatureDest.Shape as IPoint;
                        X = pointDest.X;
                        Y = pointDest.Y;
                        RasterManagement.XYconvertNumber(pRasterLayer, X, Y, ref column, ref row);
                        PollutionArriveDestProperty pullutionarrivetime = new PollutionArriveDestProperty();
                        pullutionarrivetime.Arrivetime = Math.Round(Convert.ToDouble(RasterManagement.GetPixelValue(pRasterLayer, 0, column, row)), 2);
                        pullutionarrivetime.Destname = destname;
                        pullutionarrivetime.Sequence = pdestcount + 1;
                        PollutionArriveTime.Add(pullutionarrivetime);
                        //获取应急处置空间位置应急物资需求
                        for (int i = 0; i < pFieldName.Length; i++)
                        {
                            materialdemand[pdestcount, i] = Convert.ToDouble(pFeatureDest.get_Value(pFeatureDest.Fields.FindField(pFieldName[i])));
                            DestProperty erdest = new DestProperty();
                            erdest.Resourcename = pFieldName[i];
                            erdest.Destname = destname;
                            erdest.Row = pdestcount;
                            erdest.Column = i;
                            erdest.Number = Convert.ToDouble(pFeatureDest.get_Value(pFeatureDest.Fields.FindField(pFieldName[i])));
                            pMaterialdemand.Add(erdest);
                        }
                        PollutionArriveDestProperty projecttime = new PollutionArriveDestProperty();
                        projecttime.Arrivetime = Convert.ToDouble(pFeatureDest.get_Value(pFeatureDest.Fields.FindField("Project")));
                        projecttime.Destname = destname;
                        ProjectTime.Add(projecttime);
                        pdestcount++;
                        pFeatureDest = pFeatureCursorDest.NextFeature();
                    }
                    BubbleSort(PollutionArriveTime);
                    List<EmergencyScheduleArriveProperty> ERSTime = new List<EmergencyScheduleArriveProperty>();
                    //根据污染物到达应急处置空间位置先后顺序进行应急资源调度
                    for (int j = 0; j < PollutionArriveTime.Count; j++)
                    {
                        listBox1.Items.Add(PollutionArriveTime[j].Destname + "应急资源的调度情况：");
                        listBox1.Items.Add("");
                        ERSTime.Clear();
                        int destsequence = (PollutionArriveTime[j].Sequence - 1) * pFieldName.Length;

                        //对应急物资资源库到达应急处置空间位置的时间进行排序，以便从最短的应急资源仓库进行调度
                        int pdestsequence = PollutionArriveTime[j].Sequence - 1;
                        for (int n = 0; n < DepotCount; n++)
                        {
                            EmergencyScheduleArriveProperty pertime = new EmergencyScheduleArriveProperty();
                            pertime.TotalTime = resultlist[pdestsequence].WeightTime + ProjectTime[PollutionArriveTime[j].Sequence - 1].Arrivetime;
                            pertime.Sequence = n + 1;
                            ERSTime.Add(pertime);
                            pdestsequence += DestCount;
                        }
                        BubbleSort(ERSTime);
                        //根据应急处置点需求的应急资源进行处置
                        for (int m = 0; m < pFieldName.Length; m++)
                        {
                            listBox1.Items.Add(pFieldName[m] + "需求量为：" + pMaterialdemand[destsequence].Number.ToString());

                            //根据应急资源需求的紧迫度，进行应急资源的调度
                            if (pMaterialdemand[destsequence].Number > 0)
                            {
                                for (int k = 0; k < ERSTime.Count; k++)
                                {
                                    if (ERSTime[k].TotalTime > PollutionArriveTime[j].Arrivetime)
                                    {
                                        listBox1.Items.Add("剩余的应急资源物资无法在指定时间范围内到该应急处置空间位置！");
                                        break;
                                    }
                                    else if (pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number > pMaterialdemand[destsequence].Number && ERSTime[k].TotalTime <= PollutionArriveTime[j].Arrivetime)
                                    {
                                        string strNumber = (pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number - pMaterialdemand[destsequence].Number).ToString("N2");
                                        pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number = Convert.ToDouble(strNumber);
                                        listBox1.Items.Add(pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Depotname + "→" + pMaterialdemand[destsequence].Destname + ":" + pMaterialdemand[destsequence].Number.ToString());
                                        Result pResult = new Result();
                                        pResult.DepotName = pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Depotname;
                                        pResult.DestName = pMaterialdemand[destsequence].Destname;
                                        for (int x = 0; x < resultlist.Count; x++)
                                        {
                                            if (resultlist[x].DepotName == pResult.DepotName && resultlist[x].DestName == pResult.DestName)
                                            {
                                                if (resultlist[x].dynamicschedule == false)
                                                {
                                                    resultlist[x].dynamicschedule = true;
                                                }
                                            }

                                        }
                                        pMaterialdemand[destsequence].Number = 0.0;
                                        break;
                                    }
                                    else if (pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number > 0 && pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number < pMaterialdemand[destsequence].Number && ERSTime[k].TotalTime <= PollutionArriveTime[j].Arrivetime)
                                    {
                                        string strNumber = (pMaterialdemand[destsequence].Number - pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number).ToString("N2");
                                        pMaterialdemand[destsequence].Number = Convert.ToDouble(strNumber);
                                        listBox1.Items.Add(pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Depotname + "→" + pMaterialdemand[destsequence].Destname + ":" + pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number.ToString());
                                        Result pResult = new Result();
                                        pResult.DepotName = pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Depotname;
                                        pResult.DestName = pMaterialdemand[destsequence].Destname;
                                        for (int x = 0; x < resultlist.Count; x++)
                                        {
                                            if (resultlist[x].DepotName == pResult.DepotName && resultlist[x].DestName == pResult.DestName)
                                            {
                                                if (resultlist[x].dynamicschedule == false)
                                                {
                                                    resultlist[x].dynamicschedule = true;
                                                }
                                            }
                                        }
                                        pERDepot[(ERSTime[k].Sequence - 1) * pFieldName.Length + m].Number = 0.0;
                                        if (k == ERSTime.Count - 1 && pMaterialdemand[destsequence].Number != 0)
                                        {
                                            listBox1.Items.Add("应急资源仓库的存储应急资源物资不足！");
                                        }
                                    }
                                }
                            }
                            destsequence++;
                            listBox1.Items.Add("");
                        }
                        listBox1.Items.Add("应急资源调度路线：");
                    }


                    Collection<RouteNode> routeNodes = new Collection<RouteNode>();       //存储某起点和终点最短路径网络所经过的结点

                    List<IPolyline> routeroad = new List<IPolyline>();

                    string r;
                    for (int y = 0; y < resultlist.Count; y++)
                    {                      
                        r = null;
                        if (resultlist[y].dynamicschedule == true)
                        {
                            foreach (string str in resultlist[y].StrResultNode)
                            {
                                r += str + "→";
                                DijkstraDepotToDestMethod.addRouteNodes(routeNodes, str);
                            }
                            r += resultlist[y].EndNodeID;
                            DijkstraDepotToDestMethod.addRouteNodes(routeNodes, resultlist[y].EndNodeID);
                            listBox1.Items.Add(r);
                            path.GetIElement(routeNodes);
                            routeroad.Add(Variable.PElement.Geometry as IPolyline);
                            routeNodes.Clear();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //获取
                    DijkstraDepotToDestMethod.GetIElement(routeroad);
                    DijkstraDepotToDestMethod.displayElement();
                    //IFeatureLayer comboBoxFeature=CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    //SaveVector.polylinetoFeatureLayer(comboBox4.Text, routeroad, comboBoxFeature);
                    MessageBox.Show("处理完毕！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
           
        }

        //取消该功能运行
        private void button5_Click(object sender, EventArgs e)
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

        //污染物达到应急处置空间位置的时间进行冒泡法排序
        public void BubbleSort(List<PollutionArriveDestProperty> parrivetime)
        {
            for (int m = parrivetime.Count - 1; m > 0; m--)
            {
                for (int n = 0; n < m; n++)
                {
                    if (parrivetime[n].Arrivetime > parrivetime[n + 1].Arrivetime)
                    {
                        PollutionArriveDestProperty temp = parrivetime[n];
                        parrivetime[n] = parrivetime[n + 1];
                        parrivetime[n + 1] = temp;
                    }
                }
            }
        }

        //应急资源到达各个应急处置空间位置的时间进行冒泡法排序
        public void BubbleSort(List<EmergencyScheduleArriveProperty> parrivetime)
        {
            for (int m = parrivetime.Count - 1; m > 0; m--)
            {
                for (int n = 0; n < m; n++)
                {
                    if (parrivetime[n].TotalTime > parrivetime[n + 1].TotalTime)
                    {
                        EmergencyScheduleArriveProperty temp = parrivetime[n];
                        parrivetime[n] = parrivetime[n + 1];
                        parrivetime[n + 1] = temp;
                    }
                }
            }
        }      
    }   
}
