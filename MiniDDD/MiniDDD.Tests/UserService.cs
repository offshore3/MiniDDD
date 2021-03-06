﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniDDD.Storage;
using MiniDDD.Storage.UnitOfWork;
using MiniDDD.Tests.Domain;

namespace MiniDDD.Tests
{
    public class UserService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IRepository<User> _userRepository;

        public UserService(IUnitOfWorkFactory unitOfWorkFactory, IRepository<User> userRepository )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _userRepository = userRepository;
        }


        public User Get(Guid userId)
        {
            return _userRepository.GetById(userId);
        }


        public void Create(Guid id,string firstName, string lastName)
        {
            //using (var unitOfWork=_unitOfWorkFactory.GetCurrentUnitOfWork())
            //{
                User user=new User();
                user.Create(id,firstName,lastName);
                _userRepository.Save(user,-1);

            //    unitOfWork.Commit();
            //}
        }

        public void ChangeFirstName(string firstName,Guid userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetCurrentUnitOfWork())
            {
                User user = _userRepository.GetById(userId);
                user.ChangeFirstName(firstName);

                _userRepository.Save(user, user.Version);

                unitOfWork.Commit();
            }
        }
    }




}
