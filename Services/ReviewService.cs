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

    public partial class ReviewService(IUnitOfWork unitOfWork, IMapper mapper) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
    }
}
