using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
	/// <summary>
	/// Calcola QuantitaOrdine per ogni fase di un ODP, replicando la logica
	/// precedentemente nei self-join SQL (CTE FASE_PREC, FASE_IMM_PREC, ecc.)
	/// </summary>
	public static class QuantitaOrdineCalculator
	{
		public static void Calcola(IList<Attivita> attivita)
		{
			var gruppiPerOdp = attivita.GroupBy(a => a.Odp);

			foreach (var gruppo in gruppiPerOdp)
			{
				var fasi = gruppo.OrderBy(a => a.Fase).ToList();

				for (int i = 0; i < fasi.Count; i++)
				{
					var corrente = fasi[i];
					bool isNonPianificata = corrente.IsNonPianificata == "*";

					if (isNonPianificata)
					{
						CalcolaPerFaseNonPianificata(fasi, i, corrente);
					}
					else
					{
						CalcolaPerFasePianificata(fasi, i, corrente);
					}
				}
			}
		}

		/// <summary>
		/// Fase non pianificata (FLSFCI='*'):
		/// Trova la fase pianificata precedente più vicina.
		/// Se trovata: QORDCI - somma(QPROCI + QTA_NC) delle fasi dalla pianificata (inclusa) alla corrente (esclusa).
		/// Se non trovata: QORDCI.
		/// </summary>
		private static void CalcolaPerFaseNonPianificata(List<Attivita> fasi, int indiceCorrente, Attivita corrente)
		{
			// Cerca la fase pianificata precedente più vicina (cercando indietro)
			Attivita? fasePianPrec = null;
			for (int j = indiceCorrente - 1; j >= 0; j--)
			{
				if (fasi[j].IsNonPianificata != "*")
				{
					fasePianPrec = fasi[j];
					break;
				}
			}

			if (fasePianPrec == null)
			{
				corrente.QuantitaOrdine = corrente.QuantitaOrdineOriginale;
				return;
			}

			// Somma produzione dalla fase pianificata precedente (inclusa) alla corrente (esclusa)
			int prodCumulata = 0;
			for (int j = 0; j < indiceCorrente; j++)
			{
				if (string.Compare(fasi[j].Fase, fasePianPrec.Fase, StringComparison.Ordinal) >= 0)
				{
					prodCumulata += fasi[j].QuantitaProdottaContabilizzata + fasi[j].QuantitaProdottaNonContabilizzata;
				}
			}

			corrente.QuantitaOrdine = corrente.QuantitaOrdineOriginale - prodCumulata;
			corrente.BollaFasePrecedente = fasePianPrec.Bolla;
		}

		/// <summary>
		/// Fase pianificata (FLSFCI=''):
		/// 1) Se c'è una fase pianificata precedente, ci sono fasi NP intermedie,
		///    e la fase immediatamente precedente è a saldo → somma produzione intermedie
		/// 2) Se non c'è nessuna fase precedente → QORDCI
		/// 3) Se la fase immediatamente precedente è a saldo → produzione della precedente
		/// 4) Default → QORDCI
		/// </summary>
		private static void CalcolaPerFasePianificata(List<Attivita> fasi, int indiceCorrente, Attivita corrente)
		{
			// Fase immediatamente precedente (qualsiasi tipo)
			Attivita? faseImmPrec = indiceCorrente > 0 ? fasi[indiceCorrente - 1] : null;

			// Cerca la fase pianificata precedente più vicina
			Attivita? fasePianPrec = null;
			for (int j = indiceCorrente - 1; j >= 0; j--)
			{
				if (fasi[j].IsNonPianificata != "*")
				{
					fasePianPrec = fasi[j];
					break;
				}
			}

			// Caso 2 del CASE SQL: pianificata con fasi NP intermedie + imm.prec. a saldo
			if (fasePianPrec != null)
			{
				bool haFasiNP = false;
				int sommaProduzione = 0;

				for (int j = 0; j < indiceCorrente; j++)
				{
					if (string.Compare(fasi[j].Fase, fasePianPrec.Fase, StringComparison.Ordinal) >= 0)
					{
						sommaProduzione += fasi[j].QuantitaProdottaContabilizzata + fasi[j].QuantitaProdottaNonContabilizzata;
						if (fasi[j].IsNonPianificata == "*")
							haFasiNP = true;
					}
				}

				if (haFasiNP && faseImmPrec != null && IsFaseASaldo(faseImmPrec))
				{
					corrente.QuantitaOrdine = sommaProduzione;
					corrente.BollaFasePrecedente = faseImmPrec.Bolla;
					return;
				}
			}

			// Nessuna fase precedente → QORDCI
			if (faseImmPrec == null)
			{
				corrente.QuantitaOrdine = corrente.QuantitaOrdineOriginale;
				return;
			}

			// Fase precedente a saldo → produzione della precedente
			if (IsFaseASaldo(faseImmPrec))
			{
				corrente.QuantitaOrdine = faseImmPrec.QuantitaProdottaContabilizzata + faseImmPrec.QuantitaProdottaNonContabilizzata;
				corrente.BollaFasePrecedente = faseImmPrec.Bolla;
				return;
			}

			// Default → QORDCI
			corrente.QuantitaOrdine = corrente.QuantitaOrdineOriginale;
		}

		private static bool IsFaseASaldo(Attivita fase)
		{
			return fase.TipoRicevimento == "S" || fase.SaldoAccontoJmes == "S";
		}
	}
}
