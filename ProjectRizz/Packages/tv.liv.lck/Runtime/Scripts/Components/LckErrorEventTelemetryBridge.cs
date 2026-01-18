using System;
using System.Collections.Generic;
using static Liv.Lck.LckEvents;

namespace Liv.Lck
{
    internal class LckErrorEventTelemetryBridge : IDisposable
    {
        private readonly ILckEventBus _eventBus;

        private readonly Action<ILckResult> _telemetryAction;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public LckErrorEventTelemetryBridge(ILckEventBus eventBus, Action<ILckResult> telemetryAction)
        {
            _eventBus = eventBus;
            _telemetryAction = telemetryAction;
        }

        public void Monitor<TEvent, TResult>()
            where TEvent : IEventWithResult<TResult>
            where TResult : ILckResult
        {
            var subscription = new EventSubscription<TEvent>(
                _eventBus,
                OnEventReceived<TEvent, TResult>
            );
            _subscriptions.Add(subscription);
        }

        private void OnEventReceived<TEvent, TResult>(TEvent evt)
            where TEvent : IEventWithResult<TResult>
            where TResult : ILckResult
        {
            if (!evt.Result.Success)
            {
                _telemetryAction(evt.Result);
            }
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
        }
        
        private class EventSubscription<TEvent> : IDisposable
        {
            private readonly ILckEventBus _eventBus;
            private readonly Action<TEvent> _callback;

            public EventSubscription(ILckEventBus eventBus, Action<TEvent> callback)
            {
                _eventBus = eventBus;
                _callback = callback;
                _eventBus.AddListener<TEvent>(_callback);
            }

            public void Dispose()
            {
                _eventBus.RemoveListener<TEvent>(_callback);
            }
        }
    }
}
