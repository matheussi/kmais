using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNet
{
    public interface IEspecieDocumento
    {
        IBanco Banco { get; set; }
        int Codigo { get; set;}
        string Sigla { get; set; }
        string Especie { get; set;}
    }
}
