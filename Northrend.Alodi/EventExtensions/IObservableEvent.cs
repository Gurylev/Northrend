using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.EventExtensions
{
    /// <summary>
    /// Обобщённый интерфейс наблюдаемого события.
    /// </summary>
    public interface IObservableEvent
    {
        /// <summary>
        /// Отменяет все подписки на данный тип события.
        /// </summary>
        public void UnsubscribeAll();
    }
}
