using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        public readonly IConfiguration _config;

        public DiscountRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(
                _config.GetValue<string>("DatabaseSettings:ConnectionString"));

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                 ("Select * from coupon where productname=@productName",
                 new { ProductName = productName });

            if (coupon == null)
            {
                return new Coupon
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Description = "No Discount Desc"
                };
            }

            return coupon;
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(
                _config.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected =
               await connection.ExecuteAsync(@"insert into coupon (productname,description,amount)
                    values (@productName,@Description,@Amount)",
                    new Coupon
                    {
                        ProductName = coupon.ProductName,
                        Description = coupon.Description,
                        Amount = coupon.Amount
                    });
            if (affected == 0)
                return false;
            return true;

        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(
                 _config.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync(@"delete from coupon where productname=@productName",
                new { ProductName = productName });

            if (affected == 0)
                return false;
            return true;
        }



        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(
                  _config.GetValue<string>("DatabaseSettings:ConnectionString"));


            var affected = await connection.ExecuteAsync(@"update coupon set 
                productname = @ProductName, 
                description = @Description,
                amount = @Amount where id=@Id",
                new Coupon
                {
                    ProductName = coupon.ProductName,
                    Description = coupon.Description,
                    Amount = coupon.Amount,
                    Id = coupon.Id
                });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
