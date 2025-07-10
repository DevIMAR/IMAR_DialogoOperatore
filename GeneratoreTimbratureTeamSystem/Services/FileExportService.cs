using Microsoft.Extensions.Configuration;

namespace EsportatoreTimbratureTeamSystem.Services
{
    public class FileExportService
    {
        private readonly string _pathEsportazioneFile;

        public FileExportService(
            IConfiguration config)
        {
            _pathEsportazioneFile = config["FilePaths:TeamSystemTest"];
        }

        public void Export(string timbratureCodificate)
        {
            if (!Directory.Exists(_pathEsportazioneFile))
                Directory.CreateDirectory(_pathEsportazioneFile);

            string fileName = $"TeamSystem_{DateTime.Now:yyyyMMdd}.txt";
            string fullPath = Path.Combine(_pathEsportazioneFile, fileName);
            if (File.Exists(fullPath))
                return;

            File.WriteAllText(fullPath, timbratureCodificate);
        }
    }
}
