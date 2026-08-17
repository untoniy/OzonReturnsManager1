using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonReturnsManager1.Models
{
    public class ReturnRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // "Возврат" или "Вывоз"
        public string ShopName { get; set; }
        public string OfferId { get; set; }
        public string OurStatus { get; set; }
        public int OrgType { get; set; }

        // Дополнительные поля для возвратов
        public string OzonReturnId { get; set; }
        public string PostingNumber { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        // Дополнительные поля для вывоза
        public string ReturnId { get; set; }
        public string BoxId { get; set; }
        public string BoxState { get; set; }
        public string ReturnState { get; set; }
    }
}
