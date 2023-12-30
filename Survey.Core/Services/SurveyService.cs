﻿using Auth.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Core.DbContexts;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
            var obj = _mapper.Map<SurveyBase>(dto);

            obj = await AddInitial(obj);

            foreach (var part in dto.Parts)
            {
                var partObj = _mapper.Map<SurveyBase>(part);
                partObj.ParentId = obj.Id;
                var result = await AddInitial(partObj);
            }
            
        }
        public async Task<SurveyBase> AddInitial(SurveyBase obj)
        {
            var userId = await _sessionService.GetAuthenticatedUserIdAsync();
            var created = DateTime.UtcNow;;

            obj.CreateDate = created;
            obj.LastUpdateDate = created;
            obj.Creator = userId;
            obj.Updater = userId;

            foreach (var question in obj.SurveyQuestions)
            {
                question.CreateDate = created;
                question.LastUpdateDate = created;
                question.Creator = userId;
                question.Updater = userId;

                foreach (var option in question.SurveyQuestionOptions)
                {
                    option.CreateDate = created;
                    option.LastUpdateDate = created;
                    option.Creator = userId;
                    option.Updater = userId;
                }
            }

            _context.SurveyBases.Add(obj);
            await _context.SaveChangesAsync();
            return obj;
        }
        public async Task<SurveyListDto> Get(int id)
        {
            var obj = await _context.SurveyBases.AsNoTracking()
                .Where(e => e.Id == id)
                .ProjectTo<SurveyListDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return obj;
        }

        public async Task<IEnumerable<SurveySummaryListDto>> GetList()
        {
            var userId = await _sessionService.GetAuthenticatedUserIdAsync();


            var result = await _context.SurveyBases.AsNoTracking()
                .Include(t=>t.SurveyQuestions)
                .Where(t => t.Creator == userId)
                .Where(t => t.ParentId == null)
                .ProjectTo<SurveySummaryListDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(t => t.CreateDate)
                .ToListAsync();

            return result;
        }

        public async Task Update(int id, SurveyUpdateDto dto)
        {
            var userId = await _sessionService.GetAuthenticatedUserIdAsync();
            var obj = await _context.SurveyBases.AsTracking().Where(t => t.Id == id).FirstOrDefaultAsync();


            var created = DateTime.UtcNow;
            obj = _mapper.Map(dto, obj);

            obj.LastUpdateDate = created;
            obj.Updater = userId;

            foreach (var question in obj.SurveyQuestions)
            {
                question.CreateDate = created;
                question.LastUpdateDate = created;
                question.Creator = userId;
                question.Updater = userId;

                foreach (var option in question.SurveyQuestionOptions)
                {
                    option.CreateDate = created;
                    option.LastUpdateDate = created;
                    option.Creator = userId;
                    option.Updater = userId;
                }
            }
            //Silinmiş sorular bul. Soruları ve cevaplarıı veritabanından sil.

            var deletedOptions = new List<SurveyQuestionOption>();
            foreach (var question in obj.SurveyQuestions)
            {
                var options = await _context.SurveyQuestionOptions.AsTracking().Where(t => !question.SurveyQuestionOptions.Contains(t) && t.SurveyQuestionId == question.Id).ToListAsync();
                deletedOptions.AddRange(options);
            }

            var deletedQuestions = await _context.SurveyQuestions.AsTracking().Where(t => !obj.SurveyQuestions.Contains(t) && t.SurveyBaseId == id).ToListAsync();

            foreach (var question in deletedQuestions)
            {
                var options = await _context.SurveyQuestionOptions.Where(t => t.SurveyQuestionId == question.Id).ToListAsync();
                deletedOptions.AddRange(options);
            }
            _context.SurveyQuestionOptions.RemoveRange(deletedOptions);
            _context.SurveyQuestions.RemoveRange(deletedQuestions);

            await _context.SaveChangesAsync();
        }
    }
}