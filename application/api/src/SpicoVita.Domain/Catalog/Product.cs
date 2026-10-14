using SpicoVita.Domain.Common;
namespace SpicoVita.Domain.Catalog;

public class Product : AuditableEntity
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string HsnCode { get; set; } = "0713";
    public decimal GstRatePercent { get; set; } = 5m;
    public int PackSizeGrams { get; set; }
    public decimal Mrp { get; set; }
    public bool IsActive { get; set; } = true;
}
