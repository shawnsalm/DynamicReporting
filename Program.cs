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

            ISecurityBucketFactory securityBucketFactory = new HierarchySecurityBucketFactory(new GroupSecurityBucketFactory(null, "Sector"), durationHierarchy);

            var s = securityBucketFactory.CreateSecurityBucket(datalist, null);


            var f = s[0].Securities.AsQueryable()
                .GroupBy("new(PortUID)", "it")
                .Select("new(Key.PortUID, SUM(PercentageMKV) as PercentageMKV)");






            var x = "";

            /*

            var hierarchyBucket01Securities = datalist.AsQueryable()
                .Where("Duration >= 0 && Duration < 1");



            foreach (Security s in hierarchyBucket01Securities)

            {

                Console.WriteLine(s.PercentageMKV.ToString());

                Console.WriteLine(s.Duration.ToString());

            }



            var hierarchyBucket13Securities = datalist.AsQueryable()

                .Where("Duration >= 1 && Duration < 3");



            foreach (Security s in hierarchyBucket13Securities)

            {

                Console.WriteLine(s.PercentageMKV.ToString());

                Console.WriteLine(s.Duration.ToString());

            }





            var hierarchyBucket36Securities = datalist.AsQueryable()

                .Where("Duration >= 3 && Duration < 6");



            foreach (Security s in hierarchyBucket36Securities)

            {

                Console.WriteLine(s.PercentageMKV.ToString());

                Console.WriteLine(s.Duration.ToString());

            }



            var groupHierarchyBucket01Securities = hierarchyBucket01Securities.AsQueryable()

              .GroupBy("new(PortUID, Sector)", "it")

               .Select("it");





            foreach (var s in groupHierarchyBucket01Securities.Cast<IGrouping<DynamicClass, Security>>().Select(s => s))

            {

                Console.WriteLine(s.Key);



                foreach (var sec in s)

                {

                    Console.WriteLine(sec.PercentageMKV.ToString());

                    Console.WriteLine(sec.Duration.ToString());

                }

            }



            var groupHierarchyBucket13Securities = hierarchyBucket13Securities.AsQueryable()

                .GroupBy("new(PortUID, Sector)", "it");



            foreach (var s in groupHierarchyBucket13Securities.Cast<IGrouping<DynamicClass, Security>>().Select(s => s))

            {

                Console.WriteLine(s.Key);



                foreach (var sec in s)

                {

                    Console.WriteLine(sec.PercentageMKV.ToString());

                    Console.WriteLine(sec.Duration.ToString());

                }

            }



            var groupHierarchyBucket36Securities = hierarchyBucket36Securities.AsQueryable()

                .GroupBy("new(PortUID, Sector)", "it");



            foreach (var s in groupHierarchyBucket36Securities.Cast<IGrouping<DynamicClass, Security>>().Select(s => s))

            {

                Console.WriteLine(((dynamic)s.Key).PortUID);

                Console.WriteLine(s.Key.GetType().GetProperty("Sector").GetValue(s.Key, null));

                foreach (var sec in s)

                {

                    Console.WriteLine(sec.PercentageMKV.ToString());

                    Console.WriteLine(sec.Duration.ToString());

                }

            }
            */










            /*

            var hierarchyBucket01 = datalist.AsQueryable()

                .Where("Duration >= 0 && Duration < 1")

                .GroupBy("new(PortUID)", "it")

                .Select("new(Key.PortUID, SUM(PercentageMKV) as PercentageMKV)");

 

            foreach (object o in hierarchyBucket01)

                Console.WriteLine(o.ToString());

           

            var hierarchyBucket13 = datalist.AsQueryable()

                .Where("Duration >= 1 && Duration < 3")

                .GroupBy("new(PortUID)", "it")

                .Select("new(Key.PortUID, SUM(PercentageMKV) as PercentageMKV)");

 

            foreach (object o in hierarchyBucket13)

                Console.WriteLine(o.ToString());

 

 

            var hierarchyBucket36 = datalist.AsQueryable()

                .Where("Duration >= 3 && Duration < 6")

                .GroupBy("new(PortUID)", "it")

                .Select("new(Key.PortUID, SUM(PercentageMKV) as PercentageMKV)");

                       

            foreach (object o in hierarchyBucket36)

                Console.WriteLine(o.ToString());

 

 

            var groupHierarchyBucket01 = hierarchyBucket01.AsQueryable()

              .GroupBy("new(PortUID, Sector)", "it")

              .Select("new(Key.PortUID, Key.Sector, SUM(PercentageMKV) as PercentageMKV)");

 

            foreach (object o in groupHierarchyBucket01)

                Console.WriteLine(o.ToString());

 

            var groupHierarchyBucket13 = hierarchyBucket13.AsQueryable()

                .GroupBy("new(PortUID, Sector)", "it")

                .Select("new(Key.PortUID, Key.Sector, SUM(PercentageMKV) as PercentageMKV)");

 

            foreach (object o in groupHierarchyBucket13)

                Console.WriteLine(o.ToString());

 

 

            var groupHierarchyBucket36 = hierarchyBucket36.AsQueryable()

                .GroupBy("new(PortUID, Sector)", "it")

                .Select("new(Key.PortUID, Key.Sector, SUM(PercentageMKV) as PercentageMKV)");

 

            foreach (object o in groupHierarchyBucket36)

                Console.WriteLine(o.ToString());

            */

            Console.ReadLine();

        }

    }

    class Report
    {
        public string Name { get; set; }
        public List<string> StatisticsFields { get; set; }
        public ISecurityBucketFactory SideSecurityBucketFactory { get; set; }
        public ISecurityBucketFactory TopSecurityBucketFactory { get; set; }
    }

    class ReportHelper
    {

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
