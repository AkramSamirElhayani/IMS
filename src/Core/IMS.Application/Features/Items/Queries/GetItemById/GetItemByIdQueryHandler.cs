using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IMS.Application.Common.Exceptions;
using IMS.Application.Common.Interfaces;
using IMS.Application.Features.Items.Common.Responses;
using IMS.Domain.Aggregates;
using MediatR;

namespace IMS.Application.Features.Items.Queries.GetItemById
{
    public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, GetItemResponse>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public GetItemByIdQueryHandler(IItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<GetItemResponse> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);

            if (item == null)
            {
                throw new NotFoundException(nameof(Item), request.Id);
            }

            return _mapper.Map<GetItemResponse>(item);
        }
    }
}
