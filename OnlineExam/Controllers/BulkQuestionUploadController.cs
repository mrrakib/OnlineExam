using OfficeOpenXml;
using OfficeOpenXml.Style;
using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using OnlineExam.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    public class BulkQuestionUploadController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Message _message = new Message();
        private string fileDownloadName;

        // GET: Subjects
        private readonly IChapterService _chapterService;
        private readonly ISubjectsService _subjectsService;
        private readonly ITopicService _topicService;
        private readonly IMCQQuestionsService _questionsService;

        public BulkQuestionUploadController()
        {
            _subjectsService = new SubjectsService(db);
            _chapterService = new ChapterService(db);
            _topicService = new TopicService(db);
            _questionsService = new MCQQuestionsService(db);
        }
        // GET: BulkQuestionUpload
        public ActionResult Index()
        {
            ViewBag.SubjectId = new SelectList(_subjectsService.GetAllSubjectDDL(), "Id", "Name");
            ViewBag.ChapterId = new SelectList(_chapterService.GetAllChpaterBySubjectDDL(0), "Id", "Name");
            ViewBag.TopicId = new SelectList(_topicService.GetAllTopicsByChapterDDL(0), "Id", "Name");
            ViewBag.QuestionType = new SelectList(_questionsService.GetAllQuestionType(), "Name", "Name");
            return View();
        }



        [HttpPost]
        public ActionResult GenerateExcel(BulkQuestionCommon model)
        {
            string filename = "question_format.xlsx";
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            #region Dummy Topic
            var topic = _topicService.GetFirstTopicByChapterId(model.ChapterId);
            model.TopicId = topic.Id;
            #endregion

            if (model.QuestionType.Equals("SBA"))
            {
                return RedirectToAction("DownloadSBA", new { SubjectId = model.SubjectId, ChapterId = model.ChapterId, TopicId = model.TopicId, QuestionType = model.QuestionType, fileName = filename });
            }
            else
            {
                return RedirectToAction("DownloadMCQ", new { SubjectId = model.SubjectId, ChapterId = model.ChapterId, TopicId = model.TopicId, QuestionType = model.QuestionType, fileName = filename });
            }
        }

        #region SBA Downlaod format
        public ActionResult DownloadSBA(int SubjectId, int ChapterId, int TopicId, string QuestionType, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fileName))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Questions");

                #region question info
                Subject subject = _subjectsService.GetDetails(SubjectId);
                Chapter chapter = _chapterService.GetDetails(ChapterId);
                Topic topic = _topicService.GetDetails(TopicId);

                worksheet.TabColor = Color.Gold;
                worksheet.DefaultRowHeight = 12;
                worksheet.Row(1).Height = 20;

                worksheet.Cells[1, 1].Value = "Subject";
                worksheet.Cells[1, 2].Value = subject != null ? subject.SubjectName : "N/A";

                worksheet.Cells[2, 1].Value = "Chapter";
                worksheet.Cells[2, 2].Value = chapter != null ? chapter.ChapterName : "N/A";

                //worksheet.Cells[3, 1].Value = "Topic";
                //worksheet.Cells[3, 2].Value = topic != null ? topic.TopicName : "N/A";

                worksheet.Cells[4, 1].Value = "Type";
                worksheet.Cells[4, 2].Value = QuestionType;
                #endregion

                #region header
                //fill row 4 with striped orange background
                worksheet.Cells["A6:I6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A6:I6"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);

                worksheet.Cells[6, 1].Value = "Question";
                worksheet.Cells[6, 2].Value = "Explanation";
                worksheet.Cells[6, 3].Value = "Mark";
                worksheet.Cells[6, 4].Value = "Option 1";
                worksheet.Cells[6, 5].Value = "Option 2";
                worksheet.Cells[6, 6].Value = "Option 3";
                worksheet.Cells[6, 7].Value = "Option 4";
                worksheet.Cells[6, 8].Value = "Option 5";
                worksheet.Cells[6, 9].Value = "Answer";

                var answers = worksheet.DataValidations.AddListValidation("I7:I60");
                answers.Formula.Values.Add("Option 1");
                answers.Formula.Values.Add("Option 2");
                answers.Formula.Values.Add("Option 3");
                answers.Formula.Values.Add("Option 4");
                answers.Formula.Values.Add("Option 5");

                #endregion

                #region hidden column for ids
                worksheet.Cells[1, 10].Value = subject.Id;
                worksheet.Cells[2, 10].Value = chapter.Id;
                worksheet.Cells[3, 10].Value = topic.Id;
                worksheet.Column(10).Hidden = true;
                #endregion


                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = subject.SubjectName + "_" + chapter.ChapterName + "_" + topic.TopicName + "_SBA_Format.xlsx";

                return fsr;
            }
        }
        #endregion

        #region MCQ Format
        public ActionResult DownloadMCQ(int SubjectId, int ChapterId, int TopicId, string QuestionType, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fileName))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Questions");

                #region question info
                Subject subject = _subjectsService.GetDetails(SubjectId);
                Chapter chapter = _chapterService.GetDetails(ChapterId);
                Topic topic = _topicService.GetDetails(TopicId);

                worksheet.TabColor = Color.Gold;
                worksheet.DefaultRowHeight = 12;
                worksheet.Row(1).Height = 20;

                worksheet.Cells[1, 1].Value = "Subject";
                worksheet.Cells[1, 2].Value = subject != null ? subject.SubjectName : "N/A";

                worksheet.Cells[2, 1].Value = "Chapter";
                worksheet.Cells[2, 2].Value = chapter != null ? chapter.ChapterName : "N/A";

                //worksheet.Cells[3, 1].Value = "Topic";
                //worksheet.Cells[3, 2].Value = topic != null ? topic.TopicName : "N/A";

                worksheet.Cells[4, 1].Value = "Type";
                worksheet.Cells[4, 2].Value = QuestionType;
                #endregion

                #region header
                //fill row 4 with striped orange background
                worksheet.Cells["A6:M6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A6:M6"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);

                worksheet.Cells[6, 1].Value = "Question";
                worksheet.Cells[6, 2].Value = "Explanation";
                worksheet.Cells[6, 3].Value = "Mark";
                worksheet.Cells[6, 4].Value = "Option 1";
                worksheet.Cells[6, 5].Value = "Option 1 Ans";
                worksheet.Cells[6, 6].Value = "Option 2";
                worksheet.Cells[6, 7].Value = "Option 2 Ans";
                worksheet.Cells[6, 8].Value = "Option 3";
                worksheet.Cells[6, 9].Value = "Option 3 Ans";
                worksheet.Cells[6, 10].Value = "Option 4";
                worksheet.Cells[6, 11].Value = "Option 4 Ans";
                worksheet.Cells[6, 12].Value = "Option 5";
                worksheet.Cells[6, 13].Value = "Option 5 Ans";

                var answerOne = worksheet.DataValidations.AddListValidation("E7:E60");
                answerOne.Formula.Values.Add("True");
                answerOne.Formula.Values.Add("False");

                var answerTwo = worksheet.DataValidations.AddListValidation("G7:G60");
                answerTwo.Formula.Values.Add("True");
                answerTwo.Formula.Values.Add("False");

                var answerThree = worksheet.DataValidations.AddListValidation("I7:I60");
                answerThree.Formula.Values.Add("True");
                answerThree.Formula.Values.Add("False");

                var answerFour = worksheet.DataValidations.AddListValidation("K7:K60");
                answerFour.Formula.Values.Add("True");
                answerFour.Formula.Values.Add("False");

                var answerFive = worksheet.DataValidations.AddListValidation("M7:M60");
                answerFive.Formula.Values.Add("True");
                answerFive.Formula.Values.Add("False");

                #endregion

                #region hidden column for ids
                worksheet.Cells[1, 14].Value = subject.Id;
                worksheet.Cells[2, 14].Value = chapter.Id;
                worksheet.Cells[3, 14].Value = topic.Id;
                worksheet.Column(14).Hidden = true;
                #endregion


                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var fileStream = new MemoryStream();
                package.SaveAs(fileStream);
                fileStream.Position = 0;

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = subject.SubjectName + "_" + chapter.ChapterName + "_" + topic.TopicName + "_" + QuestionType + "_Format.xlsx";

                return fsr;
            }
        }
        #endregion

        public ActionResult UploadExcel()
        {
            return View();

        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase excel)
        {
            List<MCQQuestion> questionList = new List<MCQQuestion>();
            if (excel != null && excel.ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(excel.FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    try
                    {
                        using (var package = new ExcelPackage(excel.InputStream))
                        {
                            
                            string questionName = string.Empty;
                            string explanation = string.Empty;
                            string optOne = string.Empty;
                            string optTwo = string.Empty;
                            string optThree = string.Empty;
                            string optFour = string.Empty;
                            string optFive = string.Empty;
                            string ans = string.Empty;
                            string ansOne = string.Empty;
                            string ansTwo = string.Empty;
                            string ansThree = string.Empty;
                            string ansFour = string.Empty;
                            string ansFive = string.Empty;
                            int totalMarks = 0;
                            int subjectId = 0;
                            int chapterId = 0;
                            int topicId = 0;

                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            string questionType = workSheet.Cells[4, 2].Value == null ? string.Empty : workSheet.Cells[4, 2].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });

                            subjectId = Convert.ToInt32(workSheet.Cells[1, 10].Value == null ? "0" : workSheet.Cells[1, 10].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));
                            chapterId = Convert.ToInt32(workSheet.Cells[2, 10].Value == null ? "0" : workSheet.Cells[2, 10].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));
                            topicId = Convert.ToInt32(workSheet.Cells[3, 10].Value == null ? "0" : workSheet.Cells[3, 10].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));


                            #region SBA Upload
                            if (questionType.TrimEnd().ToLower().Equals("sba"))
                            {
                                for (int i = 7; i <= noOfRow; i++)
                                {
                                    MCQQuestion question = new MCQQuestion();

                                    #region Excel data read
                                    questionName = workSheet.Cells[i, 1].Value == null ? "" : workSheet.Cells[i, 1].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });

                                    explanation = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });

                                    totalMarks = Convert.ToInt32(workSheet.Cells[i, 3].Value == null ? "" : workSheet.Cells[i, 3].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));

                                    optOne = workSheet.Cells[i, 4].Value == null ? "" : workSheet.Cells[i, 4].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optTwo = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optThree = workSheet.Cells[i, 6].Value == null ? "" : workSheet.Cells[i, 6].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optFour = workSheet.Cells[i, 7].Value == null ? "" : workSheet.Cells[i, 7].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optFive = workSheet.Cells[i, 8].Value == null ? "" : workSheet.Cells[i, 8].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    ans = workSheet.Cells[i, 9].Value == null ? "" : workSheet.Cells[i, 9].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    #endregion

                                    #region Main Question

                                    question.QuestionName = questionName;
                                    question.Explanation = explanation;
                                    question.Mark = totalMarks;
                                    question.IsMCQ = false;
                                    question.SubjectId = subjectId;
                                    question.ChapterId = chapterId;
                                    question.TopicId = topicId;
                                    #endregion

                                    #region find total options
                                    int optCount = 0;
                                    if (!string.IsNullOrEmpty(optOne))
                                        optCount++;
                                    if (!string.IsNullOrEmpty(optTwo))
                                        optCount++;
                                    if (!string.IsNullOrEmpty(optThree))
                                        optCount++;
                                    if (!string.IsNullOrEmpty(optFour))
                                        optCount++;
                                    if (!string.IsNullOrEmpty(optFive))
                                        optCount++;
                                    #endregion

                                    #region find answer
                                    bool isOneAns = false, isTwoAns = false, isThreeAns = false, isFourAns = false, isFiveAns = false;

                                    switch (ans.TrimEnd().ToLower())
                                    {
                                        case "option 1":
                                            isOneAns = true;
                                            break;
                                        case "option 2":
                                            isTwoAns = true;
                                            break;
                                        case "option 3":
                                            isThreeAns = true;
                                            break;
                                        case "option 4":
                                            isFourAns = true;
                                            break;
                                        case "option 5":
                                            isFiveAns = true;
                                            break;
                                    }

                                    #endregion

                                    #region getting option list
                                    List<MCQQuestionOption> optionList = new List<MCQQuestionOption>();
                                    if (!string.IsNullOrEmpty(optOne))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = isOneAns,
                                            OptionName = optOne
                                        });
                                    }

                                    if (!string.IsNullOrEmpty(optTwo))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = isTwoAns,
                                            OptionName = optTwo
                                        });
                                    }
                                    if (!string.IsNullOrEmpty(optThree))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = isThreeAns,
                                            OptionName = optThree
                                        });
                                    }
                                    if (!string.IsNullOrEmpty(optFour))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = isFourAns,
                                            OptionName = optFour
                                        });
                                    }
                                    if (!string.IsNullOrEmpty(optFive))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = isFiveAns,
                                            OptionName = optFive
                                        });
                                    }
                                    #endregion

                                    question.MCQQuestionOptions = optionList;
                                    questionList.Add(question);

                                }
                            }
                            #endregion

                            #region MCQ Upload
                            else if (questionType.TrimEnd().ToLower().Equals("mcq"))
                            {
                                subjectId = Convert.ToInt32(workSheet.Cells[1, 14].Value == null ? "0" : workSheet.Cells[1, 14].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));
                                chapterId = Convert.ToInt32(workSheet.Cells[2, 14].Value == null ? "0" : workSheet.Cells[2, 14].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));
                                topicId = Convert.ToInt32(workSheet.Cells[3, 14].Value == null ? "0" : workSheet.Cells[3, 14].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));

                                for (int i = 7; i <= noOfRow; i++)
                                {
                                    MCQQuestion question = new MCQQuestion();

                                    #region Excel data read
                                    questionName = workSheet.Cells[i, 1].Value == null ? "" : workSheet.Cells[i, 1].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });

                                    explanation = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });

                                    totalMarks = Convert.ToInt32(workSheet.Cells[i, 3].Value == null ? "" : workSheet.Cells[i, 3].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' }));

                                    optOne = workSheet.Cells[i, 4].Value == null ? "" : workSheet.Cells[i, 4].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optTwo = workSheet.Cells[i, 6].Value == null ? "" : workSheet.Cells[i, 6].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optThree = workSheet.Cells[i, 8].Value == null ? "" : workSheet.Cells[i, 8].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optFour = workSheet.Cells[i, 10].Value == null ? "" : workSheet.Cells[i, 10].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    optFive = workSheet.Cells[i, 12].Value == null ? "" : workSheet.Cells[i, 12].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    ansOne = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    ansTwo = workSheet.Cells[i, 7].Value == null ? "" : workSheet.Cells[i, 7].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    ansThree = workSheet.Cells[i, 9].Value == null ? "" : workSheet.Cells[i, 9].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    ansFour = workSheet.Cells[i, 11].Value == null ? "" : workSheet.Cells[i, 11].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    ansFive = workSheet.Cells[i, 13].Value == null ? "" : workSheet.Cells[i, 13].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ' });
                                    #endregion

                                    #region Main Question

                                    question.QuestionName = questionName;
                                    question.Explanation = explanation;
                                    question.Mark = totalMarks;
                                    question.IsMCQ = true;
                                    question.SubjectId = subjectId;
                                    question.ChapterId = chapterId;
                                    question.TopicId = topicId;
                                    #endregion

                                    #region find answer
                                    bool isOneAns = false, isTwoAns = false, isThreeAns = false, isFourAns = false, isFiveAns = false;

                                    switch (ans.TrimEnd().ToLower())
                                    {
                                        case "option 1":
                                            isOneAns = true;
                                            break;
                                        case "option 2":
                                            isTwoAns = true;
                                            break;
                                        case "option 3":
                                            isThreeAns = true;
                                            break;
                                        case "option 4":
                                            isFourAns = true;
                                            break;
                                        case "option 5":
                                            isFiveAns = true;
                                            break;
                                    }

                                    #endregion

                                    #region getting option list
                                    List<MCQQuestionOption> optionList = new List<MCQQuestionOption>();
                                    if (!string.IsNullOrEmpty(optOne))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = ansOne.TrimEnd().ToLower().Equals("true") ? true : false,
                                            OptionName = optOne
                                        });
                                    }

                                    if (!string.IsNullOrEmpty(optTwo))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = ansTwo.TrimEnd().ToLower().Equals("true") ? true : false,
                                            OptionName = optTwo
                                        });
                                    }
                                    if (!string.IsNullOrEmpty(optThree))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = ansThree.TrimEnd().ToLower().Equals("true") ? true : false,
                                            OptionName = optThree
                                        });
                                    }
                                    if (!string.IsNullOrEmpty(optFour))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = ansFour.TrimEnd().ToLower().Equals("true") ? true : false,
                                            OptionName = optFour
                                        });
                                    }
                                    if (!string.IsNullOrEmpty(optFive))
                                    {
                                        optionList.Add(new MCQQuestionOption
                                        {
                                            IsAnswer = ansFive.TrimEnd().ToLower().Equals("true") ? true : false,
                                            OptionName = optFive
                                        });
                                    }
                                    #endregion

                                    question.MCQQuestionOptions = optionList;
                                    questionList.Add(question);

                                }
                            } 
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        _message.custom(this, "Unable to upload file. Please contact your admin.");
                    }
                }
                else
                {
                    _message.custom(this, "Only Excel file is allowed to upload.");
                }
            }

            if (_questionsService.AddMultiple(questionList))
            {
                _message.save(this);
                return RedirectToAction("Index");
            }
            _message.custom(this, "No data saved.");
            return View();

        }

        #region ajax calls
        public ActionResult GetAllChapterBySubject(int subjectId)
        {
            var chapters = _chapterService.GetAllChpaterBySubjectDDL(subjectId);
            return Json(chapters, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllTopicByChapter(int chapterId)
        {
            var topics = _topicService.GetAllTopicsByChapterDDL(chapterId);
            return Json(topics, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [AllowAnonymous]
        public JsonResult GetChapterBySubjectId(int subjectId)
        {
            var chapters = _chapterService.GetChapterBySubjectId(subjectId);
            return Json(chapters, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetTopicByChapterId(int chapterId)
        {
            var chapters = _topicService.GetTopicByChapterId(chapterId);
            return Json(chapters, JsonRequestBehavior.AllowGet);
        }
    }
}