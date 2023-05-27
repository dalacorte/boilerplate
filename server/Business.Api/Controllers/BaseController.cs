using AutoMapper;
using Business.Domain.Helpers;
using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.UOW;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Business.Api.Controllers
{
    public class BaseController<TEntity, TContext, TViewModel, TController> : Controller
        where TEntity : class
        where TContext : IUnitOfWork
        where TViewModel : class, IViewModel<TEntity>
        where TController : Controller
    {
        protected readonly IMapper _mapper;
        protected readonly IValidator<TEntity> _validator;
        protected readonly IBaseApplication<TEntity, TContext> _application;
        protected readonly IMemoryCache _cache;
        protected readonly IStringLocalizer _localizer;
        protected readonly ILogger _logger;

        public BaseController(IMapper mapper,
                              IValidator<TEntity> validator,
                              IBaseApplication<TEntity, TContext> application,
                              IMemoryCache cache,
                              IStringLocalizer localizer,
                              ILogger<TController> logger)
        {
            _mapper = mapper;
            _validator = validator;
            _application = application;
            _cache = cache;
            _localizer = localizer;
            _logger = logger;
        }
    }
}
