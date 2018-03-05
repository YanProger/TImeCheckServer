using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum MessageCode
    {
        success = 1,
        error = 0
    }

    public class Message
    {
        /// <summary>
        /// Код сообщения (для приложений)
        /// </summary>
        public MessageCode Code { get; set; }
        /// <summary>
        /// Текст сообщения (для людей)
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Возвращаемые данные
        /// </summary>
        public object Data { get; set; }
    }

}
