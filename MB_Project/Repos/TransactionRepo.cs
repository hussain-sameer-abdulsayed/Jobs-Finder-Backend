using MB_Project.Data;
using MB_Project.IRepos;

namespace MB_Project.Repos
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly MB_ProjectContext _context;

        public TransactionRepo(MB_ProjectContext context)
        {
            _context = context;
        }
        public void BeginTransaction()
        {
            _context.Database.BeginTransactionAsync();
        }

        public void CommitTransaction()
        {
            _context.Database.CommitTransactionAsync();
        }

        public void RollBackTransaction()
        {
            _context.Database.RollbackTransactionAsync();
        }
    }
}
