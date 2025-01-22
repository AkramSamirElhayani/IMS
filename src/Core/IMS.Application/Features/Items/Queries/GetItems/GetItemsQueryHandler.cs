using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IMS.Application.Common.Interfaces;
using IMS.Application.Features.Items.Common.Responses;
using MediatR;

namespace IMS.Application.Features.Items.Queries.GetItems
{
    public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, List<GetItemResponse>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public GetItemsQueryHandler(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<List<GetItemResponse>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.SearchItemsAsync(
                request.SearchTerm,
                request.MinQuantity,
                request.MaxQuantity,
                request.SortBy,
                request.IsAscending,
                cancellationToken);

            return _mapper.Map<List<GetItemResponse>>(items);
        }
    }
}
