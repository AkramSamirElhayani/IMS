using System;
using IMS.Application.Features.Items.Common.Responses;
using MediatR;

namespace IMS.Application.Features.Items.Queries.GetItemById
{
    public record GetItemByIdQuery(Guid Id) : IRequest<GetItemResponse>;
}
