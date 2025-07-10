using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.UoW;
using IMAR_DialogoOperatore.Domain.Entities.JMES;

namespace EsportatoreTimbratureTeamSystem.Services
{
    public class TimbratureService
    {
        private readonly ISynergyJmesUoW _synergyJmesUoW;

        public TimbratureService(
            ISynergyJmesUoW synergyJmesUoW) 
        {
            _synergyJmesUoW = synergyJmesUoW;
        }

        public List<Timbratura> GetTimbrature()
        {
            List<Timbratura> timbrature = new List<Timbratura>();
            List<Timbratura> timbraturePause = new List<Timbratura>();
            List<Timbratura> timbratureIngressiUscite = new List<Timbratura>();

            List<AngRes> operatori = GetIdOperatori().ToList();
            List<TblResBrk> pause = GetPause(operatori).ToList();
            List<TblResClk> ingressiUscite = GetIngressiUscite(operatori).ToList();

            Task taskPause = Task.Run(() =>
            {
                timbraturePause = GetTimbraturePause(operatori, pause);
            });

            Task taskIngressiUscite = Task.Run(() =>
            {
                timbratureIngressiUscite = GetTimbratureIngressiUscite(operatori, ingressiUscite);
            });

            Task.WaitAll(taskPause, taskIngressiUscite);

            timbrature.AddRange(timbraturePause);
            timbrature.AddRange(timbratureIngressiUscite);
            return timbrature;
        }

        private IEnumerable<AngRes> GetIdOperatori()
        {
            List<string> badgeEsclusi = new List<string> {
                "0010", "0046", "0056", "0072", "0078", "0084", "0086", "0091",
                "0109", "0154", "0157", "0168", "0173", "0222", "0228", "0213"
            };

            var res = _synergyJmesUoW.AngRes.Get().ToList();

            return res.Where(x => !badgeEsclusi.Contains(x.ResCod) &&
                                  !x.ResCod.Contains('$') && 
                                  !x.ResCod.Contains('@'));
        }

        private IEnumerable<TblResBrk> GetPause(IEnumerable<AngRes> operatori)
        {
            DateTime oggi = DateTime.Today;
            DateTime ieri = oggi.AddDays(-1);
            DateTime oggiAlle6 = oggi.AddHours(6);

            IEnumerable<decimal> idOperatori = operatori.Select(x => x.Uid).ToList();
            var res = _synergyJmesUoW.TblResBrk.Get().ToList();

            return res.Where(x => GetEffectiveInizioPausa(x) >= ieri &&
                                  GetEffectiveFinePausa(x) <= oggiAlle6 &&
                                  idOperatori.Contains(x.ResUid));
        }

        private IEnumerable<TblResClk> GetIngressiUscite(IEnumerable<AngRes> operatori)
        {
            DateTime oggi = DateTime.Today;
            DateTime ieri = oggi.AddDays(-1);
            DateTime oggiAlle6 = oggi.AddHours(6);

            IEnumerable<decimal> idOperatori = operatori.Select(x => x.Uid);

            var res = _synergyJmesUoW.TblResClk.Get()
                                            .ToList();

            return res.Where(x => GetEffectiveIngress(x) >= ieri &&
                                  GetEffectiveUscita(x) <= oggiAlle6 &&
                                  idOperatori.Contains(x.ResUid));
        }

        private List<Timbratura> GetTimbraturePause(IEnumerable<AngRes> operatori, IEnumerable<TblResBrk> pause)
        {
            List<Timbratura> timbraturePause = new List<Timbratura>();

            foreach (var pausa in pause)
            {
                timbraturePause.Add(new Timbratura
                {
                    BadgeOperatore = operatori.Single(x => x.Uid == pausa.ResUid).ResCod,
                    Causale = Costanti.INIZIO_PAUSA,
                    Timestamp = pausa.TssStr
                });

                if (pausa.TssEnd is DateTime timestampFinePausa)
                    timbraturePause.Add(new Timbratura
                    {
                        BadgeOperatore = operatori.Single(x => x.Uid == pausa.ResUid).ResCod,
                        Causale = Costanti.FINE_PAUSA,
                        Timestamp = timestampFinePausa
                    });
            }

            return timbraturePause;
        }

        private List<Timbratura> GetTimbratureIngressiUscite(
            IEnumerable<AngRes> operatori,
            IEnumerable<TblResClk> ingressiUscite)
        {
            return ingressiUscite
                .Where(r => GetEffectiveIngress(r).HasValue || GetEffectiveUscita(r).HasValue)
                .GroupBy(r => r.ResUid)
                .SelectMany(group => ProcessOperatoreTimbrature(group, operatori))
                .ToList();
        }

        private IEnumerable<Timbratura> ProcessOperatoreTimbrature(
            IGrouping<decimal, TblResClk> group,
            IEnumerable<AngRes> operatori)
        {
            var result = new List<Timbratura>();
            var badge = operatori.Single(x => x.Uid == group.Key).ResCod;
            var ordered = group.OrderBy(r => GetEffectiveIngress(r) ?? GetEffectiveUscita(r)).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                var current = ordered[i];
                var ingresso = GetEffectiveIngress(current);
                var uscita = GetEffectiveUscita(current);

                if (uscita.HasValue &&
                    IsLateNightExit(uscita.Value) &&
                    HasQuickNextDayRestart(ordered, i, uscita.Value, current.ResUid))
                {
                    result.AddRange(HandleQuickRestartCase(current, ordered[i + 1], badge));
                    i++;
                }
                else
                {
                    result.AddRange(ProcessTimbratureRecord(current, badge));
                }
            }

            return result;
        }

        private IEnumerable<Timbratura> ProcessTimbratureRecord(TblResClk record, string badge)
        {
            var result = new List<Timbratura>();

            var ingresso = GetEffectiveIngress(record);
            var uscita = GetEffectiveUscita(record);

            if (ingresso.HasValue)
                result.Add(AddTimbratura(badge, Costanti.INGRESSO, ingresso.Value));

            if (uscita.HasValue)
                result.Add(AddTimbratura(badge, Costanti.USCITA, uscita.Value));

            return result;
        }

        private IEnumerable<Timbratura> HandleQuickRestartCase(TblResClk current, TblResClk next, string badge)
        {
            var result = new List<Timbratura>();

            var ingresso = GetEffectiveIngress(current);
            var uscita = GetEffectiveUscita(next);

            if (ingresso.HasValue)
                result.Add(AddTimbratura(badge, Costanti.INGRESSO, ingresso.Value));

            if (uscita.HasValue)
                result.Add(AddTimbratura(badge, Costanti.USCITA, uscita.Value));

            return result;
        }

        private Timbratura AddTimbratura(string badge, string causale, DateTime timestamp)
        {
            return new Timbratura
            {
                BadgeOperatore = badge,
                Causale = causale,
                Timestamp = timestamp
            };
        }

        private bool IsLateNightExit(DateTime timestamp) =>
            timestamp.TimeOfDay >= new TimeSpan(23, 50, 0) &&
            timestamp.TimeOfDay <= new TimeSpan(23, 59, 59);

        private bool HasQuickNextDayRestart(List<TblResClk> records, int index, DateTime exitTime, decimal resUid)
        {
            if (index + 1 >= records.Count)
                return false;

            var next = records[index + 1];
            var nextIngresso = GetEffectiveIngress(next);

            return next.ResUid == resUid
                && nextIngresso.HasValue
                && nextIngresso.Value > exitTime
                && nextIngresso.Value.Date > exitTime.Date
                && nextIngresso.Value <= exitTime.AddMinutes(10);
        }

        private DateTime? GetEffectiveIngress(TblResClk record) => record.EdtInnTss ?? record.ClkInnTss;
        private DateTime? GetEffectiveUscita(TblResClk record) => record.EdtOutTss ?? record.ClkOutTss;

        private DateTime? GetEffectiveInizioPausa(TblResBrk record) => record.EdtTssStr ?? record.TssStr;
        private DateTime? GetEffectiveFinePausa(TblResBrk record) => record.EdtTssEnd ?? record.TssEnd;
    }
}
