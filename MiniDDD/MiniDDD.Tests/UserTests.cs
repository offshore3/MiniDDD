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
            _userService.Create("Jack", "Wang");
        }

        [Test]
        public void Test_ChangeFirstName()
        {
            Guid userId = new Guid("120D028C-09FE-4D9A-BBB0-52D50E06A47F");
            _userService.ChangeFirstName("DESHUI",userId);

            User user = _userService.Get(userId);
        }

        [Test]
        public void Test_Get_User()
        {
            Guid userId = new Guid("120D028C-09FE-4D9A-BBB0-52D50E06A47F");
            User user = _userService.Get(userId);

            Assert.IsNotNull(user);
        }
    }
}
