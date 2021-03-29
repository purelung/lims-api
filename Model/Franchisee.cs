using System;
using System.Collections.Generic;

namespace ZeeReportingApi.Model
{
    public partial class Franchisee
    {
        public int Id { get; set; }
        public int FranchiseId { get; set; }
        public string FranchiseeName { get; set; }
        public string OwnerName { get; set; }
    }
}
