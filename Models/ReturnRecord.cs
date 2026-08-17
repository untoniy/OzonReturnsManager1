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

        // Вычисляемые свойства: Бренд и Артикул из OfferId
        public string Brand
        {
            get
            {
                if (string.IsNullOrEmpty(OfferId))
                    return string.Empty;

                var parts = OfferId.Split('_');
                
                if (parts.Length == 2)
                {
                    // Один символ подчеркивания: бренд перед ним
                    return parts[0];
                }
                else if (parts.Length >= 3)
                {
                    // Два или более подчеркиваний: бренд между первым и вторым
                    return parts[1];
                }
                
                return string.Empty;
            }
        }

        public string Article
        {
            get
            {
                if (string.IsNullOrEmpty(OfferId))
                    return string.Empty;

                // Артикул всегда между последним подчеркиванием и слэшем
                var lastUnderscoreIndex = OfferId.LastIndexOf('_');
                var slashIndex = OfferId.IndexOf('/');
                
                if (lastUnderscoreIndex != -1 && slashIndex != -1 && lastUnderscoreIndex < slashIndex)
                {
                    return OfferId.Substring(lastUnderscoreIndex + 1, slashIndex - lastUnderscoreIndex - 1);
                }
                
                return string.Empty;
            }
        }
    }
}
