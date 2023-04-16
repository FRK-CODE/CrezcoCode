using Microsoft.EntityFrameworkCore;

namespace LocationFindingService.Models.Data
{
    [PrimaryKey(nameof(IpAddress))]
    public class IPAddress
    {
        public string IpAddress { get; set; }

        public IPAddress(string ipAddress)
        {
            IpAddress = ipAddress;
        }
    }
}
