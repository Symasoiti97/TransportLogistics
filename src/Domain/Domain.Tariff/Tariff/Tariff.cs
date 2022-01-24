using System;

namespace TL.Tariff.Tariff
{
    public class Tariff
    {
        /// <summary>
        /// Ид тарифа
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Собственность контейнера
        /// </summary>
        public ContainerOwn Own { get; set; }
    }
}