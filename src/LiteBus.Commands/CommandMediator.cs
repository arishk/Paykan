﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteBus.Commands.Abstractions;
using LiteBus.Messaging.Abstractions;
using LiteBus.Messaging.Abstractions.Extensions;
using LiteBus.Registry.Abstractions;

namespace LiteBus.Commands
{
    /// <inheritdoc cref="ICommandMediator" />
    public class CommandMediator : ICommandMediator
    {
        private readonly IMessageRegistry _messageRegistry;
        private readonly IServiceProvider _serviceProvider;

        public CommandMediator(IServiceProvider serviceProvider,
                               IMessageRegistry messageRegistry)
        {
            _serviceProvider = serviceProvider;
            _messageRegistry = messageRegistry;
        }
        
        private (TResult, IEnumerable<IPostHandleHook>) SendAsync<TResult>(IMessage command)
        {
            var commandType = command.GetType();

            var descriptor = _messageRegistry.GetDescriptor(commandType);

            if (descriptor.HandlerTypes.Count > 1) throw new MultipleCommandHandlerFoundException(commandType);

            var handler = _serviceProvider.GetService(descriptor.HandlerTypes.Single()) as IMessageHandler;

            var postHandleHooks = _serviceProvider.GetPostHandleHooks(descriptor.PostHandleHookTypes);

            var commandResult = handler.Handle(command);
            
            return ((TResult)commandResult, postHandleHooks);
        }

        public async Task SendAsync(ICommand command)
        {
            var (commandResultTask, postHandleHooks) = SendAsync<Task>(command);

            await commandResultTask;
            
            foreach (var postHandleHook in postHandleHooks)
            {
                await postHandleHook.ExecuteAsync(command);
            }
        }

        public async Task<TCommandResult> SendAsync<TCommandResult>(ICommand<TCommandResult> command)
        {
            var (commandResultTask, postHandleHooks) = SendAsync<Task<TCommandResult>>(command);

            var result = await commandResultTask;
            
            foreach (var postHandleHook in postHandleHooks)
            {
                await postHandleHook.ExecuteAsync(command);
            }

            return result;
        }
    }
}