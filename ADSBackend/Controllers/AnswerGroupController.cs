﻿using FileTypeChecker;
using FileTypeChecker.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Scholarships.Data;
using Scholarships.Models.Forms;
using Scholarships.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Controllers
{
    [Authorize]
    public class AnswerGroupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;
        private readonly ViewRenderService _viewRenderService;
        private readonly Services.Configuration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AnswerGroupController(ApplicationDbContext context, DataService dataService, ViewRenderService viewRenderService, Services.Configuration configurationService, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _dataService = dataService;
            _viewRenderService = viewRenderService;
            _configuration = configurationService;
            _hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// Returns a form built from a questionset
        /// </summary>
        /// <param name="id">AnswerGroupId from a previously constructed AnswerGroup</param>
        /// <returns></returns>
        ///         
        [Produces("application/json")]
        public async Task<object> QuestionFormAjax(int? id)
        {
            if (id == null)
                return NotFound();

            // The form still needs the answer group Id
            QuestionSet qset = await _dataService.GetQuestionSetWithAnswers((int)id);

            if (qset == null)
                return NotFound();

            var result = new Dictionary<string, object>
            {
                { "primaryForm", await _viewRenderService.RenderToStringAsync("QuestionFormAjax", qset) }
            };

            Dictionary<string, IEnumerable<object>> files = new Dictionary<string, IEnumerable<object>>();

            foreach (AnswerSet answerset in qset.AnswerSets)
            {
                foreach (Answer answer in answerset.Answers)
                {
                    if (answer.FileAttachmentGroup != null)
                    {
                        var attachments = answer.FileAttachmentGroup.FileAttachments.Select(fa => new
                        {
                            filename = fa.FileName,
                            attachid = fa.FileAttachmentId,
                            uuid = fa.FileAttachmentUuid,
                            size = fa.Length,
                            type = fa.ContentType,
                            url = Url.ActionLink("Download", "SecureDownload", new { id = fa.FileAttachmentId, filename = fa.FileName }, protocol: "https")
                        }).ToList();
                        files.Add("" + answer.AnswerSetId + "." + answer.QuestionId, attachments);
                    }
                }
            }


            result.Add("attachments", files);

            return result;
        }

        /// <summary>
        /// Saves an ajax submitted form from /Scholarships/Apply/{id}
        /// </summary>
        /// <param name="id">AnswerGroup Id</param>
        /// <param name="asets">List of partial answersets constructed from form data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<FormsBaseViewModel> Save(int? id, ICollection<AnswerSet> asets)
        {
            if (id == null)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotFound };

            // First let's see if we can load the relevant question set
            var qset = await _dataService.GetQuestionSetWithAnswers((int)id);

            if (qset == null)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotFound };

            foreach (var aset in asets)
            {
                var safeAnswerSet = qset.AnswerSets.FirstOrDefault(a => a.AnswerSetId == aset.AnswerSetId);

                // Check to see if the answerSet is already saved
                if (safeAnswerSet == null)
                {
                    // TODO: Create an answerset to hold this answer?
                    continue;  // For now we don't have to worry about multiple answersets
                }

                if (aset.Answers == null)
                    continue;

                // Iterate through the answers and save them 
                for (int i = 0; i < aset.Answers.Count; i++)
                {
                    var answer = aset.Answers[i];
                    var safeAnswer = safeAnswerSet.Answers.FirstOrDefault(q => q.QuestionId == answer.QuestionId);

                    if (safeAnswer == null) continue;

                    safeAnswer.Response = answer.Response ?? "";
                    safeAnswer.DateTime = answer.DateTime;
                    safeAnswer.Config = JsonConvert.SerializeObject(new { answer.QuestionOptions });
                    safeAnswer.QuestionOptionId = answer.QuestionOptionId;

                    // Each answer must be validated, answerId likely to be invalid
                    if (safeAnswer.AnswerId == 0)
                    {
                        _context.Answer.Add(safeAnswer);
                    }
                    else
                    {
                        _context.Answer.Update(safeAnswer);
                    }
                }

                await _context.SaveChangesAsync();
            }

            var vm = new QuestionSetViewModel
            {
                ErrorCode = QuestionSetError.NoError,
                PrimaryForm = await _viewRenderService.RenderToStringAsync("QuestionFormAjax", qset)
            };

            return vm;
        }

        public async Task<IActionResult> Remove(int id, string uuid)
        {
            string attachPath = _configuration.ConfigPath.AttachmentPath;

            var profile = await _dataService.GetProfileAsync();

            if (profile == null)
                return NotFound();

            var fa = await _context.FileAttachment.Include(a => a.FileAttachmentGroup)
                                                  .FirstOrDefaultAsync(a => a.FileAttachmentId == id && a.FileAttachmentUuid == uuid);

            // Does this file exist?
            if (fa == null)
            {
                return NotFound();
            }

            if (fa.FileAttachmentGroup.ProfileId == profile.ProfileId)
            {
                var filePath = Path.Combine(attachPath,
                                                      fa.FileSubPath,
                                                      fa.SecureFileName);

                try
                {
                    _context.FileAttachment.Remove(fa);
                    await _context.SaveChangesAsync();

                    System.IO.File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to delete file: {0}", filePath);
                }
            }

            return Ok();
        }

        // See https://stackoverflow.com/questions/17759286/how-can-i-show-you-the-files-already-stored-on-server-using-dropzone-js for how to integrate already existing files into display
        // Scroll down to section that shows init function solution
        [Produces("application/json")]
        public async Task<object> Upload(IEnumerable<IFormFile> files, int questionid, int answersetid)
        {
            string attachPath = _configuration.ConfigPath.AttachmentPath;
            var profile = await _dataService.GetProfileAsync();

            var aset = await _context.AnswerSet.Include(a => a.Answers)
                                               .FirstOrDefaultAsync(a => a.AnswerSetId == answersetid && a.ProfileId == profile.ProfileId);

            if (aset == null)
                return NotFound();

            var answer = aset.Answers.FirstOrDefault(a => a.QuestionId == questionid);

            if (answer == null)
            {
                answer = new Answer
                {
                    AnswerSetId = aset.AnswerSetId,
                    QuestionId = questionid
                };
            }

            if (answer.FileAttachmentGroupId == null)
            {
                // We need to create a new file attachment group
                FileAttachmentGroup fg = new FileAttachmentGroup { ProfileId = profile.ProfileId };
                _context.FileAttachmentGroup.Add(fg);
                await _context.SaveChangesAsync();

                answer.FileAttachmentGroupId = fg.FileAttachmentGroupId;
                _context.Answer.Update(answer);
                await _context.SaveChangesAsync();
            }

            //long totalsize = files.Sum(f => f.Length);

            // full path to file in temp location
            // var filePath = Path.GetTempFileName();
            List<object> response = new List<object>();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    FileAttachment fa = new FileAttachment
                    {
                        FileAttachmentGroupId = (int)answer.FileAttachmentGroupId,
                        FileName = Path.GetFileName(formFile.FileName),
                        ContentType = formFile.ContentType,
                        CreatedDate = DateTime.Now,
                        Length = formFile.Length,
                        SecureFileName = Path.GetRandomFileName()
                    };

                    if (!fa.ContentType.StartsWith("image/") && fa.ContentType != "application/pdf")
                        continue;

                    List<string> pathParts = new List<string>();
                    pathParts.Add("" + DateTime.Now.Year);
                    pathParts.Add("" + profile.ProfileId);
                    pathParts.Add("" + fa.FileAttachmentGroupId);

                    // Calculate the subpath based on the current year and the user's profile Id
                    fa.FileSubPath = Path.Combine(pathParts.ToArray());

                    // Now let's build the correct filepath
                    pathParts.Insert(0, attachPath);

                    var filePath = Path.Combine(pathParts.ToArray());

                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(filePath);

                    // Now add the secure filename and build the full file path
                    pathParts.Add(fa.SecureFileName);
                    filePath = Path.Combine(pathParts.ToArray());


                    using (var ftStream = formFile.OpenReadStream())
                    {
                        // Examine the file byte structure to validate the type
                        IFileType fileType = FileTypeValidator.GetFileType(ftStream);

                        switch (fileType.Extension)
                        {
                            case "jpg":
                            case "png":
                            case "gif":
                            case "pdf":
                            case "doc":
                            case "docx":

                                _context.FileAttachment.Add(fa);
                                await _context.SaveChangesAsync();

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await formFile.CopyToAsync(stream);
                                }

                                response.Add(new
                                {
                                    status = "verified",
                                    attachid = fa.FileAttachmentId,
                                    uuid = fa.FileAttachmentUuid,
                                    fa.FileName
                                });

                                break;
                            default:
                                response.Add(new
                                {
                                    status = "invalid",
                                    uuid = "",
                                    fa.FileName
                                });

                                break;
                        }
                    }



                }
            }

            return response;
        }


    }
}
