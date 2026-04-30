using Microsoft.EntityFrameworkCore;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class ReviewService
    {
        public async Task<ApiResponse<ReviewDto>> AddOrUpdateReviewAsync(ReviewCreateUpdateDto dto, int userId)
        {
            var review = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .FirstOrDefaultAsync(r => r.ProductId == dto.ProductId && r.UserId == userId);

            if (review == null)
            {
                review = new Review
                {
                    ProductId = dto.ProductId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Review>().AddAsync(review);
            }
            else
            {
                review.Rating = dto.Rating;
                review.Comment = dto.Comment;
                review.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Review>().Update(review);
            }

            await _unitOfWork.CompleteAsync();

            var result = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == review.Id);

            return new ApiResponse<ReviewDto>(_mapper.Map<ReviewDto>(result), "Review saved successfully");
        }
    }
}
