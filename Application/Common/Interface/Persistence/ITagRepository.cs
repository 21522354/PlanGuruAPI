using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interface.Persistence
{
    public interface ITagRepository
    {
        public Task<List<string>> GetFiltersAsync();
        public Task<List<string>> GetTagsAsync();
    }
}
