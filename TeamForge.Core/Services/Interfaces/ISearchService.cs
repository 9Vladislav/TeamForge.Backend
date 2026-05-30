using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Search;

namespace TeamForge.Core.Services.Interfaces;

public interface ISearchService
{
    Task<List<SearchUserResultDto>> SearchUsersAsync(int currentUserId, SearchUsersRequestDto dto);
}