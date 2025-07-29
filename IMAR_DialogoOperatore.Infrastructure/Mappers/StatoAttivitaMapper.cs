using IMAR_DialogoOperatore.Application;
using System.Globalization;

namespace IMAR_DialogoOperatore.Infrastructure.Mappers
{
    public static class StatoAttivitaMapper
    {
        public static string FromJMesStatus(string statoJmes)
        {
            switch (statoJmes)
            {
                case Costanti.JMES_IN_ATTREZZAGGIO:
                    return Costanti.IN_ATTREZZAGGIO;

                case Costanti.JMES_IN_LAVORO:
                    return Costanti.IN_LAVORO;

                case Costanti.JMES_LAVORO_SOSPESO:
                    return Costanti.LAVORO_SOSPESO;

                case Costanti.JMES_ATTREZZAGGIO_SOSPESO:
                    return Costanti.ATTREZZAGGIO_SOSPESO;

                default:
                    return "";
            }
        }

        public static string FromJMesCode(decimal codiceJMes)
        {

            switch (codiceJMes)
            {
                case 1:
                    return Costanti.IN_ATTREZZAGGIO;

                case 2:
                    return Costanti.IN_LAVORO;

                case 3:
                    return Costanti.LAVORO_SOSPESO;

                case 4:
                    return Costanti.AVANZAMENTO;

                default:
                    return "";
            }
        }

        public static string FromJMesCodeExtended(decimal codiceJMes)
        {

            switch (codiceJMes)
            {
                case 1:
                    return Costanti.JMES_IN_ATTREZZAGGIO;

                case 2:
                    return Costanti.JMES_IN_LAVORO;

                case 3:
                    return Costanti.JMES_LAVORO_SOSPESO;

                case 4:
                    return Costanti.JMES_AVANZAMENTO;

                default:
                    return "";
            }
        }
    }
}
