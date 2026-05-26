using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Contact.Commands.SubmitInquiry;

public class SubmitInquiryCommandHandler : IRequestHandler<SubmitInquiryCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public SubmitInquiryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(SubmitInquiryCommand request, CancellationToken cancellationToken)
    {
        var inquiry = new ContactInquiry
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Subject = request.Subject,
            Message = request.Message
        };

        _context.ContactInquiries.Add(inquiry);
        await _context.SaveChangesAsync(cancellationToken);

        return inquiry.Id;
    }
}
