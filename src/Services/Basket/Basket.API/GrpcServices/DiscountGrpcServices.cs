using Discount.Grpc.Protos;
using System.Threading.Tasks;
using static Discount.Grpc.Protos.DiscountProtoService;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcServices
    {
        private readonly DiscountProtoServiceClient _discountProtoServices;
       
        public DiscountGrpcServices(DiscountProtoServiceClient _discount)
        {
            _discountProtoServices = _discount;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountService = new GetDiscountRequest { ProductName = productName };
            return await _discountProtoServices.GetDiscountAsync(discountService);  
        }
    }
}
