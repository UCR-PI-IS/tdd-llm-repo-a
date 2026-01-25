using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICPDLCMessageRepository
    {
        Task<CPDLCMessage> GetByIdAsync(Guid id);
        Task<IEnumerable<CPDLCMessage>> GetAllAsync();
        Task AddAsync(CPDLCMessage message);
        Task UpdateAsync(CPDLCMessage message);
        Task DeleteAsync(Guid id);
    }
}
