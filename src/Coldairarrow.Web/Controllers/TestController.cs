using Coldairarrow.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Coldairarrow.Web
{
    public class TestController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MapDatTest()
        {
            return View();
        }

        public ActionResult RequestDemo()
        {
            return View();
        }

        public ActionResult GetChartData()
        {
            double length = 0;
            LinkedList<double> pointList = new LinkedList<double>();
            List<double> rate = new List<double>(10);
            LoopHelper.Loop(10, () =>
            {
                rate.Add(0);
            });
            double minutes = 60;
            double count = 10.0 * 60 * 1000000 / 10;
            for (double i = 0; i < count; i++)
            {
                double s = 0.5861 * 1;
                length = length + s;
                double x = 0;
                if ((length / 20) % 1 < 0.5)
                {
                    x = length % 10;
                }
                else
                {
                    x = 10 - (length % 10);
                }
                rate[(int)x]++;
            }

            List<string> xData = new List<string>();
            List<double> yData = new List<double>();
            rate.ForEach((aValue, index) =>
            {
                xData.Add((index + 0.5).ToString());
                yData.Add(aValue);
            });
            var resData = new
            {
                XData = xData,
                YData = yData
            };
            return Content(resData.ToJson());
        }

        public ActionResult GetHKGeojson()
        {
            var file = Server.MapPath("~/Config/NingboGeojson.json");
            string res = System.IO.File.ReadAllText(file);
            return Content(res);
        }

        public ActionResult GetData()
        {
            return Content("Hello World");
        }

        #region 获取宁波行政区域地图的Geojson

        public ActionResult GetNingboGeojsonForm()
        {
            return View();
        }

        public ActionResult GetNingboGeojson(string obj)
        {
            var areas = obj.ToJArray();
            List<object> allAreas = new List<object>();
            areas.ForEach(aArea =>
            {
                
                var jObj = (JObject)aArea;
                string areaName = jObj["AreaName"]?.ToString();
                List<object> polygons = new List<object>();
                jObj["Points"].ToString().ToList<string>().ForEach(apo =>
                {
                    polygons.Add(GetPolygon(apo));
                });

                var newArea = new
                {
                    type = "Feature",
                    properties = new
                    {
                        name = areaName
                    },
                    geometry = new
                    {
                        type = "MultiPolygon",
                        coordinates = new List<object> { polygons }
                    }
                };
                allAreas.Add(newArea);
            });

            var geojson = new
            {
                type = "FeatureCollection",
                features = allAreas
            };

            return Content(geojson.ToJson());
        }

        private object GetPolygon(string points)
        {
            List<double[]> res = new List<double[]>();
            points = points.Replace(" ", "");
            points.Split(';').ForEach(aPoint =>
            {
                res.Add(aPoint.Split(',').Select(x => x.ToDouble()).ToArray());
            });

            return res;
        }

        #endregion
    }
}