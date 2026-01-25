using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class CPDLCMessageService
    {
        private readonly ICPDLCMessageRepository _repository;

        public CPDLCMessageService(ICPDLCMessageRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<CPDLCMessage> SendMessageAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content cannot be empty.", nameof(content));

            var message = new CPDLCMessage
            {
                Content = content,
                Status = MessageStatus.Sent,
                Timestamp = DateTime.UtcNow
            };

            await _repository.AddAsync(message);
            return message;
        }

        public async Task<CPDLCMessage> GetMessageAsync(Guid messageId)
        {
            return await _repository.GetByIdAsync(messageId);
        }

        public async Task<IEnumerable<CPDLCMessage>> GetAllMessagesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task AcknowledgeMessageAsync(Guid messageId)
        {
            var message = await _repository.GetByIdAsync(messageId);
            if (message == null)
                throw new InvalidOperationException($"Message with ID {messageId} not found.");

            message.Status = MessageStatus.Acknowledged;
            await _repository.UpdateAsync(message);
        }
    }
}
