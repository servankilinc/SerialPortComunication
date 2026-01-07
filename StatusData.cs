using System;
using System.Reflection;

namespace ConsoleApp1
{
    public class StatusData<T>
    {
        public T Entity { get; set; }
        private Enums.StatusEnum _status;
        public Enums.StatusEnum Status
        {
            get { return _status; }
            set
            {
                _status = value;

            }

        }
        public string Code { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; internal set; }
        public MethodBase MethodBase { get; internal set; }
    }
}
namespace Enums
{
    public enum StatusEnum
    {
        [System.ComponentModel.Description("Hata Oluştu")]
        Error = 0,
        [System.ComponentModel.Description("Başarılı")]
        Successful = 1,
        [System.ComponentModel.Description("Veri Yok")]
        EmptyData = 2,
        [System.ComponentModel.Description("Kayıt Zaten Mevcut")]
        RecordExist = 3,
        [System.ComponentModel.Description("Uyarı")]
        Warning = 4
    }
}
