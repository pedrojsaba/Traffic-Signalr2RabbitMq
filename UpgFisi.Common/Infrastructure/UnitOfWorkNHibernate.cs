using NHibernate;
using System.Data;

namespace UpgFisi.Common.Infrastructure
{
    public class UnitOfWorkNHibernate
    {
        public ISession Session { get; private set; }
        public ITransaction Transaction { get; private set; }

        public UnitOfWorkNHibernate(ISession session)
        {
            Session = session;
        }

        public bool BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (Transaction == null || !Transaction.IsActive)
            {
                Transaction = Session.BeginTransaction(isolationLevel);
                return true;
            }
            return false;
        }

        public void Commit(bool commit)
        {
            if (Transaction != null && Transaction.IsActive && commit)
            {
                try
                {
                    Transaction.Commit();
                }
                finally
                {
                    Transaction.Dispose();
                    //Session.Close();
                    //Session.Dispose();
                }
            }
        }

        public void Rollback(bool rollback)
        {
            if (Transaction != null && Transaction.IsActive && rollback)
            {
                try
                {
                    Transaction.Rollback();
                }
                finally
                {
                    Transaction.Dispose();
                    //Session.Close();
                    //Session.Dispose();
                }
            }
        }
    }
}
