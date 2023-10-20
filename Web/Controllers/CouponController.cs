using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.IService;
using Web.Models;

namespace Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? list = new();

            ResponseDto? responseDto = await _couponService.GetAllCouponsAsync();

            if(responseDto != null && responseDto.IsSuccess) 
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(
                    Convert.ToString(responseDto.Result));

            }
            return View(list);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid) 
            { 
                ResponseDto? response = await _couponService.CreateCouponsAsync(model);
                if(response!= null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);

            if (response != null && response.IsSuccess)
            {
                CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
    }
}
