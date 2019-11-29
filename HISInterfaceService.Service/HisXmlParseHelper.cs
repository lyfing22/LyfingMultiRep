using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Core.Enum;

namespace HISInterfaceService.Service
{
    public class HisXmlParseHelper
    {
        #region const objectName
        private const string ObjectOrder = "order";
        private const string ObjectProcedure = "procedure";
        private const string ObjectReport = "report";
        #endregion

        #region InputParametersMember_order
        private const string HisInterfaceMapperFileName = "HISInterfaceMapper.xml";
        private static readonly List<PropertyInfo> StringTypeOrderProperties = new Order().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(string)).ToList();

        private static readonly List<PropertyInfo> NullableDoubleTypeOrderProperties = new Order().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(double?)).ToList();

        private static readonly List<PropertyInfo> NullableDateTimeTypeOrderProperties = new Order().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(DateTime?)).ToList();

        private const string OrderRootNodeNamePath = "//HIS//InputParameters//OrderRootNodeName";
        private const string OrderFieldNodeNamePath = "//HIS//InputParameters//Entity//HISOrder/Field";

        /// <summary>
        /// XML方式传递时，XML Order的根节点名称
        /// </summary>
        public static string OrderRootNodeName { get; set; }
        /// <summary>
        /// XML方式传递时，XML Order节点名称和RISOrder字段属性之间的对应关系
        /// </summary>
        public static Dictionary<string, string> OrderFieldNameMapper { get; set; }
        #endregion

        #region InputParametersMember_procedure
        private static readonly List<PropertyInfo> StringTypeProcedureProperties = new Procedure().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(string)).ToList();

        private static readonly List<PropertyInfo> NullableDoubleTypeProcedureProperties = new Procedure().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(double?)).ToList();

        private static readonly List<PropertyInfo> NullableDateTimeTypeProcedureProperties = new Procedure().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(DateTime?)).ToList();

        private const string OrderProcedureRootNodeNamePath = "//HIS//OrderProcedureRootNodeName";
        private const string OrderProcedureFieldNodeNamePath = "//HIS//InputParameters//Entity//HISProcedure/Field";

        /// <summary>
        /// XML方式传递时，XML Order的检查项目根节点名称，可以为NULL(如果没有单独的检查项目节点)
        /// </summary>
        public static string OrderProcedureRootNodeName { get; set; }
        public static Dictionary<string, string> OrderProcedureFieldNameMapper { get; set; }
        #endregion

        #region InputParametersMember_patient
        private static readonly List<PropertyInfo> StringTypePatientProperties = new Patient().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(string)).ToList();

        private static readonly List<PropertyInfo> NullableDoubleTypePatientProperties = new Patient().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(double?)).ToList();

        private static readonly List<PropertyInfo> NullableDateTimeTypePatientProperties = new Patient().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite).Where(p => p.PropertyType == typeof(DateTime?)).ToList();

        private const string OrderPatientRootNodeNamePath = "//HIS//OrderPatientRootNodeName";
        private const string OrderPatientFieldNodeNamePath = "//HIS//InputParameters//Entity//HISPatient/Field";

        /// <summary>
        /// XML方式传递时，XML Order的检查项目根节点名称，可以为NULL(如果没有单独的检查项目节点)
        /// </summary>
        public static string OrderPatientRootNodeName { get; set; }
        public static Dictionary<string, string> OrderPatientFieldNameMapper { get; set; }
        #endregion

        #region OutputParametersMember
        private static readonly List<PropertyInfo> ReadStringTypeOrderProperties = new Order().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.CanRead).Where(p => p.PropertyType == typeof(string)).ToList();

        private static readonly List<PropertyInfo> ReadNullableDateTimeTypeOrderProperties = new Order().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead).Where(p => p.PropertyType == typeof(DateTime?)).ToList();

        private static readonly List<PropertyInfo> ReadStringTypeProcedureProperties = new Procedure().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.CanRead).Where(p => p.PropertyType == typeof(string)).ToList();

        private static readonly List<PropertyInfo> ReadNullableDateTimeTypeProcedureProperties = new Procedure().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead).Where(p => p.PropertyType == typeof(DateTime?)).ToList();

        private static readonly List<PropertyInfo> ReadStringTypeReportProperties = new Report().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.CanRead).Where(p => p.PropertyType == typeof(string)).ToList();

        private static readonly List<PropertyInfo> ReadNullableDateTimeTypeReportProperties = new Report().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead).Where(p => p.PropertyType == typeof(DateTime?)).ToList();

        private const string ConfirmExamOrderRootNodeNamePath = "//HIS//OutputParameters//ConfirmExam/OutputRootNodeName";
        private const string ConfirmExamFieldNamePath = "//HIS//OutputParameters//ConfirmExam//Entity//HISOrder/Field";

        private const string WriteReportRootNodeNamePath = "//HIS//OutputParameters//WriteReport/OutputRootNodeName";
        private const string WriteReportFieldNamePath = "//HIS//OutputParameters//WriteReport//Entity//HISOrder/Field";

        public static string ConfirmExamOrderRootNodeName { get; set; }
        public static Dictionary<string, string> ConfirmExamFieldNameMapper { get; set; }

        public static string WriteReportRootNodeName { get; set; }
        public static Dictionary<string, string> WriteReportFieldNameMapper { get; set; }

        #endregion

        #region construstor
        static HisXmlParseHelper()
        {
            OrderRootNodeName = null;
            OrderProcedureRootNodeName = null;
            ConfirmExamOrderRootNodeName = null;
            WriteReportRootNodeName = null;

            OrderFieldNameMapper = null;
            OrderProcedureFieldNameMapper = null;
            ConfirmExamFieldNameMapper = null;
            WriteReportFieldNameMapper = null;
        }
        #endregion

        #region LoadHISInterfaceMapper
        private static void GetHisMapperXml()
        {
            if (OrderRootNodeName == null)
                LoadFieldList();
        }

        private static void LoadFieldList()
        {
            try
            {
                var doc = new XmlDocument();
                var filename = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, HisInterfaceMapperFileName);
                doc.Load(filename);

                OrderRootNodeName = LoadSingleNodeValue(doc, OrderRootNodeNamePath);
                OrderProcedureRootNodeName = LoadSingleNodeValue(doc, OrderProcedureRootNodeNamePath);
                ConfirmExamOrderRootNodeName = LoadSingleNodeValue(doc, ConfirmExamOrderRootNodeNamePath);
                WriteReportRootNodeName = LoadSingleNodeValue(doc, WriteReportRootNodeNamePath);

                OrderFieldNameMapper = LoadFieldMapper(doc, OrderFieldNodeNamePath);
                OrderProcedureFieldNameMapper = LoadFieldMapper(doc, OrderProcedureFieldNodeNamePath);
                ConfirmExamFieldNameMapper = LoadFieldMapper(doc, ConfirmExamFieldNamePath);
                WriteReportFieldNameMapper = LoadFieldMapper(doc, WriteReportFieldNamePath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Init mapping from HISInterfaceMapper.xml failed.Exception:{0}", ex.Message));
            }
        }

        /// <summary>
        /// 获取HIS XML NodeName和RIS Entity field之间的对应关系Mapper
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadFieldMapper(XmlDocument document, string pathName)
        {
            var ret = new Dictionary<string, string>();
            var nodes = document.SelectNodes(pathName);
            if (nodes == null) return ret;
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null) continue;
                var fieldHisNodeName = node.Attributes["HISNodeName"].Value;
                var fieldPropertyName = node.Attributes["PropertyName"].Value;
                ret.Add(fieldHisNodeName, fieldPropertyName);
            }
            return ret;
        }

        public static string LoadSingleNodeValue(XmlDocument document, string pathName)
        {
            string ret = null;
            var oneNode = document.SelectSingleNode(pathName);
            if (oneNode != null)
                ret = oneNode.InnerText;
            return ret;
        }

        #endregion

        #region SyncFromHISToRIS

        /// <summary>
        /// 根据HIS输入的XML转换成RIS的一个Order
        /// </summary>
        /// <param name="prars"></param>
        /// <returns></returns>
        public static Order ParseOrderFromXml(string content)
        {
            GetHisMapperXml();
            var doc = new XmlDocument();
            doc.LoadXml(content);
            var nodes = doc.GetElementsByTagName(OrderRootNodeName);
            var order = new Order();

            if (nodes == null || nodes.Count == 0)
                return order;

            #region ParseOneOrder
            foreach (var dic in from XmlNode node in nodes
                                let childrenNodes = node.ChildNodes
                                where node.ChildNodes.Count != 0
                                select childrenNodes.Cast<XmlNode>().ToDictionary(
                                    childrenNode => childrenNode.Name, childrenNode => childrenNode.InnerText))
            {
                order = ParseSingleOrderFromXml(dic);
                if (string.IsNullOrEmpty(order.HisOrderCode))
                {
                    throw new Exception("HISOrderCode can not be empty!");
                }
            }
            #endregion

            #region ParseProceduresOfTheOrder
            nodes = doc.GetElementsByTagName(OrderProcedureRootNodeName);
            if ((nodes == null) || nodes.Count == 0)
                return order;

            var orderExamCodes = string.Empty;
            var orderExamNames = string.Empty;
            var examCosts = (float)0.00;

            var procedures = new EntityCollection<Procedure>();
            foreach (var procedure in from XmlNode node in nodes
                                      let childrenNodes = node.ChildNodes
                                      where node.ChildNodes.Count != 0
                                      select childrenNodes.Cast<XmlNode>().ToDictionary(
                                          childrenNode => childrenNode.Name, childrenNode => childrenNode.InnerText) into dic
                                      select ParseSingleOrderProcedureFromXml(order, dic))
            {
                if (!procedure.LastUpdateDateTime.HasValue)
                    procedure.LastUpdateDateTime = DateTime.Now;
                procedures.Add(procedure);
                orderExamCodes = string.Format("{0}|{1}", orderExamCodes, procedure.CheckItemCode);
                orderExamNames = string.Format("{0},{1}", orderExamNames, procedure.CheckItemName);
                if (procedure.Fee.HasValue)
                    examCosts = (float)(examCosts + procedure.Fee);

                if (string.IsNullOrEmpty(procedure.HisOrderCode))
                {
                    throw new Exception("HISOrderCode can not be empty!");
                }
            }

            if (!procedures.Any()) return order;
            order.OrderExamCode = orderExamCodes.Length > 0 ? orderExamCodes.Substring(1, orderExamCodes.Length - 1) : string.Empty;
            order.OrderExamName = orderExamNames.Length > 0 ? orderExamNames.Substring(1, orderExamNames.Length - 1) : string.Empty; ;
            order.ExamCosts = examCosts;
            //order.Procedure = procedures;
            #endregion

            return order;
        }

        /// <summary>
        /// 根据XML输入转换成一个Order
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Order ParseSingleOrderFromXml(Dictionary<string, string> data)
        {
            object order =new Order(Guid.NewGuid(), string.Empty, (int)OrderSyncStatus.OrderSynced, true, true, false);
            foreach (var e in data)
            {
                var datahandled = false;

                var propertyName = e.Key;
                if (OrderFieldNameMapper.ContainsKey(propertyName))
                {
                    propertyName = OrderFieldNameMapper[propertyName];
                }

                //处理所有字符串类型的属性
                datahandled = SetStringTypeProperyValue(StringTypeOrderProperties, propertyName, e.Value, ref order);

                if ((!datahandled))
                {
                    //处理所有double?类型的属性
                    datahandled = SetNullableDoubleTypeOrderPropertyValue(NullableDoubleTypeOrderProperties, propertyName, e.Value, ref order);
                }

                if ((!datahandled))
                {
                    //处理所有DateTime?类型的属性
                    datahandled = SetNullableDateTimeTypeOrderPropertyValue(NullableDateTimeTypeOrderProperties, propertyName, e.Value, ref order);
                }
                //if (!datahandled)
                //{
                //    throw new Exception(string.Format("Data Cannot Parsed Found. {0} : {1}.", e.Key, e.Value));
                //}
            }

            return order as Order;
        }

        /// <summary>
        /// 根据XML输入转换成一个Procedure
        /// </summary>
        /// <param name="order"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Procedure ParseSingleOrderProcedureFromXml(Order order, Dictionary<string, string> data)
        {
            object procedure =new  Procedure(Guid.NewGuid(), order.HisOrderCode, (int)OrderSyncStatus.OrderSynced, true, true, false, order.Id);
            foreach (var e in data)
            {
                var datahandled = false;

                var propertyName = e.Key;
                if (OrderProcedureFieldNameMapper.ContainsKey(propertyName))
                    propertyName = OrderProcedureFieldNameMapper[propertyName];

                //处理所有字符串类型的属性
                datahandled = SetStringTypeProperyValue(StringTypeProcedureProperties, propertyName, e.Value,
                    ref procedure);
                //处理所有double?类型的属性
                if ((!datahandled))
                    datahandled = SetNullableDoubleTypeOrderPropertyValue(NullableDoubleTypeProcedureProperties, propertyName, e.Value, ref procedure);

                //处理所有DateTime?类型的属性
                if ((!datahandled))
                    datahandled = SetNullableDateTimeTypeOrderPropertyValue(NullableDateTimeTypeProcedureProperties,
                        propertyName, e.Value, ref procedure);

                //if (!datahandled)
                //{
                //    throw new Exception(string.Format("Data Cannot Parsed Found. {0} : {1}.", e.Key, e.Value));
                //}
            }
            return (Procedure)procedure;
        }

        #endregion

        #region SyncFromRISToHIS
        /// <summary>
        ///  以"申请单"的"检查子项"为单位进行"检查确认"
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<string> ConfirmExamOrderProceduresToXml(Order order, EntityCollection<Procedure> Procedure, Report report)
        {
            GetHisMapperXml();
            return (from procedure in Procedure select CreateOneProcedureNodeXml(order, procedure, ConfirmExamFieldNameMapper,report) into oneRet select CombinRootNodeAndContentNode(ConfirmExamOrderRootNodeName, oneRet) into oneRet select string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}", oneRet)).ToList();
        }

        /// <summary>
        ///  以"申请单"的"检查子项"为单位进行"报告回写"
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<string> WriteReportProceduresToXml(Order order, EntityCollection<Procedure> Procedure, Report report)
        {
            GetHisMapperXml();
            return (from procedure in Procedure select CreateOneProcedureNodeXml(order, procedure, WriteReportFieldNameMapper,report) into oneRet select CombinRootNodeAndContentNode(WriteReportRootNodeName, oneRet) into oneRet select string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}", oneRet)).ToList();
        }

        private static string CreateOneProcedureNodeXml(Order order, Procedure procedure, Dictionary<string, string> oneDictionary,Report report)
        {
            var ret = string.Empty;
            foreach (var e in oneDictionary)
            {
                var propertyName = e.Value;
                var propertyNameArray = propertyName.Split(',');
                var objName = string.Empty;
                var value = string.Empty;
                if (propertyNameArray.Any())
                {
                    objName = propertyNameArray[0].ToLower();
                    propertyName = propertyNameArray[1];
                }
                switch (objName)
                {
                    case ObjectOrder:
                        value = GetOrderProperyValue(order, propertyName) as string;
                        break;
                    case ObjectProcedure:
                        value = GetProcedureProperyValue(procedure, propertyName) as string;
                        break;
                    case ObjectReport:
                        value = GetReportProperyValue(report, propertyName) as string;
                        break;
                }
                var oneNode = string.Format("<{0}>{1}</{0}>", e.Key, value);
                ret = string.Format("{0}{1}", ret, oneNode);
            }
            return ret;
        }

        private static string CombinRootNodeAndContentNode(string rootName, string contentNode)
        {
            var ret = contentNode;
            if (rootName == null) return ret;
            var rootNameArray = rootName.Split(',');
            var rootNameStart = string.Empty;
            var rootNameEnd = string.Empty;
            foreach (var oneNodeName in rootNameArray)
            {
                rootNameStart = string.Format("{0}<{1}>", rootNameStart, oneNodeName);
                rootNameEnd = string.Format("</{0}>{1}", oneNodeName, rootNameEnd);
            }
            ret = string.Format("{0}{1}{2}", rootNameStart, contentNode, rootNameEnd);
            return ret;
        }

        /// <summary>
        /// 以"申请单"为单位进行"检查确认",备用
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string ConfirmExamOrderToXml(Order order)
        {
            GetHisMapperXml();
            var ret = string.Empty;

            foreach (var e in ConfirmExamFieldNameMapper)
            {
                var datahandled = false;
                var propertyName = e.Value;
                var oneNode = string.Empty;
                foreach (var value in from propertyInfo in ReadStringTypeOrderProperties
                                      where string.Compare(propertyName, propertyInfo.Name, CultureInfo.InvariantCulture,
                                          CompareOptions.IgnoreCase) == 0
                                      select propertyInfo.GetValue(order, null))
                {
                    oneNode = string.Format("<{0}>{1}</{0}>", e.Key, value);
                    datahandled = true;
                    break;
                }

                if (!datahandled)
                {
                    foreach (var nodeValue in from propertyInfo in ReadNullableDateTimeTypeOrderProperties
                                              where string.Compare(propertyName, propertyInfo.Name, CultureInfo.InvariantCulture,
                                                  CompareOptions.IgnoreCase) == 0
                                              select propertyInfo.GetValue(order, null) into value
                                              select (value ?? string.Empty))
                    {
                        oneNode = string.Format("<{0}>{1}</{0}>", e.Key, nodeValue);
                        datahandled = true;
                        break;
                    }
                }
                ret = string.Format("{0}{1}", ret, oneNode);
            }

            if (ConfirmExamOrderRootNodeName != null)
                ret = string.Format("<{0}>{1}</{0}>", ConfirmExamOrderRootNodeName, ret);
            ret = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>{0}", ret);

            return ret;
        }
        #endregion

        #region SetProperyValue
        public static bool SetStringTypeProperyValue(List<PropertyInfo> listPropertyInfo, string propertyName, string value, ref object target)
        {
            var ret = false;
            //object.GetType().GetProperty(propertyName).GetType()
            //object.GetProperty(propertyName).SetValue(t, hisValue, null);
            try
            {
                foreach (
                    var propertyInfo in
                        listPropertyInfo.Where(
                            propertyInfo =>
                                string.Compare(propertyName, propertyInfo.Name, CultureInfo.InvariantCulture,
                                    CompareOptions.IgnoreCase) == 0))
                {
                    propertyInfo.SetValue(target, value, null);
                    ret = true;
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SetStringTypeProperyValue failed.The propertyName is {0}.Err:{1}", propertyName, ex.Message));
            }
            return ret;
        }

        public static bool SetNullableDoubleTypeOrderPropertyValue(List<PropertyInfo> listPropertyInfo, string propertyName, string value, ref object target)
        {
            var ret = false;
            try
            {
                foreach (var propertyInfo in listPropertyInfo.Where(propertyInfo => string.Compare(propertyName, propertyInfo.Name, CultureInfo.InvariantCulture,
                    CompareOptions.IgnoreCase) == 0))
                {
                    propertyInfo.SetValue(target, null, null);

                    double currentValue;
                    if (double.TryParse(value,
                        NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint |
                        NumberStyles.AllowLeadingWhite |
                        NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out currentValue))
                    {
                        propertyInfo.SetValue(target, currentValue, null);
                        ret = true;
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SetNullableDoubleTypeOrderPropertyValue failed.The propertyName is {0}.Err:{1}", propertyName, ex.Message));
            }
            return ret;
        }

        public static bool SetNullableDateTimeTypeOrderPropertyValue(List<PropertyInfo> listPropertyInfo, string propertyName, string value, ref object target)
        {
            var ret = false;
            try
            {
                foreach (var propertyInfo in listPropertyInfo.Where(propertyInfo => string.Compare(propertyName, propertyInfo.Name, CultureInfo.InvariantCulture,
                    CompareOptions.IgnoreCase) == 0))
                {
                    propertyInfo.SetValue(target, null, null);

                    DateTime time;
                    if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces,
                        out time))
                    {
                        propertyInfo.SetValue(target, time, null);
                        ret = true;
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SetNullableDateTimeTypeOrderPropertyValue failed.The propertyName is {0}.Err:{1}", propertyName, ex.Message));
            }
            return ret;
        }
        #endregion

        #region GetProperyValue
        private static string GetOrderProperyValue(Order order, string propertyName)
        {
            var ret = GetStringTypeProperyValue(ReadStringTypeOrderProperties, propertyName, order) ??
                         GetNullableDateTimeTypeProperyValue(ReadNullableDateTimeTypeOrderProperties, propertyName, order);
            return ret;
        }

        private static string GetProcedureProperyValue(Procedure procedure, string propertyName)
        {
            var ret = GetStringTypeProperyValue(ReadStringTypeProcedureProperties, propertyName, procedure) ??
                         GetNullableDateTimeTypeProperyValue(ReadNullableDateTimeTypeProcedureProperties, propertyName, procedure);
            return ret;
        }

        private static string GetReportProperyValue(Report report, string propertyName)
        {
            var ret = GetStringTypeProperyValue(ReadStringTypeReportProperties, propertyName, report) ??
                         GetNullableDateTimeTypeProperyValue(ReadNullableDateTimeTypeReportProperties, propertyName, report);
            return ret;
        }

        public static string GetStringTypeProperyValue(List<PropertyInfo> listPropertyInfo, string propertyName, object source)
        {
            try
            {
                return (from propertyInfo in listPropertyInfo where string.Compare(propertyName, propertyInfo.Name, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0 select propertyInfo.GetValue(source, null).ToString()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetStringTypeProperyValue failed.The propertyName is {0}.Err:{1}", propertyName, ex.Message));
                return string.Empty;
            }
        }

        public static string GetNullableDateTimeTypeProperyValue(List<PropertyInfo> listPropertyInfo, string propertyName, object source)
        {
            var ret = string.Empty;
            try
            {
                foreach (var value in from propertyInfo in listPropertyInfo
                                      where string.Compare(propertyName, propertyInfo.Name, CultureInfo.InvariantCulture,
                                          CompareOptions.IgnoreCase) == 0
                                      select propertyInfo.GetValue(source, null))
                {
                    ret = value == null
                        ? string.Empty
                        : DateTime.Parse(value.ToString()).ToString("yyyy-MM-dd hh:mm:ss");
                    break;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetNullableDateTimeTypeProperyValue failed.The propertyName is {0}.Err:{1}", propertyName, ex.Message));
            }
            return ret;
        }
        #endregion
    }
}
