using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public PositionJob[] PositionJobs { get; set; } = Array.Empty<PositionJob>();
    }
}
