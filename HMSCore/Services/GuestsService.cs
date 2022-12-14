using DataLayer;
using DataLayer.Models;
using DataLayer.Models.Enums;
using HotelManagementSystem.Models.Guests;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Services
{
    public class GuestsService : IGuestsService
    {
        private readonly HotelManagementDbContext db;

        public GuestsService(HotelManagementDbContext dBase)
        {
            this.db = dBase;
        }

        public ICollection<SelectListItem> GetCountries()
        {
            return this.db
                .Countries
                .Where(c => c.Deleted == false)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                })
                .ToList();
        }

        public ICollection<SelectListItem> GetRanks()
        {
            return this.db
                .Ranks
                .Where(r => r.Deleted == false)
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                })
                .ToList();
        }

        public async Task Add(AddCustomerFormModel customer)
        {
            var guest = new Guest
            {
                Address = customer.Address,
                CityId = customer.CityId,
                Details = customer.Details,
                Email = customer.Email,
                FirstName = customer.FirstName,
                IdentityCardId = customer.IdentityCardId,
                LastName = customer.LastName,
                Phone = customer.Phone,
                RankId = customer.RankId,
                Created = DateTime.UtcNow
            };

            await this.db.Guests.AddAsync(guest);
            await this.db.SaveChangesAsync();
        }

        public ListGuestsQueryModel GetGuests(ListGuestsQueryModel query)
        {
            var dBase = this.db.Guests.Where(g => g.Deleted == false);

            dBase = Search(query, dBase);
            dBase = Sort(query, dBase);

            var allPages = (int)Math.Ceiling((double)dBase.ToList().Count / query.ItemsPerPage);

            if (query.CurrentPage > allPages)
            {
                query.CurrentPage = allPages;
            }
            if (query.CurrentPage <= 0)
            {
                query.CurrentPage = 1;
            }

            var allGuests = dBase
                .Skip((query.CurrentPage - 1) * query.ItemsPerPage)
                .Take(query.ItemsPerPage)
                .Select(g => new ListGuestsViewModel
                {
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    Phone = g.Phone,
                    RankName = g.Rank.Name,
                    Id = g.Id,
                    Created = g.Created.Date,
                    City = g.City.Name
                })
                .ToList();

            var guestQueryModel = new ListGuestsQueryModel
            {
                CurrentPage = query.CurrentPage,
                AllGuests = allGuests,
                TotalPages = (int)Math.Ceiling((double)dBase.ToList().Count / query.ItemsPerPage),
                AscOrDesc = query.AscOrDesc
            };

            guestQueryModel.SortBy = query.SortBy;
            guestQueryModel.Search = query.Search;

            return guestQueryModel;
        }

        private IQueryable<Guest> Search(ListGuestsQueryModel query, IQueryable<Guest> currentDb)
        {
            if (string.IsNullOrWhiteSpace(query.Search))
            {
                return currentDb;
            }

            return currentDb
                .Where(g => (g.FirstName.ToLower() + " " + g.LastName.ToLower()).Contains(query.Search.ToLower()) ||
                        g.IdentityCardId.ToLower().Contains(query.Search.ToLower()) || g.Phone.ToLower().Contains(query.Search.ToLower()) ||
                        g.Rank.Name.ToLower().Contains(query.Search.ToLower()) || g.Email.ToLower().Contains(query.Search.ToLower()) ||
                        g.City.Name.ToLower().Contains(query.Search.ToLower()) || g.Address.ToLower().Contains(query.Search.ToLower()) ||
                        g.City.Country.Name.ToLower().Contains(query.Search.ToLower()));

        }

        private IQueryable<Guest> Sort(ListGuestsQueryModel query, IQueryable<Guest> dbase)
        {
            switch (query.SortBy)
            {
                case SortBy.None:
                    return dbase.OrderByDescending(g => g.Created);
                case SortBy.FName:
                    return query.AscOrDesc == 1 ? dbase.OrderBy(g => g.FirstName)
                        : dbase.OrderByDescending(g => g.FirstName);
                case SortBy.LName:
                    return query.AscOrDesc == 1 ? dbase.OrderBy(g => g.LastName)
                            : dbase.OrderByDescending(g => g.LastName);
                case SortBy.Phone:
                    return query.AscOrDesc == 1 ? dbase.OrderBy(g => g.Phone)
                            : dbase.OrderByDescending(g => g.Phone);
                case SortBy.Rank:
                    return query.AscOrDesc == 1 ? dbase.OrderBy(g => g.Rank.Name)
                            : dbase.OrderByDescending(g => g.Rank.Name);
                case SortBy.City:
                    return query.AscOrDesc == 1 ? dbase.OrderBy(g => g.City.Name)
                            : dbase.OrderByDescending(g => g.City.Name);
                case SortBy.CreatedOn:
                    return query.AscOrDesc == 1 ? dbase.OrderBy(g => g.Created)
                            : dbase.OrderByDescending(g => g.Created);
                default:
                    return dbase.OrderByDescending(g => g.Created);
            }

        }

        public bool IsCityExist(string cityId)
        {
            return this.db
                .Cities
                .Where(c => c.Deleted == false)
                .Any(c => c.Id == cityId);
        }

        public bool IsCountryExist(string countryId)
        {
            return this.db
                .Countries
                .Where(c => c.Deleted == false)
                .Any(c => c.Id == countryId);
        }

        public bool IsIdentityNumberExist(string identityNumber)
        {
            return this.db.Guests
                .Where(g => g.Deleted == false)
                .Any(g => g.IdentityCardId == identityNumber);
        }

        public bool IsRankExist(string rankId)
        {
            return this.db
                .Ranks
                .Where(r => r.Deleted == false)
                .Any(r => r.Id == rankId);
        }

        public DetailsGuestViewModel Details(string id)
        {
            return this.db.Guests
                .Where(g => g.Id == id)
                .Select(g => new DetailsGuestViewModel
                {
                    FirstName = g.FirstName,
                    Address = g.Address,
                    City = g.City.Name,
                    Country = g.City.Country.Name,
                    Created = g.Created.ToString("dd-MM-yyyy"),
                    CreatedReservationsCount = g.Reservations.Where(r => r.Status == ReservationStatus.Confirmed).Count(),
                    Details = g.Details,
                    Email = g.Email,
                    IdentityCardId = g.IdentityCardId,
                    LastName = g.LastName,
                    Phone = g.Phone,
                    Rank = g.Rank.Name,
                    Id = g.Id,
                }).FirstOrDefault();
        }

        public async Task Delete(string id)
        {
            await ChangeReservationStatus(id);

            var guest = this.db.Guests.Where(g => g.Id == id).FirstOrDefault();

            guest.Deleted = true;

            this.db.Update(guest);
            await this.db.SaveChangesAsync();
        }

        private async Task ChangeReservationStatus(string id)
        {
            this.db.
                Guests
                .Where(g => g.Id == id);


            var reservations = this.db
                .Reservations
                .Where(r => r.GuestId == id && r.EndDate >= DateTime.Now.Date)
                .ToList();

            var allInvoices = this.db
                .Invoices
                .Where(i => i.Status != InvoiceStatus.Canceled)
                .ToList();

            foreach (var res in reservations)
            {
                res.Status = ReservationStatus.Canceled;
                ChangeInvoiceStatus(res, allInvoices);
            }

            this.db.Reservations.UpdateRange(reservations);
            await this.db.SaveChangesAsync();
        }

        private void ChangeInvoiceStatus(Reservation res, IEnumerable<Invoice> invoices)
        {
            if (invoices.Any(a => a.ReservationId == res.Id))
            {
                var curInvoice = invoices.FirstOrDefault(i => i.ReservationId == res.Id);
                curInvoice.Status = InvoiceStatus.Canceled;
            }
        }

        public EditGuestFormModel GetGuest(string id)
        {
            var editGuest = this.db
                .Guests
                .Where(g => g.Id == id)
                .Select(g => new EditGuestFormModel
                {
                    Address = g.Address,
                    CityId = g.CityId,
                    Details = g.Details,
                    Email = g.Email,
                    FirstName = g.FirstName,
                    Id = g.Id,
                    IdentityCardId = g.IdentityCardId,
                    LastName = g.LastName,
                    Phone = g.Phone,
                    CountryId = g.City.CountryId,
                    RankId = g.RankId
                })
                .FirstOrDefault();

            editGuest.Countries = this.GetCountries();
            editGuest.Ranks = this.GetRanks();

            return editGuest;
        }

        public bool IsIdentityNumExistExceptSelf(string identityNumber, string id)
        {
            var dBase = this.db
                .Guests
                .Where(g => g.Id != id && g.Deleted == false)
                .AsQueryable();

            return dBase.Any(g => g.IdentityCardId == identityNumber);
        }

        public async Task Edit(EditGuestFormModel guest)
        {
            var currentGuest = this.db
                .Guests.FirstOrDefault(g => g.Id == guest.Id);

            currentGuest.FirstName = guest.FirstName;
            currentGuest.Address = guest.Address;
            currentGuest.CityId = guest.CityId;
            currentGuest.Details = guest.Details;
            currentGuest.Email = guest.Email;
            currentGuest.IdentityCardId = guest.IdentityCardId;
            currentGuest.LastName = guest.LastName;
            currentGuest.Phone = guest.Phone;
            currentGuest.RankId = guest.RankId;

            this.db.Update(currentGuest);
            await this.db.SaveChangesAsync();
        }
    }
}