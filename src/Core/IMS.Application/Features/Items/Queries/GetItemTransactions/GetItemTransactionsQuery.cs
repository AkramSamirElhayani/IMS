using System;
using System.Collections.Generic;
using IMS.Application.Features.Items.Common.Responses;
using MediatR;

namespace IMS.Application.Features.Items.Queries.GetItemTransactions
{
    public record GetItemTransactionsQuery(
        Guid ItemId, 
        DateTime? StartDate = null, 
        DateTime? EndDate = null) : IRequest<List<GetItemTransactionResponse>>;
}
