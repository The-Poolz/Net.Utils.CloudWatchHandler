using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Utils.CloudWatchHandler.Models
{
    public class MessageDetails
    {
        public string? ErrorLevel { get; set; }
        public string? Message { get; set; }
        public string? ApplicationName { get; set; }
    }
}
