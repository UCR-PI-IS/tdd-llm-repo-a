using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Interfaces
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
