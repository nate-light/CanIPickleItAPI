namespace CanIPickleItAPI.Services
{
    public interface IPickleCheckerService
    {
        Task<PickleCheckResult> CanPickleAsync(string item);
    }

    public class PickleCheckResult
    {
        public bool CanPickle { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}