using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Domain
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid Assignment { get; set; }
        public Guid User { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Comment { get; set; }
        public float Mark { get; set; }
        public string FilePath { get; set; }
    }
}
