using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static Discount.Grpc.Protos.DiscountProtoService;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoServiceBase
    {
        public readonly IDiscountRepository _repo;
        public readonly ILogger<DiscountService> _logger;
        public readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repo, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repo=repo;
            _logger=logger;
            _mapper=mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repo.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with {request.ProductName} not found"));
            }

            _logger.LogInformation
                ($"Discount is retreived for ProductName : {coupon.ProductName}, Amount : {coupon.Amount}");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon =  _mapper.Map<Coupon>(request.Coupon);
             await _repo.CreateDiscount(coupon);
            _logger.LogInformation($"Discount is successfully created ProductName : {coupon.ProductName}");

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repo.UpdateDiscount(coupon);
            _logger.LogInformation($"Discount is successfully updated ProductName : {coupon.ProductName}");

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repo.DeleteDiscount(request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };
            return response;
        }
    }
}
