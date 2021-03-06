﻿using System;
using LiteBus.Events.Abstractions;
using LiteBus.Registry.Abstractions;

namespace LiteBus.Events
{
    public class EventPublisherBuilder
    {
        public IEventPublisher Build(IServiceProvider serviceProvider,
                                     IMessageRegistry messageRegistry)
        {
            return new EventMediator(serviceProvider, messageRegistry);
        }
    }
}