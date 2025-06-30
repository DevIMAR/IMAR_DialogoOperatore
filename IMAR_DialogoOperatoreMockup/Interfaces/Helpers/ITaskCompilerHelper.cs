using IMAR_DialogoOperatore.Domain.Models;

namespace IMAR_DialogoOperatore.Interfaces.Helpers
{
    public interface ITaskCompilerHelper
    {
        TaskAsana TaskAsana { get; }

        void CompilaTaskAsana(); 
    }
}
