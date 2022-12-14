using DataLayer;
using DataLayer.Models;
using HotelManagementSystem.Models.GuestRanks;
using HotelManagementSystem.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMSCore.Services
{
    public class GuestRanksService : IGuestRanksService
    {
        private readonly HotelManagementDbContext db;

        public GuestRanksService(HotelManagementDbContext dBase)
        {
            this.db = dBase;
        }

        public async Task Add(AddRankFormModel rank)
        {
            var currentRank = new Rank
            {
                Name = rank.Name,
                Discount = rank.Discount,
            };

            await this.db.Ranks.AddAsync(currentRank);
            await this.db.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var allGuestsWithThisRank = this.db
                .Guests
                .Where(g => g.RankId == id)
                .ToList();

            var defaultRank = this.db
                .Ranks
                .FirstOrDefault(r => r.Discount == 0);

            foreach (var guest in allGuestsWithThisRank)
            {
                guest.Rank = defaultRank;
            }

            this.db.Guests.UpdateRange(allGuestsWithThisRank);

            var rank = this.db.Ranks.FirstOrDefault(r => r.Id == id);

            rank.Deleted = true;

            this.db.Ranks.Update(rank);
            await this.db.SaveChangesAsync();
        }

        public EditRankFormModel GetRank(string id)
        {
            return this.db
                .Ranks
                .Where(r => r.Id == id)
                .Select(r => new EditRankFormModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Discount = r.Discount
                })
                .FirstOrDefault();
        }



        public IEnumerable<AllRanksViewModel> GetAllRanks()
        {
            return this.db.Ranks
                .Where(r => r.Deleted == false)
                .Select(r => new AllRanksViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Discount = r.Discount
                })
                .OrderBy(r => r.Discount)
                .ToList();
        }

        public bool IsNameExistWhenAdd(string name)
        {
            return this.db.Ranks.Where(r => r.Deleted == false).Any(r => r.Name == name);
        }

        public bool IsNameExistWhenEdit(string name, string id)
        {
            return this.db
                .Ranks
                .Where(r => r.Id != id && r.Deleted == false)
                .Any(r => r.Name == name);
        }

        public async Task Edit(EditRankFormModel rank)
        {
            var currentRank = this.db
                .Ranks
                .Where(r => r.Id == rank.Id)
                .FirstOrDefault();

            currentRank.Name = rank.Name;
            currentRank.Discount = rank.Discount;

            this.db.Update(currentRank);
            await this.db.SaveChangesAsync();
        }
    }
}