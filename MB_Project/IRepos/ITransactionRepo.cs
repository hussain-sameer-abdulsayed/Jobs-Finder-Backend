namespace MB_Project.IRepos
{
    public interface ITransactionRepo
    {
        void CommitTransaction();
        void BeginTransaction();
        void RollBackTransaction();
    }
}
