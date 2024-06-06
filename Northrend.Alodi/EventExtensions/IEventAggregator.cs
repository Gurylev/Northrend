using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.EventExtensions
{
    /// <summary>
    /// Интерфейс агрегатора событий. 
    /// Содержит методы для регистрации и доступа к событиям.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Позволяет получить доступ к указанному событию.
        /// </summary>
        /// <typeparam name="TEvent">Класс ObservableEvent<T> или производный от него</typeparam>
        /// <returns>Запрошенный тип события</returns>
        TEvent OnEvent<TEvent>() where TEvent : IObservableEvent;

        /// <summary>
        /// Регистрирует указанный тип события. 
        /// </summary>
        /// <param name="observableEvent">Класс ObservableEvent<T> или производный от него</param>
        /// <returns>Ссылку на себя</returns>
        IEventAggregator Register(IObservableEvent observableEvent);

        /// <summary>
        /// Снимает с регистрации указанный тип события.
        /// </summary>
        /// <param name="observableEvent"><Класс ObservableEvent<T> или производный от него/param>
        /// <returns>true, в случае успешного выполнения операции, иначе - false</returns>
        bool Unregister(IObservableEvent observableEvent);

        /// <summary>
        /// Снимает с регистрации все виды событий.
        /// </summary>
        public void UnregisterAll();
    }
}
