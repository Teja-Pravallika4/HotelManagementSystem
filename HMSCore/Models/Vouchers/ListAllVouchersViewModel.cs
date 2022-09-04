using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Models.Vouchers
{
    public class ListAllVouchersViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Discount { get; set; }

        public string Active { get; set; }
    }
}