using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.EventExtensions
{
    /// <summary>
    /// Класс подписчика на собитие, используемый для отмены подписки.
    /// </summary>
    /// <typeparam name="T">Тип события, на которое ранее была осуществлена подписка</typeparam>
    public sealed class EventSubscriber<T> : IDisposable
    {
        private readonly List<Action<T>> mSubscribers;
        private readonly Action<T> mSubscription;
        public EventSubscriber(List<Action<T>> subscribers, Action<T> subscription)
        {
            mSubscribers = subscribers;
            mSubscription = subscription;
        }

        /// <summary>
        /// Удаляет подписку на собитие.
        /// </summary>
        public void Dispose()
        {
            lock (mSubscribers)
            {
                _ = mSubscribers.Remove(mSubscription);
            }
        }
    }
}
