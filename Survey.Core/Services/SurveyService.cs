﻿using Auth.Core.Interfaces;
using AutoMapper;
using Data.Core.DbContexts;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Survey.Core.Dtos.Survey;
using Survey.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly HdiDbContext _context;
        private readonly IMapper _mapper;
        private readonly ISessionService _sessionService;
        public SurveyService(HdiDbContext context, IMapper mapper, ISessionService sessionService)
        {
            _context = context;
            _mapper = mapper;
            _sessionService = sessionService;
        }

        public async Task Add(SurveyDto dto)
        {
            var userId = await _sessionService.GetAuthenticatedUserIdAsync();

            var created = DateTime.UtcNow;

            var obj = _mapper.Map<SurveyBase>(dto);

            obj.CreateDate = created;
            obj.LastUpdateDate = created;
            obj.Creator = userId;
            obj.Updater = userId;
        }
    }
}
