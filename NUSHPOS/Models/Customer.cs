using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("CustomerFiles")]
public class Customer
{
    [Key]
    public int AutoID { get; set; }
    public int CustomerID { get; set; }
    public string? CustomerKey { get; set; }
    public bool? CustomerIsActive { get; set; } = true;
    public string? CustomerName { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CardNumber { get; set; }
    public string? CustomerNotes { get; set; }
    public bool? AllowHouseAccount { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? SpecialBonusPercent { get; set; }
    public decimal? TotalDebt { get; set; }
    public decimal? TotalPayment { get; set; }
    public decimal? TotalRemainig { get; set; }
    public decimal? BonusStartupValue { get; set; }
    public decimal? TotalBonusUsed { get; set; }
    public decimal? TotalBonusEarned { get; set; }
    public decimal? TotalBonusRemaing { get; set; }
    
    public string? CityName { get; set; }
    public string? District { get; set; }
    public string? Neighborhood { get; set; }
    public string? Street { get; set; }
    public string? Buildings { get; set; }
    public string? Block { get; set; }
    public string? Apartment { get; set; }
    public string? ApartmentNo { get; set; }
    public string? FlatNo { get; set; }
    public string? AddressNotes { get; set; }
    
    [Column("AreaCode")]
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public int? BranchID { get; set; }
    public bool? IsEmployee { get; set; }

    [NotMapped]
    public string? DisplayPhoneNumber { get; set; }

    [NotMapped]
    public int TotalOrders { get; set; }

    [NotMapped]
    public decimal TotalSpent { get; set; }

    [NotMapped]
    public string FullAddressDisplay
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(District)) parts.Add(District);
            if (!string.IsNullOrWhiteSpace(Neighborhood)) parts.Add(Neighborhood);
            if (!string.IsNullOrWhiteSpace(Street)) parts.Add(Street);
            if (!string.IsNullOrWhiteSpace(Buildings)) parts.Add($"Bina: {Buildings}");
            if (!string.IsNullOrWhiteSpace(Block)) parts.Add($"Blok: {Block}");
            if (!string.IsNullOrWhiteSpace(FlatNo) || !string.IsNullOrWhiteSpace(ApartmentNo)) 
                parts.Add($"Mənzil: {FlatNo ?? ApartmentNo}");
            if (!string.IsNullOrWhiteSpace(AddressNotes)) parts.Add(AddressNotes);
            if (!string.IsNullOrWhiteSpace(CustomerNotes)) parts.Add(CustomerNotes);

            return parts.Count > 0 ? string.Join(", ", parts) : "Ünvan qeyd edilməyib";
        }
    }
}
