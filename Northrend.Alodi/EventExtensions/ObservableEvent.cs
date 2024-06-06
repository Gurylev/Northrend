using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.EventExtensions
{
    /// <summary>
    /// Наблюдаемое событие c параметром обработчиков типа T.
    /// </summary>
    /// <typeparam name="T">Тип параметра обработчиков события</typeparam>
    public class ObservableEvent<T> : IObservableEvent //where T : EventArgs
    {
        protected readonly List<Action<T>> mSubscribers = new();

        /// <summary>
        /// Подписка на событие.
        /// </summary>
        /// <param name="subscriber">Обработчик события подписчика</param>
        /// <returns>Экземпляр Subscriber<T>, с помощью которого можно отменить подписку</returns>
        public virtual IDisposable Subscribe(Action<T> subscriber)
        {
            lock (mSubscribers)
            {
                mSubscribers.Add(subscriber);
                return new EventSubscriber<T>(mSubscribers, subscriber);
            }
        }

        public virtual void UnsubscribeAll()
        {
            lock (mSubscribers)
            {
                mSubscribers.Clear();
            }
        }

        /// <summary>
        /// Порождает событие с указанными данными.
        /// </summary>
        /// <param name="data">Данные для обработчиков события</param>
        public virtual void Publish(T data)
        {
            lock (mSubscribers)
            {
                mSubscribers.ForEach(subscriber => subscriber.Invoke(data));
            }
        }
    }
}
