using System;
using System.Collections.Generic;

namespace ZeeReportingApi.Model
{
    public partial class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public int FranchiseId { get; set; }
        public int UserRoleId { get; set; }
    }
}
