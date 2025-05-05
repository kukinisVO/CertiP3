using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.GiftServices.Models
{
    public class Gift
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public JsonElement Data { get; set; } // Flexible handling
    }
}
