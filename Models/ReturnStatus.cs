using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonReturnsManager1.Models
{
    public enum EOurReturnStatus
    {
        NEW = 0,
        ACCEPTED,
        DISPUTE,
        C1ACCEPTED,
        WRITTENOFF
    }

    public static class ReturnStatusExtensions
    {
        public static string ToRussianString(this EOurReturnStatus status)
        {
            switch (status)
            {
                case EOurReturnStatus.NEW:
                    return "НОВЫЙ";
                case EOurReturnStatus.ACCEPTED:
                    return "ОПРИХОДОВАН";
                case EOurReturnStatus.DISPUTE:
                    return "ОТКРЫТ СПОР";
                case EOurReturnStatus.C1ACCEPTED:
                    return "1c ОПРИХОДОВАН";
                case EOurReturnStatus.WRITTENOFF:
                    return "СПИСАН";
                default:
                    return string.Empty;
            }
        }

        public static EOurReturnStatus FromRussianString(string russianStatus)
        {
            switch (russianStatus)
            {
                case "НОВЫЙ":
                    return EOurReturnStatus.NEW;
                case "ОПРИХОДОВАН":
                    return EOurReturnStatus.ACCEPTED;
                case "ОТКРЫТ СПОР":
                    return EOurReturnStatus.DISPUTE;
                case "1c ОПРИХОДОВАН":
                    return EOurReturnStatus.C1ACCEPTED;
                case "СПИСАН":
                    return EOurReturnStatus.WRITTENOFF;
                default:
                    throw new ArgumentException($"Неизвестный статус: {russianStatus}");
            }
        }

        public static string[] GetAllRussianStatuses()
        {
            return new string[]
            {
                "НОВЫЙ",
                "ОПРИХОДОВАН",
                "ОТКРЫТ СПОР",
                "1c ОПРИХОДОВАН",
                "СПИСАН"
            };
        }
    }
}
