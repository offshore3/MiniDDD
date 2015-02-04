using System;
using MiniDDD.Domain;
using MiniDDD.Events;

namespace MiniDDD.Tests.Domain
{
    public class User : AggregateRoot,
        IHandle<UserCreatedEvent>,
        IHandle<ChangeFirstNameEvent>
        
    {
        private string FirstName { get; set; }
        private string LastName { get; set; }
       

        public void Handle(UserCreatedEvent e)
        {
            this.FirstName = e.FirstName;
            this.LastName = e.LastName;
            this.Id = e.AggregateRootId;
        }

        public void ChangeFirstName(string firstName)
        {
            ApplyChange(new ChangeFirstNameEvent()
            {
                FirstName = firstName
            });
        }

        public  void Create(Guid id,string firstName, string lastName)
        {
           this.Id = id;
           ApplyChange(new UserCreatedEvent
           {
               FirstName = firstName,
               LastName = lastName
           });
        }

        public void Handle(ChangeFirstNameEvent e)
        {
            this.FirstName = e.FirstName;
        }

    }

    public class ChangeFirstNameEvent : AggregateRootEvent
    {
        public string FirstName { get; set; }
    }

    public class UserCreatedEvent:AggregateRootEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
