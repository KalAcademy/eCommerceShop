using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public double MinAmount { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        public string Description { get; set; }

    }
}
