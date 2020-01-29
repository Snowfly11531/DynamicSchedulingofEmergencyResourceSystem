using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DynamicSchedulingofEmergencyResourceSystem;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;


namespace AttributeTable
{
    public partial class NewFieldFrm : Form
    {
        public NewFieldFrm()
        {
            InitializeComponent();
        }

        private void NewFieldFrm_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox1.SelectedIndex=2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + '\n' + ex.ToString(), "异常！");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.AppStarting;  // 设置对话框的鼠标指针为等待指针
                string fieldName = textBox1.Text;
                string aliasName = textBox2.Text;
                IFeatureClass featureClass = Variable.pAttributeTableFeatureLayer.FeatureClass;
                IFields fields = featureClass.Fields;
                if (fields.FindField(fieldName) != -1)
                {
                    MessageBox.Show("字段名称重复！");
                    this.Cursor = Cursors.Default;  // 设置对话框的鼠标指针为默认指针
                    textBox1.Focus();
                    return;
                }
                if (fields.FindFieldByAliasName(aliasName) != -1)
                {
                    MessageBox.Show("显示名称重复！");
                    this.Cursor = Cursors.Default;  // 设置对话框的鼠标指针为默认指针
                    textBox2.Focus();
                    return;
                }
                IField newField = new FieldClass();    // 新建字段
                IFieldEdit newFieldEdit = (IFieldEdit)newField;
                newFieldEdit.Name_2 = fieldName;       // 设置字段名称
                newFieldEdit.AliasName_2 = aliasName;  // 设置字段别名
                switch (comboBox1.Text)       // 设置字段类型
                {
                    case "整数":
                        newFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                        break;
                    case "小数":
                        newFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;

                        break;
                    case "文本":
                        newFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                        newFieldEdit.Length_2 = int.Parse(textBox1.Text);
                        break;
                    case "日期时间":
                        newFieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                        break;
                }
                newFieldEdit.IsNullable_2 = true;  // 允许空值
                featureClass.AddField(newFieldEdit);
                this.DialogResult = DialogResult.OK;
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                this.Cursor = Cursors.Default;  // 设置对话框的鼠标指针为默认指针
                this.textBox1.Focus();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = textBox1.Text;
                if (textBox1.Text == "")
                    button1.Enabled = false;
                else
                    button1.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "文本")
                {
                    label4.Visible = true;
                    textBox3.Visible = true;
                    //textBox_Length.ShortcutsEnabled = false;  // 禁用右键弹出快捷菜单（从而禁用复制粘贴功能）
                }
                else
                {
                    label4.Visible = false;
                    textBox3.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

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

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                // 字段名称太长，自动修改字段名称为...。（中文字符占两个长度，英文字符占一个长度。字段名称总长度不能超过10）
                string fieldName = textBox1.Text;
                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                byte[] fieldNameBytes = gb2312.GetBytes(fieldName);
                if (fieldNameBytes.Length > 10)
                {
                    // 确定前 10 个长度的字符串是否为原字符串的子串。如"123456789中"、"123协警文员"
                    string newFieldName =
                        fieldName.IndexOf(gb2312.GetString(fieldNameBytes, 0, 10)) == 0 ?
                        gb2312.GetString(fieldNameBytes, 0, 10) : gb2312.GetString(fieldNameBytes, 0, 9);
                    MessageBox.Show(string.Format("{0} 字段名称太长！", textBox1.Text));
                    textBox1.Text = newFieldName;
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (textBox3.Text == "" || textBox3.Text == "0")
                {
                    textBox3.Text = "50";
                    e.Cancel = true;
                }
                else
                {
                    long length = long.Parse(textBox3.Text);
                    int maxLength = 2147483647;
                    if (length > maxLength)
                    {
                        MessageBox.Show(string.Format("{0} 超过了最大长度 {1}！", length, maxLength));
                        textBox3.Text = maxLength.ToString();
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }
    }
}
