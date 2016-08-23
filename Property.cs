using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

namespace Nuwa
{

    #region  xProperty 类

    public class Property
    {
        private List<ObjSettings> objSets;
        public static List<int> oldObjShowPattern = new List<int>();
        private GlobalSettings globalSet;

        public Property()
        {
            this.objSets = new List<ObjSettings>();
            this.globalSet = new GlobalSettings();
        }

        public int NumberOfProperties
        {
            get { return this.objSets.Count; }
        }

        public double Offset
        {
            get { return this.globalSet.LineSpacing; }
        }

        public double FirstOffset
        {
            get { return this.Offset; }
        }

        public int FillPattern
        {
            get { return this.GlobalSet.FillPattern; }
        }

        public double LineHeight
        {
            get { return this.GlobalSet.LineHeight; }
            set { this.GlobalSet.LineHeight = value; }
        }

        public double LineSpacing
        {
            get { return this.GlobalSet.LineSpacing; }
            set { this.GlobalSet.LineSpacing = value; }
        }     

        public double FillFactor
        {
            get { return this.GlobalSet.FillFactor; }
            set { this.GlobalSet.FillFactor = value; }
        }

        public double FeedFlow
        {
            get { return this.GlobalSet.FeedFlow; }
            set { this.GlobalSet.FeedFlow = value; }
        }

        public double PrintSpeed
        {
            get { return this.GlobalSet.PrintSpeed; }
            set { this.GlobalSet.PrintSpeed = value; }
        }     


        //public double[] Offset
        //{
        //    get 
        //    { 
        //        List<double> offset = new List<double>();
        //        for (int i = 0; i<NumberOfProperties; i++ )
        //        {
        //            offset.Add(this.objSets[i].Offset);
        //        }
        //        return offset.ToArray();
        //    }
        //}

        //public double[] FirstOffset
        //{
        //    get
        //    {
        //        List<double> firstOffset = new List<double>();
        //        for (int i = 0; i < NumberOfProperties; i++)
        //        {
        //            firstOffset.Add(this.objSets[i].FirstOffset);
        //        }
        //        return firstOffset.ToArray();
        //    }
        //}

        //public int[] FillPattern
        //{
        //    get
        //    {
        //        List<int> fillPattern = new List<int>();
        //        for (int i = 0; i < NumberOfProperties; i++)
        //        {
        //            fillPattern.Add(this.objSets[i].FillPattern);
        //        }
        //        return fillPattern.ToArray();
        //    }
        //}

        public List<ObjSettings> ObjSets
        {
            get { return this.objSets; }
            //set { this.globalSet = value; }
        }

        public GlobalSettings GlobalSet
        {
            get { return this.globalSet; }
            //set { this.globalSet = value; }
        }

        public void AddObjSet()
        {
            ObjSettings os = new ObjSettings();
            this.objSets.Add(os);
            oldObjShowPattern.Add(os.ObjShowPattern);
        }

        public ObjSettings GetObjSetAt(int index_)
        {
            if (index_ < this.objSets.Count)
                return this.objSets[index_];
            else
                return null;
        }

        public ObjSettings GetLastObjSet()
        {
           return this.objSets[this.objSets.Count -1];
        }


        public void RemoveLastObjSet()
        {
            oldObjShowPattern.RemoveAt(this.objSets.Count - 1);
            this.objSets.RemoveAt(this.objSets.Count - 1);
        }

        public void DelAllProperty()
        {
            if (this.objSets.Count >= 1)
                this.objSets.RemoveRange(0, this.objSets.Count);
        }


    }

    #endregion

    #region   xProperty所包含的类

    //[Serializable]
    public class GlobalSettings
    {
        private int tipSize = 0;
        private double printSpeed = 10;
        private double feedFlow = 0.5; // 实际上应该是1.32 ul/s ,  用水测量是0.88 ul/10000脉冲，1.5的齿轮比，10mm（10000脉冲）/s的速度， 因此齿轮比 = 流量 X 3
        private int fillPattern = 0;
        private double lineSpacing = 0.443;
        private double lineHeight = 0.443;
        private double fillFactor = 1;

        [CategoryAttribute("1. 机器设置"),
        DisplayNameAttribute("  1.1. 针头内径"),
        DescriptionAttribute("紫0.5/蓝0.4/橙0.35/红0.25/白/0.2/浅紫0.15/黄0.1"),
        TypeConverter(typeof(PropertyGridComboBoxItem)) ]
        public int TipSize
        {
            get { return tipSize; }
            set { tipSize = value; }
        }

        [CategoryAttribute("1. 机器设置"),
        DisplayNameAttribute("  1.2. 打印速度(mm/s)"),
        DescriptionAttribute("XYZ轴的移动速度")]
        public double PrintSpeed
        {
            get { return printSpeed; }
            set { printSpeed = value; }
        }

        [CategoryAttribute("1. 机器设置"),
        DisplayNameAttribute("  1.3. 给料流量(uL/s)"),
        DescriptionAttribute("打印头的吐料流量")]
        public double FeedFlow
        {
            get { return feedFlow; }
            set { feedFlow = value; }
        }

        [CategoryAttribute("2. 打印设置"),
        DisplayNameAttribute("  2.1. 填充模式"),
        DescriptionAttribute("轮廓/扫描/交叉扫描"),
        TypeConverter(typeof(ComboFillPattern))]
        public int FillPattern
        {
            get { return fillPattern; }
            set { fillPattern = value; }
        }

        [CategoryAttribute("2. 打印设置"),
        DisplayNameAttribute("  2.2. 层高(mm)"),
        DescriptionAttribute("每层的高度")]
        public double LineHeight
        {
            get { return lineHeight; }
            set { lineHeight = value; }
        }

        [CategoryAttribute("2. 打印设置"),
        DisplayNameAttribute("  2.3. 线间距(mm)"),
        DescriptionAttribute("路径之间的间距")]
        public double LineSpacing
        {
            get {  return lineSpacing;  }
            set { lineSpacing = value; }
        }

        [CategoryAttribute("2. 打印设置"),
        DisplayNameAttribute("  2.4. 填充密度"),
        DescriptionAttribute("10～110%"),
        ReadOnlyAttribute(true)]
        public double FillFactor
        {
            get { return fillFactor; }
            set { fillFactor = value; }
        }





    }

    public class ObjSettings
    {
        private Scale scale = new Scale();
        private Rotate rotate = new Rotate();
        private Translate translate = new Translate();
        private double clipPos = 0;
        private int objShowPattern = 0;
        private int pathShowPattern = 2;
        //private double offset = 0.35;
        //private double firstOffset = 0;
        //private int fillPattern = 0;

        //[CategoryAttribute("1. 打印路径设置"),
        //DisplayNameAttribute("    1.2. 填充模式"),
        //DescriptionAttribute("Vorono/Raster/CrossRaster"),
        //TypeConverter(typeof(ComboFillPattern))    ]
        //public int FillPattern
        //{
        //    get { return fillPattern; }
        //    set { fillPattern = value; }
        //}

        //[CategoryAttribute("1. 打印路径设置"),
        //DisplayNameAttribute("    1.3. 线间00距"),
        //DescriptionAttribute("路径之间的间距"),
        //DefaultValueAttribute(0.35)]
        //public double Offset
        //{
        //    get { return offset; }
        //    set { offset = value; }
        //}

        //[CategoryAttribute("1. 打印路径设置"),
        //DisplayNameAttribute("    1.4. 离壁间距"),
        //DescriptionAttribute("第1个路径离外周的间距"),
        //DefaultValueAttribute(0)]
        //public double FirstOffset
        //{
        //    get { return firstOffset; }
        //    set { firstOffset = value; }
        //}


        [CategoryAttribute("2. 物体转换"),
        DisplayNameAttribute("   2.1. 移动"),
        TypeConverter(typeof(TranslateCoverter))]
        public Translate Translate
        {
            get { return this.translate; }
            set { this.translate = value; }
        }

        [CategoryAttribute("2. 物体转换"),
        DisplayNameAttribute("   2.2. 旋转"),
        TypeConverter(typeof(RotateCoverter))]
        public Rotate Rotate
        {
            get { return this.rotate; }
            set { this.rotate = value; }
        }

        [CategoryAttribute("2. 物体转换"),
        DisplayNameAttribute("   2.3. 放大/缩小"),
        TypeConverter(typeof(ScaleCoverter))]
        public Scale Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        [CategoryAttribute("2. 物体转换"),
        DisplayNameAttribute("   2.4. 裁剪位置"),
        DefaultValueAttribute(0)]
        public double ClipPos
        {
            get { return clipPos; }
            set { clipPos = value; }
        }

        [CategoryAttribute("3. 显示设置"),
        DisplayNameAttribute("   3.1 3D显示模式"),
        TypeConverter(typeof(PropertyGridObjShowPatternItem))]
        public int ObjShowPattern
        {
            get { return objShowPattern; }
            set { objShowPattern = value; }
        }

        [CategoryAttribute("3. 显示设置"),
        DisplayNameAttribute("   3.2 2D显示模式"),
        TypeConverter(typeof(PropertyGridPathShowPatternItem))]
        public int PathShowPattern
        {
            get { return pathShowPattern; }
            set { pathShowPattern = value; }
        }

    }
    # endregion

    #region Coverter类

    public class Scale
    {
        private string x = "1";
        private string y = "1";
        private string z = "1";
        public string X_Scale
        {
            get { return x; }
            set { x = value; }
        }
        public string Y_Scale
        {
            get { return y; }
            set { y = value; }
        }

        public string Z_Scale
        {
            get { return z; }
            set { z = value; }
        }  
    }

    public class ScaleCoverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(Scale))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is Scale)
            {
                Scale so = (Scale)value;
                return "X: " + so.X_Scale + "    Y: " + so.Y_Scale + "    Z:" + so.Z_Scale;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class Rotate
    {
        private string x = "0";
        private string y = "0";
        private string z = "0";
        public string X_Rotate
        {
            get { return x; }
            set { x = value; }
        }
        public string Y_Rotate
        {
            get { return y; }
            set { y = value; }
        }

        public string Z_Rotate
        {
            get { return z; }
            set { z = value; }
        }  
    }

    public class RotateCoverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(Rotate))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&  value is Rotate)
            {
                Rotate so = (Rotate)value;
                return "X: " + so.X_Rotate+ "    Y: " + so.Y_Rotate + "    Z:" + so.Z_Rotate;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }    
    }


    public class Translate
    {
        private string x = "0";
        private string y = "0";
        private string z = "0";
        public string X_Translate
        {
            get { return x; }
            set { x = value; }
        }
        public string Y_Translate
        {
            get { return y; }
            set { y = value; }
        }

        public string Z_Translate
        {
            get { return z; }
            set { z = value; }
        }
    }

    public class TranslateCoverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(Translate))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is Translate)
            {
                Translate so = (Translate)value;
                return "X: " + so.X_Translate + "    Y: " + so.Y_Translate + "    Z:" + so.Z_Translate;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }


    public class PropertyGridBoolItem : ComboBoxItemTypeConvert
    {
        public override void GetConvertHash()
        {
            _hash.Add(0, "是");
            _hash.Add(1, "否");
        }
    }


    public class PropertyGridObjShowPatternItem : ComboBoxItemTypeConvert
    {
        public override void GetConvertHash()
        {
            _hash.Add(0, "显示物体");
            _hash.Add(1, "显示切片");
            _hash.Add(2, "显示路径");
            _hash.Add(3, "显示所有");
        }
    }

    public class PropertyGridPathShowPatternItem : ComboBoxItemTypeConvert
    {
        public override void GetConvertHash()
        {
            _hash.Add(0, "显示外周");
            _hash.Add(1, "显示路径");
            _hash.Add(2, "显示所有");
        }
    }

    public class ComboFillPattern  : ComboBoxItemTypeConvert
    {
        public override void GetConvertHash()
        {
            _hash.Add(0, "轮廓填充");
            _hash.Add(1, "扫描填充");
            _hash.Add(2, "交叉扫描填充");
            _hash.Add(3, "外轮廓内扫描");
        }

    }

    public class PropertyGridComboBoxItem : ComboBoxItemTypeConvert
    {
        public override void GetConvertHash()
        {
            _hash.Add(0, "紫  0.50mm");
            _hash.Add(1, "蓝  0.40mm");
            _hash.Add(2, "橙  0.35mm");
            _hash.Add(3, "红  0.25mm");
            _hash.Add(4, "白  0.20mm");
            _hash.Add(5, "浅紫 0.15mm");
            _hash.Add(6, "黄  0.10mm");
        }
    }

    # endregion

    # region  ComboBoxItemTypeConvert基础类

    /// IMSTypeConvert 的摘要说明。  

    public abstract class ComboBoxItemTypeConvert : TypeConverter
    {

        public Hashtable _hash = null;

        public ComboBoxItemTypeConvert()
        {
            _hash = new Hashtable();
            GetConvertHash();
        }

        public abstract void GetConvertHash();

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            int[] ids = new int[_hash.Values.Count];
            int i = 0;
            foreach (DictionaryEntry myDE in _hash)
            {
                ids[i++] = (int)(myDE.Key);
            }
            return new StandardValuesCollection(ids);

        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);

        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object v)
        {
            if (v is string)
            {
                foreach (DictionaryEntry myDE in _hash)
                {
                    if (myDE.Value.Equals((v.ToString())))
                        return myDE.Key;
                }
            }
            return base.ConvertFrom(context, culture, v);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object v, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                foreach (DictionaryEntry myDE in _hash)
                {
                    if (myDE.Key.Equals(v))
                        return myDE.Value.ToString();
                }
                return "";

            }

            return base.ConvertTo(context, culture, v, destinationType);

        }

        public override bool GetStandardValuesExclusive( ITypeDescriptorContext context)
        {
            return false;
        }
    }


    # endregion


}


