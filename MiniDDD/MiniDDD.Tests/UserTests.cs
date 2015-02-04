using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniDDD.Tests.Domain;
using NUnit.Framework;

namespace MiniDDD.Tests
{
    [TestFixture]
    public class UserTests
    {
        private Guid userId = new Guid("E4B1D05C-CD6B-40F2-B442-B20A1A8BB916");

        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            var container = BootStartup.Start();

             _userService = container.Resolve<UserService>();
        }

        [Test]
        public void Test_Create_User()
        {
            _userService.Create(userId,"Jack", "Wang");
        }

        [Test]
        public void Test_ChangeFirstName()
        {
            
            _userService.ChangeFirstName("xx",userId);

            User user = _userService.Get(userId);
        }

        [Test]
        public void Test_Get_User()
        {
            User user = _userService.Get(userId);

            Assert.IsNotNull(user);
        }
    }
}
