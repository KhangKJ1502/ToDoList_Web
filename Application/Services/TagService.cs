using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<List<TagDTO>> GetAllsTags(int UserId)
        {
            var tags = await _tagRepository.GetAllTagAsync(UserId);

            return tags.Select(t => new TagDTO
            {
                TagID = t.TagID,
                Name = t.Name,
                UserID = t.UserID
            }).ToList();
        }

    }
}
