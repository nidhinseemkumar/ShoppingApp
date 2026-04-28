using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Repositories;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public interface IReviewService
    {
        Task<ApiResponse<ReviewDto>> AddOrUpdateReviewAsync(ReviewCreateUpdateDto dto, int userId);
        Task<ApiResponse<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId);
        Task<ReviewDto?> GetUserReviewForProductAsync(int productId, int userId);
        Task<ApiResponse<IEnumerable<ReviewDto>>> GetAllRecentReviewsAsync(int count = 50);
    }

    public class ReviewService(IUnitOfWork unitOfWork, IMapper mapper) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

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

            // Refresh to get navigation properties if needed (e.g. UserName)
            var result = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == review.Id);

            return new ApiResponse<ReviewDto>(_mapper.Map<ReviewDto>(result), "Review saved successfully");
        }

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
