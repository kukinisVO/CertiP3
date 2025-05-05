using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.GiftServices.Models
{
    public class Gift
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
