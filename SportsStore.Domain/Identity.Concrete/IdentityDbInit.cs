using System.Data.Entity;

namespace SportsStore.Domain.Concrete
{
    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<EfIdentityDbContext>
    {
        protected override void Seed(EfIdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        private void PerformInitialSetup(EfIdentityDbContext context)
        {

        }
    }
}
