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









global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.CreateUsers;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateUsers;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.DeleteUsers;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetUsersById;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetAllUsers;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.AddTenant;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.RemoveTenant;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.AddRole;
global using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.RemoveRole;