using System.Collections.Generic;
using IMS.Application.Features.Items.Common.Responses;
using MediatR;

namespace IMS.Application.Features.Items.Queries.GetItems
{
    public class GetItemsQuery : IRequest<List<GetItemResponse>>
    {
        public string? SearchTerm { get; init; }
        public int? MinQuantity { get; init; }
        public int? MaxQuantity { get; init; }
        public string? SortBy { get; init; }
        public bool IsAscending { get; init; } = true;
    }
}
