﻿using System.Threading.Tasks;
using LiteBus.Messaging.Abstractions;

namespace LiteBus.Queries.Abstractions
{
    /// <summary>
    ///     Represents a query
    /// </summary>
    /// <typeparam name="TQueryResult">The result type of query</typeparam>
    public interface IQuery<TQueryResult> : IMessage<Task<TQueryResult>>
    {
    }
}