﻿using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Mappers;

namespace IMAR_DialogoOperatore.Infrastructure.Mappers
{
    public class StatoAttivitaMapper : IStatoAttivitaMapper
    {
        public string FromJMesStatus(string statoJmes)
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
    }
}
