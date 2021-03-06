USE [OnlineExamProdDB]
GO
/****** Object:  StoredProcedure [dbo].[GetResultStatus]    Script Date: 07/12/2021 07:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*

EXEC GetExamStatus @ExamId=6, @StudentId='R101'

*/
CREATE PROCEDURE [dbo].[GetExamStatus](
	@ExamId INT=0,
	@StudentId VARCHAR(500)=''
)
AS
BEGIN TRY
    SET NOCOUNT ON;
	BEGIN TRAN;
	DECLARE @TotalExamCount INT=0;
	DECLARE @TotalTodayExamCount INT=0;
	DECLARE @TotalMonthExamCount INT=0;
	DECLARE @TotalYearlyExamCount INT=0;	

	SELECT @TotalExamCount=COUNT(e.Id) FROM [dbo].[Exams] e
	JOIN [dbo].[ExamWiseQuestions] q ON e.Id=q.ExamId
	WHERE e.Id=IIF(@ExamId=0, e.Id, @ExamId) 

	SELECT @TotalTodayExamCount=COUNT(e.Id) FROM [dbo].[Exams] e
	JOIN [dbo].[ExamWiseQuestions] q ON e.Id=q.ExamId
	WHERE e.Id=IIF(@ExamId=0, e.Id, @ExamId) 
	--AND MONTH(r.ExamDateTime)=MONTH(GETDATE())
	--AND MONTH(r.ExamDateTime)=MONTH(GETDATE())
	--AND YEAR(r.ExamDateTime)=YEAR(GETDATE())

	SELECT 0 as Id,
	@StudentId StudentId,
	@ExamId ExamId,
	GETDATE() ExamDate,
	ISNULL(@TotalExamCount,0) TotalExamCount,
	ISNULL(@TotalTodayExamCount,0) TotalTodayExamCount,
	ISNULL(@TotalMonthExamCount,0) TotalMonthExamCount,
	ISNULL(@TotalYearlyExamCount,0) TotalYearlyExamCount;

	COMMIT TRAN;
END TRY
BEGIN CATCH
      IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
      SELECT 'Error Number:' + CAST(ERROR_NUMBER() AS VARCHAR(MAX)) 
	        + ', Error State: '  + CAST(ERROR_STATE() AS VARCHAR(MAX)) 
	        + ', Error Procedure: '  + CAST(ERROR_PROCEDURE() AS VARCHAR(MAX)) 
	        + ', Error Line: '  + CAST(ERROR_LINE() AS VARCHAR(MAX)) 
	        + ', Error Message: '  + CAST(ERROR_MESSAGE() AS VARCHAR(MAX))
END CATCH
