using Microsoft.EntityFrameworkCore;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class ReviewService
    {
        public async Task<ApiResponse<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return new ApiResponse<IEnumerable<ReviewDto>>(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        public async Task<ReviewDto?> GetUserReviewForProductAsync(int productId, int userId)
        {
            var review = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ApiResponse<IEnumerable<ReviewDto>>> GetAllRecentReviewsAsync(int count = 50)
        {
            var reviews = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .Take(count)
                .ToListAsync();

            return new ApiResponse<IEnumerable<ReviewDto>>(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }
    }
}
