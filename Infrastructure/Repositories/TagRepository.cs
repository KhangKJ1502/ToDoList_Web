using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ToDoListWebContext _context;

        public TagRepository(ToDoListWebContext context)
        {
            _context = context;
        }

        public async Task<List<Domain.Entities.Tag>> GetAllTagAsync(int userId)
        {
            var tags = await _context.Tags
                                     .Where(tag => tag.UserId == userId)
                                     .ToListAsync();

            return tags.Select(t => new Domain.Entities.Tag
            {
                TagID = t.TagId,
                Name = t.Name,
                UserID = t.UserId
            }).ToList();
        }

        public Task<Tag> GetTagsByIdsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
