using LocationFindingService.Models.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;

namespace LocationFindingService.Data
{
    /// <summary>
    /// Implementation of the <see cref="ILocationRepository"/> using an EF database context
    /// </summary>
    public class LocationRepository : ILocationRepository
    {
        private readonly DatabaseContext _context;

        public LocationRepository(DatabaseContext dbContext)
        {
            _context = dbContext;
        }

        /// <inheritdoc />
        public async Task<bool> AddIpAddress(string ipAddress, CancellationToken token)
        {
            try
            {
                if (!await _context.IPAddresses.AnyAsync(ip => ip.IpAddress == ipAddress, token))
                {
                    await _context.AddAsync(new IPAddress(ipAddress), token);
                    await _context.SaveChangesAsync(token);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqliteException sqlLiteEx && sqlLiteEx.SqliteExtendedErrorCode == (int)SQLiteErrorCode.Constraint_Unique)
                {
                    return false;
                }

                throw;
            }
        }
    }
}
