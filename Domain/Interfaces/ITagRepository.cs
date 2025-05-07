using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllTagAsync(int UserId);

        Task<Tag> GetTagsByIdsAsync(int id);
    }
}
