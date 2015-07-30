namespace Savvy.Services.Ynab
{
    public class Budget
    {
        public Budget(long dropboxUserId, string budgetName, string dropboxPath)
        {
            this.DropboxUserId = dropboxUserId;
            this.BudgetName = budgetName;
            this.DropboxPath = dropboxPath;
        }

        public long DropboxUserId { get; }
        public string BudgetName { get; }
        public string DropboxPath { get; }

        protected bool Equals(Budget other)
        {
            return this.DropboxUserId == other.DropboxUserId && string.Equals(this.DropboxPath, other.DropboxPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Budget)obj);
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