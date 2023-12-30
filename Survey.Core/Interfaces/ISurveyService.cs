﻿using Survey.Core.Dtos.Survey;
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
    }
}
