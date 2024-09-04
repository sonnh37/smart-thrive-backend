﻿using AutoMapper;
using BCrypt.Net;
using EXE201.SmartThrive.Domain.Contracts.Bases;
using EXE201.SmartThrive.Domain.Contracts.Repositories;
using EXE201.SmartThrive.Domain.Contracts.Services;
using EXE201.SmartThrive.Domain.Contracts.UnitOfWorks;
using EXE201.SmartThrive.Domain.Entities;
using EXE201.SmartThrive.Domain.Models.Requests.Commands.User;
using EXE201.SmartThrive.Domain.Models.Requests.Queries.Course;
using EXE201.SmartThrive.Domain.Models.Requests.Queries.User;
using EXE201.SmartThrive.Domain.Models.Responses;
using EXE201.SmartThrive.Domain.Models.Results;
using EXE201.SmartThrive.Domain.Utilities;
using EXE201.SmartThrive.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EXE201.SmartThrive.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUserRepository repository;
        private readonly IConfiguration configuration;
        public UserService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration _configuration) : base(mapper, unitOfWork)
        {
            repository = _unitOfWork.UserRepository;
            configuration = _configuration;
        }

        public async Task<string> CreateToken(UserResult user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection("JWT:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: creds
                    );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<PaginatedResponse<UserResult>> GetAllFiltered(UserGetAllQuery query)
        {
            var total = await repository.GetAllFiltered(query);
            var result = _mapper.Map<List<UserResult>>(total.Item1);
            var resultWithTotal = (result, total.Item2);

            return AppResponse.CreatePaginated(resultWithTotal, query);
        }

        public async Task<UserResult> Login(string usernameOrEmail, string password)
        {
            var user = await repository.FindByEmailOrUsername(usernameOrEmail);

            //check username 
            if (user == null)
            {
                return null;
            }

            //check password
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return _mapper.Map<UserResult>(user);
        }
    }
}
