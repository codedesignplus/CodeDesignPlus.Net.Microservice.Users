global using CodeDesignPlus.Microservice.Api.Dtos;
global using CodeDesignPlus.Net.Logger.Extensions;
global using CodeDesignPlus.Net.Mongo.Extensions;
global using CodeDesignPlus.Net.Observability.Extensions;
global using CodeDesignPlus.Net.RabbitMQ.Extensions;
global using CodeDesignPlus.Net.Redis.Extensions;
global using CodeDesignPlus.Net.Security.Extensions;
global using Mapster;
global using MapsterMapper;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using C = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
global using CodeDesignPlus.Net.Serializers;
global using NodaTime;









global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteUser;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetAllUsers;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveTenant;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateJob;
global using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateContact;