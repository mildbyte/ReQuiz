using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReQuizServer
{
    interface IQuizQuestion
    {
        bool MarkAnswer(string answer);
    }
}
