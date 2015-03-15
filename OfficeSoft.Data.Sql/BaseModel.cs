using System;

namespace OfficeSoft.Data
{
    [Serializable]
    public abstract class BaseModel
    {
        public Guid Id { get; set; }
        public int IdDb { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}