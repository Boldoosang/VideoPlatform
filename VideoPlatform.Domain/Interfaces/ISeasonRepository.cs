using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Domain.Interfaces {
    public interface ISeasonRepository {
        Task<Season?> GetSeasonAsync(int SeasonId);
        Task<IEnumerable<Season>> GetAllSeasonsAsync();
        Task AddSeasonAsync(Season season);
        Task UpdateSeasonAsync(Season season);
        Task DeleteSeasonByIdAsync(int SeasonId);
        Task<bool> SeasonExists(int SeasonId);
    }
}
