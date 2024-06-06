using Northrend.Alodi.EventExtensions.ObservableEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.EventExtensions
{
    public record MetaObservableEvent(Type ObservableEventType, string Name);
    
    /// <summary>
    /// Фабрика наблюдаемых событий <see cref="ObservableEvent{T}"/>
    /// </summary>
    public static class ObservableEventsFactory
    {
        private static readonly MetaObservableEvent[] mObservableEvents;

        static ObservableEventsFactory()
        {
            mObservableEvents = new[]
            {
                new MetaObservableEvent(typeof(UpdateIntegralVelocitiesEvent),       nameof(UpdateIntegralVelocitiesEvent)),
                new MetaObservableEvent(typeof(TransferRequestEvent),          nameof(TransferRequestEvent)),
                new MetaObservableEvent(typeof(AvailableIceBraker),              nameof(AvailableIceBraker)),
            };
        }

        /// <summary>
        /// Возвращает (Read only) коллекцию доступных наблюдаемых событий.
        /// </summary>
        public static IEnumerable<IObservableEvent> AvailableEvents 
            => mObservableEvents.Select(m => (IObservableEvent)Activator.CreateInstance(m.ObservableEventType)!);

        /// <summary>
        /// Создаёт экземпляр наблюдаемого события <see cref="ObservableEvent{T}"/>.
        /// </summary>
        /// <param name="name">Название наблюдаемого события (соответствует имени типа)</param>
        /// <returns></returns>
        public static IObservableEvent Create(string name)
        {
            return mObservableEvents.FirstOrDefault(me => 0 == string.Compare(me.Name, name, StringComparison.OrdinalIgnoreCase)) is MetaObservableEvent metaEvent
                ? (IObservableEvent)Activator.CreateInstance(metaEvent.ObservableEventType)!
                : throw new ArgumentException($"Неизвестный ObservableEvent: {name}", nameof(name));
        }

        /// <summary>
        /// Создаёт экземпляр наблюдаемого события <see cref="ObservableEvent{T}"/>.
        /// </summary>
        /// <typeparam name="TObservableEvent"></typeparam>
        /// <returns></returns>
        public static TObservableEvent Create<TObservableEvent>() where TObservableEvent : notnull, IObservableEvent
        {
            var t = typeof(TObservableEvent);
            return mObservableEvents.FirstOrDefault(me => me.ObservableEventType == t) is MetaObservableEvent metaEvent
                ? (TObservableEvent)Activator.CreateInstance(metaEvent.ObservableEventType)!
                : throw new ArgumentException($"Неизвестный ObservableEvent: {t.Name}");
        }
    }
}
