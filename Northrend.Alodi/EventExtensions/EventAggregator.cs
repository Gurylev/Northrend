using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.EventExtensions
{
    /// <summary>
    /// Агрегатор событий. Позволяет регистрировать события и подписчиков для
    /// каждого вида событий посредством классов, производных от ObservableEvent<T>.
    /// </summary>
    public sealed class EventAggregator : IEventAggregator
    {
        private readonly object mSyncPoint = new();
        private readonly List<IObservableEvent> mEvents = new();

        public IEventAggregator Register(IObservableEvent observableEvent)
        {
            lock (mSyncPoint)
            {
                mEvents.Add(observableEvent);
                return this;
            }
        }

        public bool Unregister(IObservableEvent observableEvent)
        {
            lock (mSyncPoint)
            {
                return mEvents.Remove(observableEvent);
            }
        }

        public TEvent OnEvent<TEvent>() where TEvent : IObservableEvent
        {
            lock (mSyncPoint)
            {
                return mEvents.OfType<TEvent>().First();
            }
        }

        public void UnregisterAll()
        {
            lock (mSyncPoint)
            {
                mEvents.Clear();
            }
        }
    }
}
