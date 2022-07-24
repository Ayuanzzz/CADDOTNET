using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;


namespace DotNetArX
{
    public static class RegionTools
    {
        public static List<Region> CreateRegion(params Curve[] curves)
        {
            //新建面域列表，存储创建的面域
            List<Region> regionList = new List<Region>();
            //将可变数组转化为集合类，用于面域的创建
            DBObjectCollection curveCollection = new DBObjectCollection();
            //遍历曲线
            foreach (Curve curve in curves)
            {
                //如果曲线已经被加入到数据库且为写的状态，则返回
                if (!curve.IsNewObject && curve.IsWriteEnabled)
                    return null;
                //曲线添加到集合中
                curveCollection.Add(curve);
            }
            try
            {
                //根据曲线集合，在内存中创建面域对象集合
                DBObjectCollection regionObjs = Region.CreateFromCurves(curveCollection);
                //将面域对象集合复制到面域列表
                foreach(Region region in regionObjs)
                {
                    regionList.Add(region);
                }
                return regionList;
            }
            //面域创建失败
            catch
            {
                //清空面域列表
                regionList.Clear();
                //返回空的面域列表
                return regionList;
            }
        }
    }
}
