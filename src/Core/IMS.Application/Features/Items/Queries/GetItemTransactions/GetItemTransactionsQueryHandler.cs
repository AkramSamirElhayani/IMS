using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IMS.Application.Common.Exceptions;
using IMS.Application.Common.Interfaces;
using IMS.Application.Features.Items.Common.Responses;
using IMS.Domain.Aggregates;
using MediatR;

namespace IMS.Application.Features.Items.Queries.GetItemTransactions
{
    public class GetItemTransactionsQueryHandler 
        : IRequestHandler<GetItemTransactionsQuery, List<GetItemTransactionResponse>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public GetItemTransactionsQueryHandler(
            IItemRepository itemRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<List<GetItemTransactionResponse>> Handle(
            GetItemTransactionsQuery request, 
            CancellationToken cancellationToken)
        {
            // First check if the item exists
            var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException(nameof(Item), request.ItemId);
            }

            // Get transactions for the item within the date range
            var transactions = await _transactionRepository.GetByDateRangeAsync(
                request.StartDate ?? DateTimeOffset.MinValue,
                request.EndDate ?? DateTimeOffset.MaxValue,
                cancellationToken);

            // Filter transactions for this specific item and map to response
            var itemTransactions = transactions
                .Where(t => t.ItemId == request.ItemId)
                    .Select(t => new GetItemTransactionResponse
                    {
                        TransactionId = t.Id,
                        Date = t.TransactionDate.DateTime,
                        Quantity = t.Quantity,
                        TransactionType = t.Type.ToString()
                    })
                .OrderByDescending(t => t.Date)
                .ToList();

            return itemTransactions;
        }
    }
}
