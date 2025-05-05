using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ClinicLogic.Models;

namespace Services.GiftServices.Managers
{
    public class GiftManager
    {
        private readonly AppConfig _config;
        public GiftManager(IOptions<AppConfig> config)
        {
            _config = config.Value;
        }

        public string GetUrl() => _config.GiftsApiUrl;
    }
}
