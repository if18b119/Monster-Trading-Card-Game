using NUnit.Framework;
using MCTGclass;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            User new_admin = new Admin("Tarek", "123", UserRole.admin);
            DBManagment.AddUser(new_admin.UniqueName, new_admin.Pwd, new_admin.Role);
            Assert.IsTrue(true);
        }
    }
}