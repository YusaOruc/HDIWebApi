﻿using Data.Core.Dtos;
using Survey.Core.Dtos.Survey;
using Survey.Core.Dtos.SurveyResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Interfaces
{
    public interface ISurveyService
    {
        /// <summary>
        /// Anket Ekleme
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task Add(SurveyDto dto);

        /// <summary>
        /// Anket Ekler parentı ile
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task AddPart(int parentId, SurveyDto dto);

        /// <summary>
        /// Anket Detay
        /// </summary>
        /// <param name="id">Anket Id</param>
        /// <returns></returns>
        Task<SurveyListDto> Get(int id);

        /// <summary>
        /// Anket Listeleme
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SurveySummaryListDto>> GetList();

        /// <summary>
        /// Anket Güncelleme
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task Update(int id, SurveyUpdateDto dto);

        /// <summary>
        /// Anket Güncelleme
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task UpdatePart(int id, SurveyPartUpdateDto dto);

        /// <summary>
        /// Anket partlarını siler
        /// </summary>
        /// <param name="id"></param>
        /// <param name="partIds"></param>
        /// <returns></returns>
        Task RemoveParts(int id, List<int>? partIds);

        /// <summary>
        /// Anket İsimlerini Listeleme
        /// </summary>
        /// <param name="parentId">parent Id</param>
        /// <returns></returns>
        Task<IEnumerable<NameDto>> GetNameList(int? parentId, bool isParent);

        /// <summary>
        /// Anketin cevaplarını ekler
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="surveyResults"></param>
        /// <returns></returns>
        Task AddSurveyResultMultiple( int surveyId, List<SurveyResultDto> surveyResults);

        /// <summary>
        /// Anket Cevaplarıı Listeleme
        /// </summary>
        /// <param name="surveyId">parent Id</param>
        /// <param name="userId">user Id</param>
        /// <returns></returns>
        Task<IEnumerable<SurveyResultListDto>> GetSurveyResultList(int? surveyId,string? userId);
    }
}
