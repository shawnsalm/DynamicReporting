using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace DynamicReporting
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Security> datalist = new List<Security>();

            datalist.Add(
                new Security() { PortUID = "P851", CUSIP = "123456789", Sector = "A", MKV = 100000, MaturityDate = new DateTime(2020, 1, 1), PortTotalMarketValue = 800000 });

            datalist.Add(
                new Security() { PortUID = "P851", CUSIP = "234567891", Sector = "A", MKV = 200000, MaturityDate = new DateTime(2019, 1, 1), PortTotalMarketValue = 800000 });

            datalist.Add(
                new Security() { PortUID = "P851", CUSIP = "345678912", Sector = "A", MKV = 300000, MaturityDate = new DateTime(2020, 1, 1), PortTotalMarketValue = 800000 });

            datalist.Add(
                new Security() { PortUID = "P851", CUSIP = "456789123", Sector = "B", MKV = 200000, MaturityDate = new DateTime(2024, 1, 1), PortTotalMarketValue = 800000 });


            Hierarchy durationHierarchy = new Hierarchy();
            durationHierarchy.Name = "Duration Hierarchy";
            durationHierarchy.HierarchyNodes = new List<HierarchyNode>();

            durationHierarchy.HierarchyNodes.Add(new HierarchyNode()
            {
                Condition = "Duration >= 0 && Duration < 1",
                Name = "Duration >= 0 && Duration < 1"
            });

            durationHierarchy.HierarchyNodes.Add(new HierarchyNode()
            {
                Condition = "Duration >= 1 && Duration < 3",
                Name = "Duration >= 1 && Duration < 3"
            });

            durationHierarchy.HierarchyNodes.Add(new HierarchyNode()
            {
                Condition = "Duration >= 3 && Duration < 6",
                Name = "Duration >= 3 && Duration < 6"
            });
                        
            Report report = new Report();
            report.Name = "Test Report";
            report.SecurityBucketFactory = new HierarchySecurityBucketFactory(new GroupSecurityBucketFactory(null, "Sector"), durationHierarchy);
            report.StatisticsFields = new List<StatisticsField>();
            report.StatisticsFields.Add(new StatisticsField()
            {
                DisplayName = "%MKV",
                FieldName = "PercentageMKV",
                Statistic = "SUM(PercentageMKV)"
            });


            var x = "";

            

            Console.ReadLine();

        }

    }

    class ReportFactory
    {

    }

    class Report
    {
        public string Name { get; set; }
        public List<StatisticsField> StatisticsFields { get; set; }
        public ISecurityBucketFactory SecurityBucketFactory { get; set; }
    }

    class ReportDataCreator
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public HashSet<string> SideNames { get; set; }
        public HashSet<string> TopNames { get; set; }

        public ReportDataCreator(Report report, List<Security> securities)
        {
            SideNames = new HashSet<string>();
            TopNames = new HashSet<string>();

            var securityBuckets = report.SecurityBucketFactory.CreateSecurityBucket(securities, null);
                       
            foreach (var securityBucket in securityBuckets)
            {


                foreach (var childSecurityBucket in securityBucket.ChildrenSecurityBucket)
                {

                }

            }

            // get totals for whole report
            dynamic fieldsData = CreateFieldData(securities, report);

            foreach(var field in report.StatisticsFields)
            {
                var key = CreateKey(field.GetType().GetProperty("PortUID").GetValue(field, null).ToString(),
                            null, null, field.FieldName);

                _data.Add(key, field.GetType().GetProperty(field.FieldName).GetValue(field, null));
            }

        }

        public string CreateKey(string portUID, string sideName = "", string topName = "", string fieldName = "")
        {
            return portUID + "~" + sideName ?? "" + "~" + topName ?? "" + "~" + fieldName ?? "";
        }

        public object GetData(string key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key];
            }

            throw new Exception("Invalid key: " + key);
        }

        private dynamic CreateFieldData(List<Security> securities, Report report)
        {
            string fields = string.Join(",", report.StatisticsFields.Select(f => f.Statistic + " as " + f.FieldName));

            return securities.AsQueryable()
                    .GroupBy("new(PortUID)", "it")
                    .Select("new(Key.PortUID, " + fields)
                    .Cast<dynamic>().Select(s1 => s1).FirstOrDefault();
        }
    }

    class StatisticsField
    {
        public string Statistic { get; set; }
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
    }

    class Security
    {
        public string PortUID { get; set; }

        public string CUSIP { get; set; }

        public string Sector { get; set; }

        public decimal MKV { get; set; }

        public DateTime MaturityDate { get; set; }

        public decimal PortTotalMarketValue { get; set; }

        public double Duration
        {
            get
            {
                return MaturityDate.Subtract(DateTime.Today).TotalDays / 365.25;
            }
        }

        public decimal PercentageMKV
        {
            get
            {
                return (MKV / PortTotalMarketValue) * 100;
            }
        }
    }

    class SecurityBucket
    {
        public SecurityBucket ParentSecurityBucket { get; set; }
        public List<Security> Securities { get; set; }
        public string Name { get; set; }
        public string PortUID { get; set; }
        public List<SecurityBucket> ChildrenSecurityBucket { get; set; }
    }

    class Hierarchy
    {
        public string Name { get; set; }
        public List<HierarchyNode> HierarchyNodes { get; set; }
    }

    class HierarchyNode
    {
        public string Name { get; set; }
        public string Condition { get; set; }
    }
      

    interface ISecurityBucketFactory
    {
        List<SecurityBucket> CreateSecurityBucket(List<Security> securities, SecurityBucket parentSecurityBucket);
    }

    class HierarchySecurityBucketFactory : ISecurityBucketFactory
    {
        private ISecurityBucketFactory _securityBucketFactory;

        private Hierarchy _hierarchy;

        public HierarchySecurityBucketFactory(ISecurityBucketFactory securityBucketFactory, Hierarchy hierarchy)
        {
            _securityBucketFactory = securityBucketFactory;
            _hierarchy = hierarchy;
        }

        public List<SecurityBucket> CreateSecurityBucket(List<Security> securities, SecurityBucket parentSecurityBucket)
        {
            List<SecurityBucket> securityBuckets = new List<SecurityBucket>();

            foreach(var hierarchyNode in _hierarchy.HierarchyNodes)
            {
                var filteredSecurities = securities.AsQueryable().Where(hierarchyNode.Condition);
                
                var groupBysSecurities = filteredSecurities.AsQueryable().GroupBy("new(PortUID)", "it");

                foreach (var groupBySecurities in groupBysSecurities.Cast<IGrouping<DynamicClass, Security>>().Select(s => s))
                {
                    SecurityBucket securityBucket = new SecurityBucket();
                    securityBucket.ParentSecurityBucket = parentSecurityBucket;
                    securityBucket.Securities = groupBySecurities.ToList();
                    securityBucket.PortUID = ((dynamic)groupBySecurities.Key).PortUID;
                    securityBucket.Name = hierarchyNode.Name;

                    if (_securityBucketFactory != null)
                    {
                        securityBucket.ChildrenSecurityBucket = _securityBucketFactory.CreateSecurityBucket(securityBucket.Securities, securityBucket);
                    }

                    securityBuckets.Add(securityBucket);
                }
            }

            return securityBuckets;
        }
    }

    class GroupSecurityBucketFactory : ISecurityBucketFactory
    {
        private ISecurityBucketFactory _securityBucketFactory;

        private string _groupBy;
        
        public GroupSecurityBucketFactory(ISecurityBucketFactory securityBucketFactory, string groupBy)
        {
            _securityBucketFactory = securityBucketFactory;
            _groupBy = groupBy;
        }
        
        public List<SecurityBucket> CreateSecurityBucket(List<Security> securities, SecurityBucket parentSecurityBucket)
        {
            List<SecurityBucket> securityBuckets = new List<SecurityBucket>();

            var groupBysSecurities = securities.AsQueryable().GroupBy("new(PortUID, " + _groupBy + ")", "it");
            
            foreach (var groupBySecurities in groupBysSecurities.Cast<IGrouping<DynamicClass, Security>>().Select(s => s))
            {
                SecurityBucket securityBucket = new SecurityBucket();
                securityBucket.ParentSecurityBucket = parentSecurityBucket;
                securityBucket.Securities = groupBySecurities.ToList();
                securityBucket.PortUID = ((dynamic)groupBySecurities.Key).PortUID;
                securityBucket.Name = groupBySecurities.Key.GetType().GetProperty(_groupBy).GetValue(groupBySecurities.Key, null).ToString();
                
                if(_securityBucketFactory != null)
                {
                    securityBucket.ChildrenSecurityBucket = _securityBucketFactory.CreateSecurityBucket(securityBucket.Securities, securityBucket);
                }

                securityBuckets.Add(securityBucket);
            }

            return securityBuckets;
        }
    }

}
