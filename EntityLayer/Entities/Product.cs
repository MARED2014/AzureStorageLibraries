using Azure.Data.Tables;
using EntityLayer.Base;

namespace EntityLayer.Entities;

public class Product : BaseModel, ITableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockCount { get; set; }
}
