namespace Savvy.Services.Ynab
{
    public class SynchronizedBudget
    {
        public SynchronizedBudget(Budget budget)
        {
            this.DropboxUserId = budget.DropboxUserId;
            this.BudgetName = budget.BudgetName;
            this.DropboxPath = budget.DropboxPath;
        }

        public long DropboxUserId { get; }
        public string BudgetName { get; }

        public string DropboxPath { get; }

        public string DropboxCursor { get; set; }

        protected bool Equals(SynchronizedBudget other)
        {
            return this.DropboxUserId == other.DropboxUserId && string.Equals(this.DropboxPath, other.DropboxPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SynchronizedBudget)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.DropboxUserId.GetHashCode() * 397) ^ (this.DropboxPath != null ? this.DropboxPath.GetHashCode() : 0);
            }
        }
    }
}