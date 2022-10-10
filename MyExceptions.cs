using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExceptions
{
    public class EmptyLoginExeption : Exception
    {
        public override string Message => "Ошибка. Не указан логин";
    }
    public class EmptyPasswordExeption : Exception
    {
        public override string Message => "Ошибка. Не указан пароль";
    }
    public class EmptyfieldExeption : Exception
    {
        public override string Message => "Ошибка. Остались пустые поля";
    }
    public class NoConnectionExeption : Exception
    {
        public override string Message => "Ошибка. Нет соединения с нужной базой данных";
    }
}
