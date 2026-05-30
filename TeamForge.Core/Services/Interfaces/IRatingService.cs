using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Ratings;

namespace TeamForge.Core.Services.Interfaces;

public interface IRatingService
{
    Task<RatingDto> CreateRatingAsync(
        int authorId,
        CreateRatingDto dto);

    Task<List<RatingDto>> GetUserRatingsAsync(int userId);
}