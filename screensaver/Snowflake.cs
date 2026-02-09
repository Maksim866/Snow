using System;
namespace screensaver
{
        /// <summary>
        /// Класс, представляющий отдельную снежинку
        /// </summary>
        public class Snowflake
        {
            /// <summary>
            /// Х - координата снежинки
            /// </summary>
            public float X { get; set; }
            /// <summary>
            /// Y - координата снежинки
            /// </summary>
            public float Y { get; set; }
            /// <summary>
            /// Размер снежинки в пикселях
            /// </summary>
            public float Size { get; set; }
            /// <summary>
            /// Скорость падения снежинки
            /// </summary>
            public float Speed { get; set; }
        }
}
