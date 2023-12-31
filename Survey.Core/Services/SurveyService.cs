﻿using Auth.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Core.DbContexts;
using Data.Core.Dtos;
using Data.Core.Entity;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Survey.Core.Dtos.Survey;
using Survey.Core.Dtos.SurveyResult;
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
            if (dto.Parts != null)
            {
                foreach (var part in dto.Parts)
                {
                    var partObj = _mapper.Map<SurveyBase>(part);
                    partObj.ParentId = obj.Id;
                    var result = await AddInitial(partObj);
                }
            }


        }
        public async Task AddPart(int parentId, SurveyDto dto)
        {
            var obj = _mapper.Map<SurveyBase>(dto);

            obj = await AddInitial(obj);
            obj.ParentId = parentId;
            await _context.SaveChangesAsync();

        }
        public async Task<SurveyBase> AddInitial(SurveyBase obj)
        {
            var userId = await _sessionService.GetAuthenticatedUserIdAsync();
            var created = DateTime.UtcNow; ;

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

            obj.Parts = await _context.SurveyBases
                .AsNoTracking()
                .Where(t => t.ParentId == id)
                .ProjectTo<SurveyListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return obj;
        }

        public async Task<IEnumerable<SurveySummaryListDto>> GetList()
        {

            var result = await _context.SurveyBases.AsNoTracking()
                .Include(t => t.SurveyQuestions)
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

        public async Task UpdatePart(int id, SurveyPartUpdateDto dto)
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
        public async Task RemoveParts(int id, List<int>? partIds)
        {
            var deletedParts = new List<SurveyBase>();
            if (partIds == null)
            {
                deletedParts = await _context.SurveyBases.AsTracking().Where(t => t.ParentId == id).ToListAsync();
            }
            else
            {
                deletedParts = await _context.SurveyBases.AsTracking().Where(t => !partIds.Contains(t.Id) && t.ParentId == id).ToListAsync();
            }


            var qustions = new List<SurveyQuestion>();
            foreach (var part in deletedParts)
            {
                part.ParentId = null;
                var obj = await _context.SurveyQuestions.Where(t => t.SurveyBaseId == part.Id).FirstOrDefaultAsync();
                if (obj != default)
                {
                    qustions.Add(obj);
                }

            }
            var options = new List<SurveyQuestionOption>();
            foreach (var qustionId in qustions.Select(t => t.Id))
            {
                var obj = await _context.SurveyQuestionOptions.Where(t => t.SurveyQuestionId == qustionId).FirstOrDefaultAsync();
                if (obj != default)
                {
                    options.Add(obj);
                }
            }
            _context.SurveyQuestionOptions.RemoveRange(options);
            _context.SurveyQuestions.RemoveRange(qustions);
            _context.SurveyBases.RemoveRange(deletedParts);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NameDto>> GetNameList(int? parentId,bool isParent = false)
        {
            var userId = await _sessionService.GetAuthenticatedUserIdAsync();

            var result = await _context.SurveyBases.AsNoTracking()
                .Include(t => t.SurveyQuestions)
                .Where(t => isParent == false || t.ParentId == null)
                .Where(t => parentId == null || t.ParentId == parentId)
                .Select(t => new NameDto
                {
                    Id = t.Id,
                    Name = t.Title
                })
                .OrderByDescending(t => t.Name)
                .ToListAsync();

            return result;
        }

        public async Task AddSurveyResultMultiple(int anketId, List<SurveyResultDto> surveyResults)
        {
            var userId = await _sessionService.GetAuthenticatedUserIdAsync();

            var created = DateTime.UtcNow;

            foreach (var result in surveyResults)
            {

                var updateObj = await _context.SurveyResults
                    .AsTracking()
                    .Where(t => t.ApplicationUserId == userId)
                    .Where(t => t.SurveyBaseId == result.SurveyBaseId)
                    .Where(t => t.SurveyQuestionId == result.SurveyQuestionId)
                    .FirstOrDefaultAsync();




                if (updateObj != null)
                {
                    updateObj = _mapper.Map(result, updateObj);
                    updateObj.LastUpdateDate = created;
                    updateObj.Updater = userId;
                }
                else
                {
                    var obj = _mapper.Map<SurveyResult>(result);
                    obj.CreateDate = created;
                    obj.LastUpdateDate = created;
                    obj.Creator = userId;
                    obj.Updater = userId;
                    obj.ApplicationUserId = userId;

                    _context.SurveyResults.Add(obj);
                }
            }

            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<SurveyResultListDto>> GetSurveyResultList(int? surveyId, string? userId)
        {
            var id = userId ?? await _sessionService.GetAuthenticatedUserIdAsync();


            var result = await _context.SurveyResults
                .AsNoTracking()
                .Where(t => t.ApplicationUserId == id)
                .Where(t => surveyId == null || t.SurveyBaseId == surveyId)
                .Select(t => new SurveyResultListDto
                {
                    Id = t.Id,
                    ApplicationUserId = t.ApplicationUserId,
                    ApplicationUserUserName = t.ApplicationUser.Id,
                    CorrectQuestionOptionIndex = t.SurveyQuestion.CorrectQuestionOptionIndex,
                    IsCorrect = t.SurveyQuestion.CorrectQuestionOptionIndex != null ? t.SurveyQuestion.SurveyQuestionOptions.ToList()[t.SurveyQuestion.CorrectQuestionOptionIndex - 1 ?? 0].Id == t.SurveyQuestionOptionId : false,
                    SurveyQuestionOptionId = t.SurveyQuestionOptionId,
                    CreateDate = t.CreateDate,
                    Creator = t.Creator,
                    LastUpdateDate = t.LastUpdateDate,
                    Updater = t.Updater,
                    SurveyBaseId = t.SurveyBaseId,
                    SurveyBaseTitle = t.SurveyBase.Title,
                    SurveyQuestionId = t.SurveyQuestionId,
                    SurveyQuestionOptionTitle = t.SurveyQuestionOption.Title,
                    SurveyQuestionTitle = t.SurveyQuestion.Title


                })
                .OrderByDescending(t => t.CreateDate)
                .ToListAsync();

            return result;
        }
    }
}
