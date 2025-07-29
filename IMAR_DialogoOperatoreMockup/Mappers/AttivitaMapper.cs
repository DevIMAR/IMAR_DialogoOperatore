﻿using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Mappers;
using IMAR_DialogoOperatore.Interfaces.ViewModels;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Mappers
{
    public class AttivitaMapper : IAttivitaMapper
	{
        public Attivita AttivitaViewModelToAttivita(IAttivitaViewModel attivitaViewModel)
		{
			return new Attivita
			{
                Causale = attivitaViewModel.Causale,
                CausaleEstesa = attivitaViewModel.CausaleEstesa,
                Bolla = attivitaViewModel.Bolla,
                Odp = attivitaViewModel.Odp,
                Articolo = attivitaViewModel.Articolo,
                DescrizioneArticolo = attivitaViewModel.DescrizioneArticolo,
                Fase = attivitaViewModel.Fase,
                DescrizioneFase = attivitaViewModel.DescrizioneFase,
                QuantitaOrdine = attivitaViewModel.QuantitaOrdine,
                QuantitaProdotta = attivitaViewModel.QuantitaProdotta,
                QuantitaProdottaNonContabilizzata = attivitaViewModel.QuantitaProdottaNonContabilizzata,
                QuantitaProdottaContabilizzata = attivitaViewModel.QuantitaProdottaContabilizzata,
                QuantitaScartata = attivitaViewModel.QuantitaScartata,
                QuantitaScartataNonContabilizzata = attivitaViewModel.QuantitaScartataNonContabilizzata,
                QuantitaScartataContabilizzata = attivitaViewModel.QuantitaScartataContabilizzata,
                QuantitaResidua = attivitaViewModel.QuantitaResidua,
                SaldoAcconto = attivitaViewModel.SaldoAcconto,
                CodiceJMes = attivitaViewModel.CodiceJMes,
                Macchina = attivitaViewModel.Macchina
            };
		}

		public IEnumerable<IAttivitaViewModel> ListaAttivitaToListaAttivitaViewModel(IEnumerable<Attivita> attivitaList)
        {
            IList<IAttivitaViewModel> attivitaViewModelList = new List<IAttivitaViewModel>();

			foreach (Attivita attivita in attivitaList)
				attivitaViewModelList.Add(new AttivitaViewModel(attivita));

			return attivitaViewModelList;
		}

        public TimbraturaAttivitaViewModel AttivitaToTimbraturaAttivitaViewModel(Attivita attivita)
        {
            TimbraturaAttivitaViewModel temp = new TimbraturaAttivitaViewModel
            {
                Causale = attivita.Causale,
                CausaleEstesa = attivita.CausaleEstesa,
                Fase = attivita.Fase,
                Bolla = attivita.Bolla,
                Odp = attivita.Odp,
                Timestamp = attivita.InizioAttivita
            };

            if (temp.CausaleEstesa != Costanti.JMES_IN_LAVORO && !temp.CausaleEstesa.Contains("Attrezzaggio"))
            {
                temp.QuantitaProdotta = attivita.QuantitaProdotta;
                temp.QuantitaScartata = attivita.QuantitaScartata;
            }

            return temp;
        }

        public IList<TimbraturaAttivitaViewModel> ListAttivitaToListTimbraturaAttivitaViewModel(IList<Attivita>? attivitaList)
        {
            IList<TimbraturaAttivitaViewModel> timbratureAttivita = new List<TimbraturaAttivitaViewModel>();
            TimbraturaAttivitaViewModel timbraturaAttivitaTemp;

            foreach (Attivita attivita in attivitaList)
            {
                timbratureAttivita.Add(AttivitaToTimbraturaAttivitaViewModel(attivita));

                if (attivita.FineAttivita != null && attivita.CausaleEstesa != Costanti.AVANZAMENTO)
                    timbraturaAttivitaTemp = AggiungiTimbraturaFineAttivita(timbratureAttivita, attivita);
            }

            return timbratureAttivita;
        }

        private TimbraturaAttivitaViewModel AggiungiTimbraturaFineAttivita(IList<TimbraturaAttivitaViewModel> timbratureAttivita, Attivita attivita)
        {
            TimbraturaAttivitaViewModel timbraturaAttivitaTemp = AttivitaToTimbraturaAttivitaViewModel(attivita);
            timbraturaAttivitaTemp.CausaleEstesa = "Fine " + attivita.CausaleEstesa.Split().Last();
            timbraturaAttivitaTemp.Timestamp = (DateTime)attivita.FineAttivita;
            timbraturaAttivitaTemp.QuantitaProdotta = attivita.QuantitaProdotta;
            timbraturaAttivitaTemp.QuantitaScartata = attivita.QuantitaScartata;

            timbratureAttivita.Add(timbraturaAttivitaTemp);
            return timbraturaAttivitaTemp;
        }
    }
}
